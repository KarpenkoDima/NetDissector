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
    public void Parse_ValidByteArray_ShouldReturnCorrectProperties()
    {
        // Arrange
        byte[] rawData = _validRawFrame;

        // Act
        EthernetFrame ethernetFrame = new EthernetFrame();
        ethernetFrame.Parse(rawData);

        // Assert
        ethernetFrame.DestinationMac.Should().Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        ethernetFrame.SourceMac.Should().Equal(new byte[] { 0x00, 0x0C, 0x29, 0x48, 0x8A, 0x2B });
        ethernetFrame.EtherType.Should().Be(0x0800);
        ethernetFrame.Payload.Should().StartWith(new byte[] { 0x45, 0x00 });
    }

    [Fact]
    public void Serialize_ValidPacket_ShouldMatchExpectedBytes()
    {
        // Arrange
        var packet = new EthernetFrame(_validRawFrame);

        // Act
        byte[] result = packet.Serialize();

        // Assert
        result.Should().BeEquivalentTo(_validRawFrame);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x01 })] // Слишком короткий массив
    public void Parse_InvalidLength_ShouldThrowException(byte[] invalidData)
    {
        // Act & Assert       
        EthernetFrame ethernetFrame = new EthernetFrame();
        Assert.Throws<ArgumentException>(() => ethernetFrame.Parse(invalidData));
    }
}
