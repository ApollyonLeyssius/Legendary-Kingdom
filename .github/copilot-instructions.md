# Copilot Instructions

## Project Guidelines
- User prefers Unity event classes to follow the existing GameEventsManager pattern: plain event classes expose public events and public methods that raise them, while gameplay components call those methods instead of event classes subscribing through Unity lifecycle methods.