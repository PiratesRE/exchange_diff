using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authentication.Mailbox;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class OfflineOrgIdAuth
	{
		internal static DomainConfig GetHRDEntryFromAD(ITenantConfigurationSession session, DomainConfig domainConfig, string domain, out string errorMsg)
		{
			AcceptedDomain acceptedDomain = null;
			string text = null;
			errorMsg = null;
			if (domainConfig != null && !string.IsNullOrEmpty(domainConfig.DomainName))
			{
				acceptedDomain = session.GetAcceptedDomainByDomainName(domainConfig.DomainName);
				OfflineOrgIdAuth.counters.NumberOfAdRequestForOfflineOrgId.Increment();
				OfflineOrgIdAuth.counters.NumberOfADHrdRequests.Increment();
				text = domainConfig.DomainName;
			}
			int num = 0;
			while (acceptedDomain == null && num < AuthServiceStaticConfig.Config.OfflineHrdMaxParentDomainRetry)
			{
				acceptedDomain = session.GetAcceptedDomainByDomainName(domain);
				OfflineOrgIdAuth.counters.NumberOfAdRequestForOfflineOrgId.Increment();
				OfflineOrgIdAuth.counters.NumberOfADHrdRequests.Increment();
				text = domain;
				num++;
				if (acceptedDomain != null)
				{
					break;
				}
				string[] array = domain.Split(OfflineOrgIdAuth.domainNameSpliter, 2);
				if (array.Length != 2)
				{
					break;
				}
				domain = array[1];
			}
			if (acceptedDomain == null)
			{
				errorMsg = "OfflineHrd:cannot find accepted domain " + domain;
				return null;
			}
			LiveIdInstanceType value = acceptedDomain.LiveIdInstanceType.Value;
			bool flag = acceptedDomain.AuthenticationType == AuthenticationType.Federated;
			string text2 = acceptedDomain[AcceptedDomainSchema.HomeRealmRecord].ToString();
			if (string.IsNullOrEmpty(text2))
			{
				if (!flag)
				{
					return new DomainConfig(text, value, flag, null, true, LivePreferredProtocol.Unknown)
					{
						OrgId = (OrganizationId)acceptedDomain[ADObjectSchema.OrganizationId]
					};
				}
				errorMsg = "OfflineHrd:cannot find ADFS for a federated domain " + domain;
				return null;
			}
			else
			{
				string[] array2 = text2.Split(new char[]
				{
					':'
				}, 2);
				string text3 = null;
				if (array2.Length == 2)
				{
					text3 = array2[1];
				}
				if (flag && !text3.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
				{
					errorMsg = string.Format("OfflineHrd:ADFS {0} for domain {1} is not secure.", text3, domain);
					return null;
				}
				int protocol;
				if (!int.TryParse(array2[0], out protocol))
				{
					OfflineOrgIdAuth.eventLogger.LogEvent(SecurityEventLogConstants.Tuple_ADHRDCorrupted, "GetHRDEntryFromAD", new object[]
					{
						text
					});
					return new DomainConfig(text, value, flag, text3, true, LivePreferredProtocol.Unknown)
					{
						OrgId = (OrganizationId)acceptedDomain[ADObjectSchema.OrganizationId]
					};
				}
				return new DomainConfig(text, value, flag, text3, true, (LivePreferredProtocol)protocol)
				{
					OrgId = (OrganizationId)acceptedDomain[ADObjectSchema.OrganizationId]
				};
			}
		}

		internal static void UpdateHRDEntryInAD(ITenantConfigurationSession session, DomainConfig domainConfig, string userDomain, out string iisLog)
		{
			AcceptedDomain acceptedDomain = null;
			iisLog = null;
			if (!string.IsNullOrEmpty(domainConfig.DomainName))
			{
				acceptedDomain = session.GetAcceptedDomainByDomainName(domainConfig.DomainName);
				OfflineOrgIdAuth.counters.NumberOfAdRequestForOfflineOrgId.Increment();
				OfflineOrgIdAuth.counters.NumberOfADHrdRequests.Increment();
			}
			if (acceptedDomain == null)
			{
				ExTraceGlobals.AuthenticationTracer.Information<string, string>((long)domainConfig.GetHashCode(), "Looking up accepted domain for user {0} by user SMTP domain info {1}", userDomain, userDomain);
				acceptedDomain = session.GetAcceptedDomainByDomainName(userDomain);
				OfflineOrgIdAuth.counters.NumberOfAdRequestForOfflineOrgId.Increment();
				OfflineOrgIdAuth.counters.NumberOfADHrdRequests.Increment();
			}
			if (acceptedDomain == null)
			{
				iisLog = string.Format("Failed to look up accepted domain for user {0} by user SMTP domain info {1} or by domain {2} from HRD", userDomain, userDomain, domainConfig.DomainName);
				ExTraceGlobals.AuthenticationTracer.TraceError((long)domainConfig.GetHashCode(), iisLog);
				return;
			}
			string value = (int)domainConfig.PreferredProtocol + ":" + domainConfig.AuthURL;
			acceptedDomain[AcceptedDomainSchema.HomeRealmRecord] = value;
			session.Save(acceptedDomain);
			OfflineOrgIdAuth.counters.NumberOfAdRequestForOfflineOrgId.Increment();
			OfflineOrgIdAuth.counters.NumberOfADHrdRequests.Increment();
		}

		internal static bool CheckPasswordConfidence(string puid, ADObjectId adUserId, int passwordConfidenceInDays, ITenantRecipientSession recipientSession)
		{
			DateTime dateTime = MailboxUserProfile.GetLastLogonTime(puid);
			if (dateTime == DateTime.MinValue)
			{
				ADUser adUser = (ADUser)recipientSession.Read(adUserId);
				string text;
				dateTime = MailboxUserProfile.GetLastLogonTimeFromMailbox(adUser, out text);
			}
			return dateTime + TimeSpan.FromDays((double)passwordConfidenceInDays) >= DateTime.UtcNow;
		}

		private static readonly LiveIdBasicAuthenticationCountersInstance counters = AuthServiceHelper.PerformanceCounters;

		private static readonly ExEventLog eventLogger = AuthServiceHelper.EventLogger;

		private static readonly char[] domainNameSpliter = new char[]
		{
			'.'
		};
	}
}
