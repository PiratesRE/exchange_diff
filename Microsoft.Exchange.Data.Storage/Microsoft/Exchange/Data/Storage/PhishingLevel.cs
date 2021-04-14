using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum PhishingLevel
	{
		Pass = 1,
		Neutral,
		SoftFail,
		Suspicious1,
		Suspicious2,
		Suspicious3,
		Suspicious4
	}
}
