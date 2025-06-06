package com.mmacphail.lab;

class Order {
  public String orderId;
  public String eventType;
  public String timestamp;
  public String service;
  public String customer;
  public String location;

  @Override
  public String toString() {
    return "Order{" +
        "orderId='" + orderId + '\'' +
        ", eventType='" + eventType + '\'' +
        ", timestamp='" + timestamp + '\'' +
        ", service='" + service + '\'' +
        ", customer='" + customer + '\'' +
        ", location='" + location + '\'' +
        '}'; 
  }
}

// { "orderId": "order001", "eventType": "ORDER_PLACED", "timestamp": "2025-04-08T09:00:00Z", "service": "WaffleZoom", "customer": "Alice", "location": "North Zone" }
