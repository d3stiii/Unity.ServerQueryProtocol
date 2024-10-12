using System.Buffers.Binary;
using System.Text;

namespace Unity.ServerQueryProtocol;

public class BigEndianEncoder
{
  public void Write(BinaryWriter writer, byte value) => 
    writer.Write(value);

  public void Write(BinaryWriter writer, ushort value)
  {
    byte[] buffer = new byte[2];
    BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
    writer.Write(buffer);
  }

  public void Write(BinaryWriter writer, uint value)
  {
    byte[] buffer = new byte[4];
    BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
    writer.Write(buffer);
  }

  public void Write(BinaryWriter writer, ulong value)
  {
    byte[] buffer = new byte[8];
    BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
    writer.Write(buffer);
  }

  public void Write(BinaryWriter writer, string value)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(value);

    if (bytes.Length > 255)
      throw new ArgumentException("String is too long. Maximum length is 255 bytes.");

    writer.Write((byte)bytes.Length);
    writer.Write(bytes);
  }
}