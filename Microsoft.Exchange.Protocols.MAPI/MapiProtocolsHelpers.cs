using System;
using System.IO;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal static class MapiProtocolsHelpers
	{
		public static void AssertPropValueIsNotSqlType(object propValue)
		{
		}

		public static byte[] GetUnderlyingBytesFromMemoryStream(MemoryStream memoryStream)
		{
			return memoryStream.GetBuffer();
		}
	}
}
