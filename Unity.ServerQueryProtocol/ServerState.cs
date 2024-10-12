using System.Text;

namespace Unity.ServerQueryProtocol;

public class ServerState
{
  public ushort CurrentPlayers;
  public ushort MaxPlayers;
  public string ServerName = string.Empty;
  public string GameType = string.Empty;
  public string BuildId = string.Empty;
  public string GameMap = string.Empty;
  public ushort Port;

  public ushort Size()
  {
    return (ushort)(sizeof(ushort) + // CurrentPlayers
                    sizeof(ushort) + // MaxPlayers
                    Encoding.UTF8.GetByteCount(ServerName) + 1 +
                    Encoding.UTF8.GetByteCount(GameType) + 1 +
                    Encoding.UTF8.GetByteCount(BuildId) + 1 +
                    Encoding.UTF8.GetByteCount(GameMap) + 1 +
                    sizeof(ushort)); // Port
  }
}