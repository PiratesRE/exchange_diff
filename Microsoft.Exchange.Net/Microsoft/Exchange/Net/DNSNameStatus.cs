using System;

namespace Microsoft.Exchange.Net
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
