using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class LegacyDnHelper
	{
		public static string ConvertToLegacyDn(string address, OrganizationId organizationId, bool forcePrimaryLegacyDn)
		{
			if (address.StartsWith("/o=", StringComparison.InvariantCultureIgnoreCase))
			{
				return address;
			}
			if (address.StartsWith("EX:", StringComparison.InvariantCultureIgnoreCase))
			{
				return address.Substring("EX:".Length).Trim();
			}
			ADSessionSettings adSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
			ExchangePrincipal exchangePrincipal = null;
			if (address.StartsWith("SMTP:", StringComparison.InvariantCultureIgnoreCase))
			{
				Guid empty = Guid.Empty;
				string address2 = address.Substring("SMTP:".Length).Trim();
				if (SmtpAddress.IsValidSmtpAddress(address2) && SmtpProxyAddress.TryDeencapsulateExchangeGuid(address2, out empty))
				{
					exchangePrincipal = ExchangePrincipal.FromMailboxGuid(adSettings, empty, null);
				}
			}
			if (exchangePrincipal == null)
			{
				exchangePrincipal = ExchangePrincipal.FromProxyAddress(adSettings, address);
			}
			if (!forcePrimaryLegacyDn && exchangePrincipal.MailboxInfo.IsArchive)
			{
				return exchangePrincipal.LegacyDn + "/guid=" + exchangePrincipal.MailboxInfo.MailboxGuid.ToString();
			}
			return exchangePrincipal.LegacyDn;
		}

		public static string GetDomainAndLegacyDnFromAddress(string address, out string legacyDn)
		{
			legacyDn = null;
			if (address.StartsWith("DomDn:", StringComparison.InvariantCultureIgnoreCase))
			{
				string text = address.Substring("DomDn:".Length).Trim();
				int num = text.IndexOf(";");
				int num2 = text.Length - (num + 1);
				if (num > 0 && num2 > 0)
				{
					legacyDn = text.Substring(num + 1, num2);
					string text2 = text.Substring(0, num);
					if (SmtpAddress.IsValidDomain(text2))
					{
						return text2;
					}
				}
			}
			return null;
		}

		public static bool IsFederatedSystemAttendant(string legacyDn)
		{
			return !string.IsNullOrEmpty(legacyDn) && string.Compare(legacyDn, 0, "*/cn=Microsoft Federated System Attendant", 0, "*/cn=Microsoft Federated System Attendant".Length, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public static bool IsValidClientFederatedSystemAttendant(string legacyDn)
		{
			return LegacyDnHelper.IsFederatedSystemAttendant(legacyDn) && legacyDn.Length == "*/cn=Microsoft Federated System Attendant".Length;
		}

		private const string LegacyDnPrefix = "/o=";

		private const string ExchangeLegacyDnPrefix = "EX:";

		private const string SmtpPrefix = "SMTP:";

		private const string FederatedSystemAttendantLegacyDn = "*/cn=Microsoft Federated System Attendant";

		public const string DomainAndLegacyDnPrefix = "DomDn:";
	}
}
