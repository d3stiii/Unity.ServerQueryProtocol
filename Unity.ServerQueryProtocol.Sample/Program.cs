namespace Unity.ServerQueryProtocol.Sample;

public static class Program
{
  public static Task Main(string[] args)
  {
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
    
    Console.WriteLine($"Query server started using SQP on port: {queryPort}");

    return Task.Delay(-1);
  }
}