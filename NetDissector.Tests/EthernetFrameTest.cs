using FluentAssertions;
using NetDissector.Protocols;
namespace NetDissector.Tests;

public class EthernetFrameTest
{
    private readonly byte[] _validRawFrame = new byte[]
    {
        0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // Destination MAC
        0x00, 0x0C, 0x29, 0x48, 0x8A, 0x2B, // Source MAC
        0x08, 0x00,                         // EtherType (IPv4)
        0x45, 0x00, 0x00, 0x54              // Payload (IP header snippet)
    };

    [Fact]
    public void TryParse_ValidByteArray_ShouldReturnCorrectProperties()
    {
        // Act
        bool success = EthernetFrame.TryParse(_validRawFrame, out EthernetFrame frame);

        // Assert
        success.Should().BeTrue();
        frame.DestinationMac.ToArray().Should().Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        frame.SourceMac.ToArray().Should().Equal(new byte[] { 0x00, 0x0C, 0x29, 0x48, 0x8A, 0x2B });
        frame.EtherType.Should().Be(0x0800);
        frame.Payload.ToArray().Should().StartWith(new byte[] { 0x45, 0x00 });
    }

    [Fact]
    public void TrySerialize_ValidPacket_ShouldMatchExpectedBytes()
    {
        // Arrange
        EthernetFrame.TryParse(_validRawFrame, out EthernetFrame frame);
        Span<byte> destination = new byte[_validRawFrame.Length];

        // Act
        bool success = frame.TrySerialize(destination, out int bytesWritten);

        // Assert
        success.Should().BeTrue();
        bytesWritten.Should().Be(_validRawFrame.Length);
        destination.ToArray().Should().BeEquivalentTo(_validRawFrame);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x01 })] // Слишком короткий массив
    public void TryParse_InvalidLength_ShouldReturnFalse(byte[] invalidData)
    {
        // Act
        bool success = EthernetFrame.TryParse(invalidData, out EthernetFrame frame);

        // Assert
        success.Should().BeFalse();
    }
}
