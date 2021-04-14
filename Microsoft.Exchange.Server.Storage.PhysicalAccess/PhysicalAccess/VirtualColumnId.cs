using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public enum VirtualColumnId
	{
		PageNumber = 1,
		DataSize,
		LongValueDataSize,
		OverheadSize,
		LongValueOverheadSize,
		NonTaggedColumnCount,
		TaggedColumnCount,
		LongValueCount,
		MultiValueCount,
		CompressedColumnCount,
		CompressedDataSize,
		CompressedLongValueDataSize
	}
}
