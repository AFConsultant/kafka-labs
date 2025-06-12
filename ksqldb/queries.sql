CREATE STREAM trips (
  "TripDuration" INT,
  "StartTime" BIGINT,
  "StopTime" BIGINT,
  "StartStationId" INT,
  "StartStationName" VARCHAR,
  "StartStationLatitude" DOUBLE,
  "StartStationLongitude" DOUBLE,
  "EndStationId" INT,
  "EndStationName" VARCHAR,
  "EndStationLatitude" DOUBLE,
  "EndStationLongitude" DOUBLE,
  "BikeId" INT,
  "UserType" VARCHAR,
  "BirthYear" VARCHAR,
  "Gender" INT
) WITH (
  KAFKA_TOPIC='bike_trips',
  VALUE_FORMAT='AVRO',
  TIMESTAMP='"StartTime"'
);

CREATE STREAM long_trips WITH (KAFKA_TOPIC='long_trips') AS
SELECT
    `StartStationName`,
    `EndStationName`,
    `TripDuration` / 60 AS DurationMinutes
FROM trips
WHERE `TripDuration` / 60 > 10;