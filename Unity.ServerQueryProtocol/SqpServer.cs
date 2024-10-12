using System.Net;
using System.Net.Sockets;

namespace Unity.ServerQueryProtocol;

public class SqpServer
{
  private readonly UdpClient _udpClient;
  private readonly QueryHandler _handler;
  private readonly ushort _port;
  private bool _isRunning;

  public SqpServer(ushort port, ServerState serverState)
  {
    _port = port;
    _udpClient = new UdpClient();
    _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
    _udpClient.Client.SendTimeout = 1000;

    _handler = new QueryHandler(serverState);
  }

  public void Start()
  {
    Console.WriteLine($"Starting query server using SQP on {_port}");

    _isRunning = true;
    new Thread(UpdateLoop).Start();
  }

  private void UpdateLoop()
  {
    while (_isRunning)
    {
      var receiveResult = _udpClient.ReceiveAsync().Result;
      var buffer = receiveResult.Buffer;

      var response = _handler.Handle(receiveResult.RemoteEndPoint.ToString(), buffer);

      if (response == null || response.Length <= 0)
      {
        Console.WriteLine("Query response error");
        continue;
      }

      try
      {
        _udpClient.Send(response, response.Length, receiveResult.RemoteEndPoint);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Writing response error " + ex.Message);
      }
    }
  }

  public void Stop() =>
    _isRunning = false;
}