CREATE STREAM trips_enriched AS
SELECT
  s.station_id AS \"StationId\",
  t.\"TripDuration\",
  t.\"StartTime\",
  t.\"StopTime\",
  t.\"StartStationId\",
  t.\"StartStationName\",
  s.neighborhood AS \"StartStationNeighborhood\",
  t.\"StartStationLatitude\",
  t.\"StartStationLongitude\",
  t.\"EndStationId\",
  t.\"EndStationName\",
  t.\"EndStationLatitude\",
  t.\"EndStationLongitude\",
  t.\"BikeId\",
  t.\"UserType\",
  t.\"BirthYear\",
  t.\"Gender\"
FROM trips_by_station t
LEFT JOIN station_details s 
ON CAST(t.\"StartStationId\" AS STRING) = s.station_id;