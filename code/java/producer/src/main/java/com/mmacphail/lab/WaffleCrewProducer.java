package com.mmacphail.lab;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.Properties;
import java.util.stream.Stream;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

public class WaffleCrewProducer {
  private static final Logger LOG = LoggerFactory.getLogger(WaffleCrewProducer.class);

  private static final String ORDERS_FILE_PATH = "../../orders.ndjson";

  public static void main(String[] args) {
    Properties properties = loadApplicationProperties();

    var orderStream = loadJsonOrderStream(ORDERS_FILE_PATH);

    orderStream.forEach(jsonOrder -> {
      var order = parseOrder(jsonOrder);
      LOG.info("parsed order: {}", order);
    });
  }

  private static Properties loadApplicationProperties() {
    try {
      Properties properties = new Properties();
      properties.load(WaffleCrewProducer.class.getClassLoader().getResourceAsStream("application.properties"));
      return properties;
    } catch (IOException e) {
      throw new RuntimeException(e);
    }
  }

  private static Stream<String> loadJsonOrderStream(String fileName) {
    try {
      return Files.lines(Paths.get(fileName));
    } catch (IOException e) {
      throw new RuntimeException(e);
    }
  }

  private static final ObjectMapper OBJECT_MAPPER = new ObjectMapper();

  private static Order parseOrder(String json) {
    try {
      return OBJECT_MAPPER.readValue(json, Order.class);
    } catch (JsonProcessingException e) {
      throw new RuntimeException(e);
    }
  }
}
