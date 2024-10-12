namespace Unity.ServerQueryProtocol.Sample;

public static class Program
{
  public static Task Main(string[] args)
  {
    const ushort queryPort = 8080;

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

    var sqpServer = new SqpServer(queryPort, serverState);
    sqpServer.Start();

    return Task.Delay(-1);
  }
}