# Setup

## Running on your local machine ?

### Prerequisites

* Have docker and docker-compose installed.
* Using java ? Have a JDK 17 installed.

### Configure your host

Add the following to your host:

```text
127.0.0.1 kafka
127.0.0.1 schema-registry
```

## Run docker-compose

Run Kafka by using the following command:

`docker-compose up -d`

Go to [AKHQ](http://localhost:8085/ui). You should see a single topic, `orders`.