# Lab 02 - Create a Kafka Producer

## Introducing QuickWaffle

QuickWaffle, a new ultra-fast waffle delivery, promising to deliver hot waffles in under 20 minutes.
You work in the system that manages QuickWaffles order.
You need to communicate the orders to a subsystem.
To start, you need to send all the orders in a Json format to the `orders` topic.
The mock data you will work with is in the [orders.ndjson](../code/orders.ndjson) file.

This file contains lines of orders like this: 
```json
{ "orderId": "order001", "eventType": "ORDER_PLACED", "timestamp": "2025-04-08T09:00:00Z", "service": "WaffleZoom", "customer": "Alice", "location": "North Zone" }
```
Each order has several properties, along the `orderId`.
Send all the orders as Json to the `orders` topic. Important : use the `orderId` as a key.

A small code skeletton has been provided for you. See the `producer` project, in the `code` folder (use the language of your choice).
