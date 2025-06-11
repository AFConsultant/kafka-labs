CREATE TABLE neighborhood_departures_count 
AS SELECT 
    \"StartStationNeighborhood\",
    AS_VALUE(\"StartStationNeighborhood\") AS neighborhood,
    FROM_UNIXTIME(WINDOWSTART) AS window_start,
    FROM_UNIXTIME(WINDOWEND) AS window_end,
    COUNT(*) AS departures_count
FROM trips_enriched
WINDOW HOPPING (SIZE 1 HOUR, ADVANCE BY 5 MINUTES)
GROUP BY \"StartStationNeighborhood\"
EMIT CHANGES;