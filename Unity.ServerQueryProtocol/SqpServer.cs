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

      if (response.Length <= 0)
        continue;

      _udpClient.Send(response, response.Length, receiveResult.RemoteEndPoint);
    }
  }

  public void Stop() =>
    _isRunning = false;
}