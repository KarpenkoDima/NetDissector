using System.Buffers.Binary;

namespace NetDissector.Protocols;

/// <summary>
/// Ethernet II (DIX) frame. Zero-allocation view over a byte buffer.
/// </summary>
public readonly ref struct EthernetFrame
{
    public ReadOnlySpan<byte> DestinationMac { get; }
    public ReadOnlySpan<byte> SourceMac { get; }
    public ushort EtherType { get; }
    public ReadOnlySpan<byte> Payload { get; }

    private EthernetFrame(ReadOnlySpan<byte> destinationMac, ReadOnlySpan<byte> sourceMac, ushort etherType, ReadOnlySpan<byte> payload)
    {
        DestinationMac = destinationMac;
        SourceMac = sourceMac;
        EtherType = etherType;
        Payload = payload;
    }

    public static bool TryParse(ReadOnlySpan<byte> rawData, out EthernetFrame frame)
    {
        frame = default;

        if (rawData.Length < EthernetFields.HeaderLength)
        {
            return false;
        }

        ushort etherType = BinaryPrimitives.ReadUInt16BigEndian(rawData.Slice(EthernetFields.EthernetTypePosition, EthernetFields.EthernetTypeLength));
        if (etherType < 0x0600)
        {
            return false;
        }

        ReadOnlySpan<byte> destinationMac = rawData.Slice(EthernetFields.DestinationMacPosition, EthernetFields.MacAddressLength);
        ReadOnlySpan<byte> sourceMac = rawData.Slice(EthernetFields.SourceMacPosition, EthernetFields.MacAddressLength);
        ReadOnlySpan<byte> payload = rawData.Slice(EthernetFields.HeaderLength);

        frame = new EthernetFrame(destinationMac, sourceMac, etherType, payload);
        return true;
    }

    public bool TrySerialize(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;

        if (DestinationMac.Length != EthernetFields.MacAddressLength || SourceMac.Length != EthernetFields.MacAddressLength)
        {
            return false;
        }

        int frameLength = EthernetFields.HeaderLength + Payload.Length;
        if (destination.Length < frameLength)
        {
            return false;
        }

        DestinationMac.CopyTo(destination.Slice(EthernetFields.DestinationMacPosition, EthernetFields.MacAddressLength));
        SourceMac.CopyTo(destination.Slice(EthernetFields.SourceMacPosition, EthernetFields.MacAddressLength));
        BinaryPrimitives.WriteUInt16BigEndian(destination.Slice(EthernetFields.EthernetTypePosition, EthernetFields.EthernetTypeLength), EtherType);
        Payload.CopyTo(destination.Slice(EthernetFields.HeaderLength));

        bytesWritten = frameLength;
        return true;
    }
}
