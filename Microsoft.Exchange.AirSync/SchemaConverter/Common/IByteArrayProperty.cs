using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IByteArrayProperty : IProperty
	{
		byte[] ByteArrayData { get; }
	}
}
