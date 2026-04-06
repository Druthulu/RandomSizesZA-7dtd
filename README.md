# RandomSizesZA

Game mod that randomizes entity scale at spawn with full multiplayer network synchronization.

## Technical Overview

This isn't a simple cosmetic mod — it solves the problem of keeping randomized visual state consistent across a server and all connected clients in a Unity-based multiplayer game.

**Server-Authoritative RNG** — The server generates all random scale values and acts as the single source of truth. Clients never generate their own values, preventing desync.

**Custom Network Packets** — Built custom MonoBehaviour-based network packages to transmit scale data from server to all connected clients. Handles late-joining clients, entity despawn/respawn, and connection interruptions.

**Callback Architecture** — Network sync uses callback patterns to ensure scale is applied only after the client confirms receipt and the entity is fully loaded in the scene.

**Harmony Patching** — Hooks into the game's entity spawn pipeline via Harmony to intercept and modify entities at the correct lifecycle point.

**Configuration** — XML-based config for per-entity-type min/max scale ranges, feature toggles, and debug logging.

## Compatibility

- Single player, non-dedicated servers, and dedicated servers
- Null reference protection and error recovery
- Verbose debug mode for diagnosing issues across game version updates

## Config
```xml
<randomZombieSizes>true</randomZombieSizes>
<randomAnimalSizes>true</randomAnimalSizes>
<zombieMin>0.75</zombieMin>
<zombieMax>1.25</zombieMax>
<animalMin>0.75</animalMin>
<animalMax>1.25</animalMax>
<debugMode>false</debugMode>
```

## Tech Stack

C#, Harmony, Unity MonoBehaviour, Custom Network Packets, XML Configuration, Server-Authoritative Architecture
