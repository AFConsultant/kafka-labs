CREATE TABLE station_details (
    station_id VARCHAR PRIMARY KEY,
    station_id_dup VARCHAR,
    station_name VARCHAR,
    latitude DOUBLE,
    longitude DOUBLE,
    neighborhood VARCHAR
) WITH (
    KAFKA_TOPIC='station_infos_raw_csv',
    VALUE_FORMAT='DELIMITED'
);