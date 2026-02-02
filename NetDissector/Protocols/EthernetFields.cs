using System.Runtime.InteropServices;

namespace NetDissector.Protocols;

/// <summary>
/// Константы полей Ethernet фрейма
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct EthernetFields
{
    public static readonly int DestinationMacPosition;

    public static readonly int HeaderLength;

    public static readonly int MacAddressLength;

    public static readonly int SourceMacPosition;

    public static readonly int EthernetTypeLength;

    public static readonly int EthernetTypePosition;

    static EthernetFields()
    {
        DestinationMacPosition = 0;
        MacAddressLength = 6;
        EthernetTypeLength = 2;
        SourceMacPosition = MacAddressLength;
        EthernetTypePosition = MacAddressLength * 2;
        HeaderLength = EthernetTypePosition + EthernetTypeLength;
    }
}
