# Overview

This repository contains a sample implementation of a signing system that allows for secure webhooks.

# Components

**WebhooksServer**

Sends payloads with signed messages to the WebhooksClient.

Endpoints:
- `/Keys` : Provides JSON-format public keys for clients to pull to verify payloads.

- `/SendMessage` : Initiates sending a message from the WebhooksServer to the WebhooksClient. For testing purposes.

**WebhooksClient**

Receives and validates signed message payloads from the WebhooksServer.

- `/Receive` : Receives the message from the WebhooksServer and validates it.


# Workflow

1. Initiate call to `/SendMessage` on WebhooksServer
2. WebhooksServer creates payload and generates a signed hash.
3. WebhooksServer calls WebhooksClient with `X-Webhooks-Signature` header containing signed hash
4. WebhooksClient receives message at `/Receive`
5. WebhooksClient pulls signing keys from WebhooksServer `/Keys` endpoint
6. WebhooksClient uses signing keys to validate signed hash and compares request body hash with signed hash to determine request legitimacy