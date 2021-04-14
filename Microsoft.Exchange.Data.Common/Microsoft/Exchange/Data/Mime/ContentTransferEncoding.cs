using System;

namespace Microsoft.Exchange.Data.Mime
{
	public enum ContentTransferEncoding
	{
		SevenBit = 1,
		EightBit,
		Binary,
		QuotedPrintable,
		Base64,
		UUEncode,
		BinHex,
		Unknown = 0
	}
}
