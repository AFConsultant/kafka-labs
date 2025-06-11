CREATE STREAM trips_by_station
  WITH (KAFKA_TOPIC='bike_trips_by_station', VALUE_FORMAT='AVRO')
AS SELECT *
   FROM trips
PARTITION BY CAST(\"StartStationId\" AS STRING);