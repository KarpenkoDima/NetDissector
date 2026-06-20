# ARCHITECTURAL DOGMAS: NETDISSECTOR REFACTORING & EXPANSION
# TARGET: .NET 9 ZERO-ALLOCATION PARSER

1. CURRENT CODEBASE FLAWS TO ELIMINATE:
   - Destroy `class EthernetFrame` -> Must become `readonly ref struct`.
   - Eradicate heap allocations (`new byte[]`, `.ToArray()`) in `Parse` and `Serialize` methods.
   - Remove exceptions (`ArgumentException`) for control flow in parsing. Use `TryParse` paradigm.
   - Refactor `IPacket` interface. In high-performance C#, `ref struct` cannot efficiently use interfaces without boxing or breaking constraints (prior to advanced C# 13 features). Switch to static `TryParse` methods and direct `Span` manipulation.
   - Replace manual bitwise shifting for Endianness with `System.Buffers.Binary.BinaryPrimitives`.

2. IMPLEMENTATION RULES (L2 to L4):
   - ALL headers (Ethernet, IPv4, TCP) must be parsed by projecting directly from `ReadOnlySpan<byte>` using `Slice`.
   - "Make invalid states unrepresentable": `TryParse` MUST return `false` instantly if `span.Length` is smaller than the required header size.
   - TCP Implementation: Must extract `SourcePort`, `DestinationPort`, `SequenceNumber`, `AcknowledgmentNumber`, `Flags`, and calculate `DataOffset` to correctly slice the remaining payload. Skip TCP Options safely.