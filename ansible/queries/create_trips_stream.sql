CREATE STREAM IF NOT EXISTS trips (
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
