using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum StreamPropertyType : short
	{
		Null = 1,
		Bool = 2,
		Byte = 3,
		SByte = 4,
		Int16 = 5,
		UInt16 = 6,
		Int32 = 7,
		UInt32 = 8,
		Int64 = 9,
		UInt64 = 10,
		Single = 11,
		Double = 12,
		Decimal = 13,
		Char = 14,
		String = 15,
		DateTime = 16,
		Guid = 17,
		IPAddress = 18,
		IPEndPoint = 19,
		RoutingAddress = 20,
		ADObjectId = 21,
		RecipientType = 22,
		ADObjectIdUTF8 = 23,
		ADObjectIdWithString = 24,
		ProxyAddress = 25,
		Array = 4096,
		List = 8192,
		MultiValuedProperty = 16384
	}
}
