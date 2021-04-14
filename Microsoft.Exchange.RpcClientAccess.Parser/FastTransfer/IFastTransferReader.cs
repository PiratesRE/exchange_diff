using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IFastTransferReader : IFastTransferDataInterface, IDisposable
	{
		bool IsDataAvailable { get; }

		bool TryPeekMarker(out PropertyTag propertyTag);

		void ReadMarker(PropertyTag expectedMarker);

		PropertyTag ReadPropertyInfo(out NamedProperty namedProperty, out int codePage);

		PropertyValue ReadAndParseFixedSizeValue(PropertyTag propertyTag);

		int ReadLength(int maxValue);

		ArraySegment<byte> ReadVariableSizeValue(int maxToRead);
	}
}
