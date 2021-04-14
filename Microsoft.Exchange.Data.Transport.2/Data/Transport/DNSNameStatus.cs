using System;

namespace Microsoft.Exchange.Data.Transport
{
	internal enum DNSNameStatus : uint
	{
		Valid,
		InvalidCharacter = 9560U,
		NumericName,
		InvalidName = 123U,
		NonRFCName = 9556U
	}
}
