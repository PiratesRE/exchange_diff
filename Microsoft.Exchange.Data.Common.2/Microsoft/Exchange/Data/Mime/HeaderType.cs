using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal enum HeaderType : byte
	{
		Text,
		AsciiText,
		Date,
		Received,
		ContentType,
		ContentDisposition,
		Address
	}
}
