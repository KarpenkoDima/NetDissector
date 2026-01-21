using NetDissector.Interfaces;
using System.Net.Http.Headers;

namespace NetDissector.Protocols;

/// <summary>
/// Класс представляющий Ethernet II (DIX) frame
/// </summary>
public class EthernetFrame : IPacket
{
    public byte[] DestinationMac { get; set; }
    public byte[] SourceMac { get; set; }
    public ushort EtherType { get; set; }
    public byte[]? Payload { get; set; }

    public EthernetFrame()
    {
        
    }

    public EthernetFrame(ReadOnlySpan<byte> rawData, int offset=0)
    {
        Parse(rawData, offset);
    }

    #region interface IPacket
    public void Parse(ReadOnlySpan<byte> rawData, int offset = 0)
    {
        if (rawData.Length - offset < 14)
        {
            throw new ArgumentException("Длина меньше чем 14 байт минимальной длны Ethernet II (DIX) frame");
        }
        if ((rawData[12] << 8 | rawData[13]) < 0x0600)
        { throw new ArgumentException("Данный массивбайт не является Ethernet II (DIX) frame"); }

        DestinationMac = rawData.Slice(offset, 6).ToArray();
        SourceMac = rawData.Slice(offset + 6, 6).ToArray();
        EtherType = (ushort)(rawData[12] << 8 | rawData[13]);
        if (rawData.Length - offset >= 14)
        {
            Payload = new byte[rawData.Length - offset - 14];
            Payload = rawData.Slice(14, Payload.Length).ToArray();
        }
        else Payload = null;

    }

    public byte[] Serialize()
    {
        if (DestinationMac == null || DestinationMac.Length != 6)
        {
            throw new ArgumentException("Destination MAC должен быть 6 байт длинной");
        }
        if (SourceMac == null || SourceMac.Length != 6)
        {
            throw new ArgumentException("Source MAC должен быть 6 байт длинной");
        }
        int frameLength = DestinationMac.Length + SourceMac.Length + 2 + Payload.Length;

        byte[] frame = new byte[frameLength];
        Span<byte> frameSpan = frame;

        Span<byte> destinationMac = new Span<byte>(DestinationMac);
        destinationMac.CopyTo(frameSpan.Slice(0, destinationMac.Length));

        Span<byte> sourceMac = new Span<byte>(SourceMac);
        sourceMac.CopyTo(frameSpan.Slice(DestinationMac.Length, sourceMac.Length));

        byte[] etherTypeBytes = BitConverter.GetBytes(EtherType);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(etherTypeBytes);
        }
        Span<byte> etherTypeSpan = new Span<byte>(etherTypeBytes);
        etherTypeSpan.CopyTo(frameSpan.Slice(DestinationMac.Length + SourceMac.Length, 2));

        Span<byte> payloadSpan = new Span<byte>(Payload);
        payloadSpan.CopyTo(frameSpan.Slice(DestinationMac.Length + SourceMac.Length + 2));

        return frame;

    }
    #endregion
}
