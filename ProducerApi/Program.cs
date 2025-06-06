var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

const string filePath = "/workspaces/kafka-labs/data/201306-citibike-tripdata_1_6K.csv";

app.MapPost("/trip", async () => {
    
    if (!File.Exists(filePath))
    {
        return Results.NotFound($"Le fichier n'a pas été trouvé à l'emplacement : {filePath}");
    }

    _ = Task.Run(async () => {
        try
        {
            Console.WriteLine("--- Démarrage de la lecture du fichier de trajets ---");
            
            await foreach (var line in File.ReadLinesAsync(filePath))
            {
                Console.WriteLine(line);
                await Task.Delay(1000);
            }
            
            Console.WriteLine("--- Fin de la lecture du fichier ---");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Une erreur est survenue lors de la lecture du fichier : {ex.Message}");
        }
    });
    return Results.Accepted(value: "La lecture du fichier a démarré en arrière-plan.");
})
.WithName("PostTripData")
.WithOpenApi();

app.Run();
