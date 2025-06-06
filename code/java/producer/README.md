# Hints

## Adding the Kafka clients library

Add the `kafka-clients` library to the classpath. Add this dependency to your `build.gradle` file:

```groovy
implementation 'org.apache.kafka:kafka-clients:4.0.0'
```

Note that the version corresponds to the Kafka version.

If you are intelliJ, don't forget to reload your gradle environment.

### Adding the Kafka properties

#### Select the serializer

Use the
[StringSerializer](https://kafka.apache.org/32/javadoc/org/apache/kafka/common/serialization/StringSerializer.html) for the key and the value.

#### Set relevant properties

See the [documentation](https://kafka.apache.org/documentation.html) to have a description of each property.

Add the relevant properties to the `application.properties` file:

* `bootstrap.servers` to set the address of your Kafka cluster
* `key.serializer` full classpath to specify which serializer you use for the key...
* `value.serializer` ... and the value
* `acks` to all
* `enable.idempotence` to all
* `retries` to it's max value (2147483647)
* `batch.size` and `linger.ms` to set up batching
* `compression.type` to add compression (`snappy` is fine here)

### Create the Kafka producer

```java
try(KafkaProducer<String, String> kafkaProducer=new KafkaProducer<>(properties)){

}
```

### Send the messages

* For each message
* Create a `ProducerRecord<String, String>`
* Call the `send()` method of the `KafkaProducer`.

The `ProducerRecord` will contain information about the message you are trying to send: the topic, the key, and the value

```java
ProducerRecord<String, String> record=new ProducerRecord<>(
      "topic","key","value"
);
```

Replace with the appropriate key and value.

Then, to send the message:

```java
kafkaProducer.send(record);
```

Note that when the `send` method is called, the message is not yet sent to Kafka. In practice, the message is added to an
in-memory buffer. The message will be sent alongside other messages to the broker once the batch of messages it belongs
to is ready (remember `batch.size` and `linger.ms` ?).

Calling `send()` is not enough to know if the message has been properly sent. Luckily, the Kafka library provides two
ways of knowing if the message has been sent to Kafka. If you provide no additional argument than the record, then the
send method returns a `Future<RecordMetadata>`. Since it's a java future, you can force waiting for the cluster to repond
by using it's `get` method.

However, a better way exist. You can supply a lambda as the second argument of the `send()` method. This lambda must be a
`Callback` and implement a single method, `void onCompletion(RecordMetadata metadata, Exception exception)`.
If the exception is not `null`, this means there was an error sending the record to Kafka that your need to process in
your applicative code. If the exception is `null`, you are certain the record was sent successfully and you can use the
`RecordMetadata`.

For example, you could use it like this:

```java
kafkaProducer.send(record,(metadata,exception)->{
  if(exception!=null){
    LOG.error("Error while sending record {} to Kafka: {}", record.key(),exception);
  } else {
    LOG.info("Record {} - {} sent to Kafka", record.key(), record.value());
  }
});
```

### Run the producer

Run the producer using gradle: `./gradlew producer:run`.
Check [AKHQ](http://localhost:8085). The messages you sent will be in the `orders` topic. Check the messages in this
topic using the 🔎 in AKHQ.