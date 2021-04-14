using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MailboxLocationExtensions
	{
		public static bool IsLocal(this IMailboxLocation mailboxLocation)
		{
			string serverFqdn = mailboxLocation.ServerFqdn;
			return string.Equals(serverFqdn, ComputerInformation.DnsFullyQualifiedDomainName, StringComparison.OrdinalIgnoreCase) || string.Equals(serverFqdn, ComputerInformation.DnsHostName, StringComparison.OrdinalIgnoreCase) || string.Equals(serverFqdn, "localhost", StringComparison.OrdinalIgnoreCase) || string.Equals(serverFqdn, Environment.MachineName, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsLegacyServer(this IMailboxLocation mailboxLocation)
		{
			int num = 0;
			if (mailboxLocation != null)
			{
				num = mailboxLocation.ServerVersion;
			}
			return num <= Server.E2007MinVersion;
		}
	}
}
