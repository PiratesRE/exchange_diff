using System;

namespace Microsoft.Exchange.Data.Mime
{
	public enum EncodingScheme : byte
	{
		None,
		Rfc2047,
		Rfc2231,
		Jis,
		EightBit
	}
}
