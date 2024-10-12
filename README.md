# Unity.ServerQueryProtocol

Unity.ServerQueryProtocol is a C#/.NET implementation of [SQP](https://docs.unity.com/ugs/en-us/manual/game-server-hosting/manual/concepts/sqp) (Server Query Protocol) for [Unity Multiplay](https://unity.com/products/game-server-hosting).

Examples
--------

- Create a `ServerState`.
- Create a `SqpServer` by passing queryPort and serverState as arguments.
- Call the `sqpServer.Start()` method to launch the SQP server.

```cs
var serverState = new ServerState
{
  Port = 25565,
  BuildId = "alpha v0.0.1",
  ServerName = "GameServer",
  CurrentPlayers = 2,
  GameMap = "TestMap",
  GameType = "Competitive",
  MaxPlayers = 10
};

const ushort queryPort = 8080;
var sqpServer = new SqpServer(queryPort, serverState);
sqpServer.Start();
```

Documentation
-------------
- [Unity SQP Documentation](https://docs.unity.com/ugs/en-us/manual/game-server-hosting/manual/concepts/sqp).
