using NetDissector.Interfaces;
using PacketDotNet;
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
        DestinationMac = new byte[EthernetFields.MacAddressLength];
        SourceMac = new byte[EthernetFields.MacAddressLength];
        EtherType = 0;
        Payload = Array.Empty<byte>();
    }

    public EthernetFrame(ReadOnlySpan<byte> rawData, int offset=0)
    {
        Parse(rawData, offset);
    }

    #region interface IPacket
    public void Parse(ReadOnlySpan<byte> rawData, int offset = 0)
    {
        // Проверяем границы
        if (rawData.Length - offset < EthernetFields.HeaderLength)
        {
            throw new ArgumentException("Длина меньше чем 14 байт минимальной длны Ethernet II (DIX) frame");
        }
        if ((rawData[EthernetFields.EthernetTypePosition] << 8 |
            rawData[EthernetFields.EthernetTypePosition + 1]) < 0x0600)
        { throw new ArgumentException("Данный массив байт не является Ethernet II (DIX) frame"); }

        DestinationMac = rawData.Slice(offset, EthernetFields.MacAddressLength).ToArray();
        SourceMac = rawData.Slice(offset + EthernetFields.SourceMacPosition, EthernetFields.MacAddressLength).ToArray();
        EtherType = (ushort)(rawData[EthernetFields.EthernetTypePosition] << 8 | rawData[EthernetFields.EthernetTypePosition + 1]);
        if (rawData.Length - offset >= EthernetFields.HeaderLength)
        {
            Payload = new byte[rawData.Length - offset - EthernetFields.HeaderLength];
            Payload = rawData.Slice(EthernetFields.HeaderLength, Payload.Length).ToArray();
        }
        else Payload = null;

    }

    public byte[] Serialize()
    {
        if (DestinationMac == null || DestinationMac.Length != 6)
        {
            throw new ArgumentException("Destination MAC должен быть 6 байт длинной");
        }
        if (SourceMac == null || SourceMac.Length != EthernetFields.MacAddressLength)
        {
            throw new ArgumentException("Source MAC должен быть 6 байт длинной");
        }
        int frameLength = EthernetFields.HeaderLength + Payload.Length;

        byte[] frame = new byte[frameLength];
        Span<byte> frameSpan = frame;

        Span<byte> destinationMac = new Span<byte>(DestinationMac);
        destinationMac.CopyTo(frameSpan.Slice(EthernetFields.DestinationMacPosition, destinationMac.Length));

        Span<byte> sourceMac = new Span<byte>(SourceMac);
        sourceMac.CopyTo(frameSpan.Slice(EthernetFields.SourceMacPosition, EthernetFields.MacAddressLength));

        byte[] etherTypeBytes = BitConverter.GetBytes(EtherType);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(etherTypeBytes);
        }
        Span<byte> etherTypeSpan = new Span<byte>(etherTypeBytes);
        etherTypeSpan.CopyTo(frameSpan.Slice(EthernetFields.EthernetTypePosition, EthernetFields.EthernetTypeLength));

        Span<byte> payloadSpan = new Span<byte>(Payload);
        payloadSpan.CopyTo(frameSpan.Slice(EthernetFields.HeaderLength));

        return frame;

    }
    #endregion
}
