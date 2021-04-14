using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WellKnownUserAgent
	{
		public static string GetEwsNegoAuthUserAgent(string value)
		{
			return "ExchangeInternalEwsClient-" + value;
		}

		public const string EwsNegoAuthUserAgentPrefix = "ExchangeInternalEwsClient-";
	}
}
