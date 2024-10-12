using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Text;

namespace Unity.ServerQueryProtocol;

public class QueryHandler
{
  private readonly ConcurrentDictionary<string, uint> _challenges = new();
  private readonly BigEndianEncoder _encoder = new();
  private readonly ServerState _state;

  public QueryHandler(ServerState state) =>
    _state = state;

  public byte[] Handle(string clientAddress, byte[] buffer)
  {
    if (IsChallenge(buffer))
      return HandleChallenge(clientAddress);

    if (IsQuery(buffer))
      return HandleQuery(clientAddress, buffer);

    throw new InvalidOperationException("Unsupported query");
  }

  private bool IsChallenge(byte[] buffer) =>
    buffer.Take(5).SequenceEqual(new byte[] { 0, 0, 0, 0, 0 });

  private bool IsQuery(byte[] buffer) =>
    buffer[0] == 1;

  private byte[] HandleChallenge(string clientAddress)
  {
    var challengeValue = (uint)new Random().Next(1, int.MaxValue);
    _challenges[clientAddress] = challengeValue;

    using var memoryStream = new MemoryStream();
    using (var writer = new BinaryWriter(memoryStream))
    {
      const byte challengeResponseType = 0;
      _encoder.Write(writer, challengeResponseType);
      _encoder.Write(writer, challengeValue);
    }

    return memoryStream.ToArray();
  }

  private byte[] HandleQuery(string clientAddress, byte[] buffer)
  {
    if (!_challenges.TryRemove(clientAddress, out var expectedChallenge))
      throw new InvalidOperationException("No challenge");

    if (buffer.Length < 8)
      throw new InvalidOperationException("Packet not long enough");

    if (BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[1..5]) != expectedChallenge)
      throw new InvalidOperationException("Challenge mismatch");

    var protocolVersion = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[5..7]);
    if (protocolVersion != 1)
      throw new InvalidOperationException($"Unsupported SQP version: {protocolVersion}");

    var requestedChunks = buffer[7];
    var requestedServerInfo = (requestedChunks & 0x1) > 0;

    using var memoryStream = new MemoryStream();
    using (var writer = new BinaryWriter(memoryStream, Encoding.BigEndianUnicode))
    {
      const byte queryResponseType = 1;
      const byte currentPacket = 0;
      const byte lastPacket = 0;
      ushort packetLength = (ushort)(sizeof(uint) + _state.Size());

      _encoder.Write(writer, queryResponseType);
      _encoder.Write(writer, expectedChallenge);
      _encoder.Write(writer, protocolVersion);
      _encoder.Write(writer, currentPacket);
      _encoder.Write(writer, lastPacket);
      _encoder.Write(writer, requestedServerInfo ? packetLength : (ushort)0);

      if (requestedServerInfo)
      {
        uint chunkLength = _state.Size();
        _encoder.Write(writer, chunkLength);
        _encoder.Write(writer, _state.CurrentPlayers);
        _encoder.Write(writer, _state.MaxPlayers);
        _encoder.Write(writer, _state.ServerName);
        _encoder.Write(writer, _state.GameType);
        _encoder.Write(writer, _state.BuildId);
        _encoder.Write(writer, _state.GameMap);
        _encoder.Write(writer, _state.Port);
      }
    }

    return memoryStream.ToArray();
  }
}