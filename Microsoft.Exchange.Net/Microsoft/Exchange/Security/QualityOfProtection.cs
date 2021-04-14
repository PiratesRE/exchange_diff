using System;

namespace Microsoft.Exchange.Security
{
	internal enum QualityOfProtection
	{
		None,
		WrapNoEncrypt = -2147483647,
		WrapOutOfBandData = 1073741824
	}
}
