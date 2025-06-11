CREATE SINK CONNECTOR jdbc_sink_postgres_departures_01 WITH (
    'connector.class' = 'io.confluent.connect.jdbc.JdbcSinkConnector',
    'connection.url' = 'jdbc:postgresql://postgres:5432/',
    'connection.user' = 'postgres',                          
    'connection.password' = 'postgres',                      
    'topics' = 'NEIGHBORHOOD_DEPARTURES_COUNT',              
    'table.name.format' = 'neighborhood_departures_count',   
    'input.data.format' = 'AVRO',                            
    'key.converter' = 'org.apache.kafka.connect.storage.StringConverter',
    'value.converter' = 'io.confluent.connect.avro.AvroConverter',
    'value.converter.schema.registry.url' = 'http://schema-registry:8081',
    'pk.mode' = 'record_value',
    'pk.fields' = 'NEIGHBORHOOD,WINDOW_START',
    'insert.mode' = 'upsert',
    'auto.create' = 'true',
    'auto.evolve' = 'true',
    'db.timezone' = 'UTC'
);