using System;
using System.Net;
using Microsoft.Exchange.TextProcessing.Boomerang;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class LocalHost
	{
		public static string Fqdn
		{
			get
			{
				if (LocalHost.fqdn == null)
				{
					string hostName = Dns.GetHostName();
					IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
					LocalHost.fqdn = hostEntry.HostName;
				}
				return LocalHost.fqdn;
			}
		}

		public static string GenerateInternetMessageId(string smtpSenderAddress, Guid organizationId)
		{
			return BoomerangProvider.Instance.GenerateBoomerangMessageId(smtpSenderAddress, LocalHost.Fqdn, organizationId);
		}

		private static string fqdn;
	}
}
