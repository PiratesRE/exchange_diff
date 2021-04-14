using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	public static class MapiDispHelper
	{
		public static string GetDnsHostName()
		{
			return ComputerInformation.DnsHostName;
		}

		public static bool IsSupportedMrsVersion(ulong clientVersion)
		{
			return clientVersion >= MapiVersion.MRS14SP1.Value;
		}
	}
}
