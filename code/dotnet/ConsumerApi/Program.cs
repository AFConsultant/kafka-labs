using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;
using CsvIndex = CsvHelper.Configuration.Attributes.IndexAttribute;

var builder = WebApplication.CreateBuilder(args);

var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var dbPath = Path.Combine(userProfile, "citibike.db");

// The database file will be created at /home/vscode/citibike.db
builder.Services.AddDbContext<TripDbContext>(options => 
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/trips", async (TripDbContext db, [FromQuery] int page = 1, [FromQuery] int pageSize = 100) =>
{
    pageSize = Math.Clamp(pageSize, 1, 1000);

    var trips = await db.Trips
        .AsNoTracking()
        .OrderBy(t => t.StartTime)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Results.Ok(trips);
});

app.MapGet("/trip/{tripId:int}", async (int tripId, TripDbContext db) =>
{
    var trip = await db.Trips.FindAsync(tripId);

    if (trip is null)
    {
        return Results.NotFound($"Trip with ID {tripId} not found.");
    }

    return Results.Ok(trip);
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TripDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();

public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(IServiceScopeFactory scopeFactory, ILogger<KafkaConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () => await ConsumeLoop(stoppingToken), stoppingToken);
    }

    private async Task ConsumeLoop(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "kafka:9092",
            GroupId = "citibike-dotnet-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        const string topic = "bike_trips_raw_csv";

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(topic);
        _logger.LogInformation("Kafka Consumer started. Subscribed to '{Topic}' topic.", topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    var csvLine = consumeResult.Message.Value;
                    
                    if (string.IsNullOrWhiteSpace(csvLine)) continue;

                    await ProcessCsvLine(csvLine);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError("Error consuming from Kafka: {Reason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer is shutting down.");
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessCsvLine(string csvLine)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TripDbContext>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        try
        {
            using var reader = new StringReader(csvLine);
            using var csv = new CsvReader(reader, config);

            var numberStyle = System.Globalization.NumberStyles.Float;
            csv.Context.TypeConverterOptionsCache.GetOptions<int?>().NumberStyles = numberStyle;
            csv.Context.TypeConverterOptionsCache.GetOptions<int>().NumberStyles = numberStyle;

            if (csv.Read())
            {
                var record = csv.GetRecord<CitiBikeTrip>();
                if (record != null)
                {
                    dbContext.Trips.Add(record);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Successfully processed and saved trip with BikeId: {record.BikeId}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to process CSV line: \"{csvLine}\"");
        }
    }
}

public class CitiBikeTrip
{
    [CsvIndex(0)]
    public int TripDuration { get; set; }

    [CsvIndex(1)]
    public DateTime StartTime { get; set; }

    [CsvIndex(2)]
    public DateTime StopTime { get; set; }

    [CsvIndex(3)]
    public int StartStationId { get; set; }

    [CsvIndex(4)]
    public string? StartStationName { get; set; }

    [CsvIndex(5)]
    public double StartStationLatitude { get; set; }

    [CsvIndex(6)]
    public double StartStationLongitude { get; set; }

    [CsvIndex(7)]
    public int? EndStationId { get; set; }

    [CsvIndex(8)]
    public string? EndStationName { get; set; }

    [CsvIndex(9)]
    public double? EndStationLatitude { get; set; }

    [CsvIndex(10)]
    public double? EndStationLongitude { get; set; }

    [CsvIndex(11)]
    public int BikeId { get; set; }

    [CsvIndex(12)]
    public string? UserType { get; set; }

    [CsvIndex(13)]
    public string? BirthYear { get; set; }

    [CsvIndex(14)]
    public int Gender { get; set; }

    [Key]
    [Ignore]
    public int Id { get; set; }
}


/// <summary>
/// The Entity Framework database context. It represents the session with the
/// database and allows us to query and save data.
/// </summary>
public class TripDbContext : DbContext
{
    // Constructor that accepts DbContextOptions, allowing us to configure
    // it from Program.cs (e.g., to specify the database provider and connection string).
    public TripDbContext(DbContextOptions<TripDbContext> options) : base(options) { }

    // Represents the "Trips" table in the database.
    // Each row in the table will correspond to a CitiBikeTrip object.
    public DbSet<CitiBikeTrip> Trips { get; set; }
}
