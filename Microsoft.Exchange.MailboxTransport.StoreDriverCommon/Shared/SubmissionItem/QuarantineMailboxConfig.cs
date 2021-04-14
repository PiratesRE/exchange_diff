using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem
{
	internal sealed class QuarantineMailboxConfig
	{
		private QuarantineMailboxConfig()
		{
		}

		internal static QuarantineMailboxConfig Instance
		{
			get
			{
				if (QuarantineMailboxConfig.instance == null)
				{
					lock (QuarantineMailboxConfig.syncRoot)
					{
						if (QuarantineMailboxConfig.instance == null)
						{
							QuarantineMailboxConfig.instance = new QuarantineMailboxConfig();
						}
					}
				}
				return QuarantineMailboxConfig.instance;
			}
		}

		internal bool IsUserQuarantineMailbox(string userAddress, string qMailbox, OrganizationId orgId)
		{
			SmtpAddress recipientAddress = new SmtpAddress(qMailbox);
			if (!recipientAddress.IsValidAddress)
			{
				return false;
			}
			bool result;
			try
			{
				bool flag = false;
				if (!qMailbox.Equals(this.quarantineMailbox, StringComparison.OrdinalIgnoreCase))
				{
					this.quarantineMailbox = qMailbox;
					flag = true;
				}
				if (flag || this.quarantineMailboxProxies == null)
				{
					this.quarantineMailboxProxies = QuarantineMailboxConfig.LookupProxyAddresses(recipientAddress, orgId);
				}
				if (this.quarantineMailboxProxies == null)
				{
					TraceHelper.StoreDriverTracer.TraceFail(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "Failed to load quarantine mailbox proxies");
					result = false;
				}
				else
				{
					result = QuarantineMailboxConfig.IsUserAmongProxies(userAddress, this.quarantineMailboxProxies);
				}
			}
			catch (QuarantineMailboxConfigurationLoadException ex)
			{
				TraceHelper.StoreDriverTracer.TraceFail<string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "Failed to load quarantine mailbox proxies: {0}", ex.Message);
				result = false;
			}
			return result;
		}

		private static bool IsUserAmongProxies(string userAddress, string[] proxies)
		{
			foreach (string text in proxies)
			{
				if (text.Equals(userAddress, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static string[] LookupProxyAddresses(SmtpAddress recipientAddress, OrganizationId orgId)
		{
			string addressString = recipientAddress.ToString();
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 180, "LookupProxyAddresses", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\Shared\\SubmissionItem\\QuarantineMailboxConfig.cs");
			ProxyAddress proxyAddress = ProxyAddress.Parse(ProxyAddressPrefix.Smtp.PrimaryPrefix, addressString);
			Exception ex = null;
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			list.Add(ADRecipientSchema.EmailAddresses);
			list.Add(ADRecipientSchema.LegacyExchangeDN);
			ADRawEntry adrawEntry = null;
			try
			{
				adrawEntry = tenantOrRootOrgRecipientSession.FindByProxyAddress(proxyAddress, list);
			}
			catch (TransientException ex2)
			{
				ex = ex2;
			}
			catch (DataValidationException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new QuarantineMailboxConfigurationLoadException(ex.Message);
			}
			if (adrawEntry == null)
			{
				return null;
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)adrawEntry[ADRecipientSchema.EmailAddresses];
			string[] array = new string[proxyAddressCollection.Count + 1];
			string defaultDomainName = Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName;
			string text = (string)adrawEntry[ADRecipientSchema.LegacyExchangeDN];
			SmtpProxyAddress smtpProxyAddress;
			if (!SmtpProxyAddress.TryEncapsulate(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, text, defaultDomainName, out smtpProxyAddress))
			{
				throw new QuarantineMailboxConfigurationLoadException(string.Format("Could not encapsulate legacyExchangeDN: {0}", text));
			}
			array[0] = smtpProxyAddress.AddressString;
			int num = 1;
			foreach (ProxyAddress proxyAddress2 in proxyAddressCollection)
			{
				SmtpProxyAddress smtpProxyAddress2 = null;
				if (proxyAddress2.Prefix == ProxyAddressPrefix.Smtp)
				{
					array[num++] = proxyAddress2.AddressString;
				}
				else
				{
					if (!SmtpProxyAddress.TryEncapsulate(proxyAddress2, defaultDomainName, out smtpProxyAddress2))
					{
						throw new QuarantineMailboxConfigurationLoadException(string.Format("Could not encapsulate this proxy-address: {0}", proxyAddress2.ProxyAddressString));
					}
					array[num++] = smtpProxyAddress2.AddressString;
				}
			}
			return array;
		}

		private static readonly Trace diag = ExTraceGlobals.MapiStoreDriverSubmissionTracer;

		private static object syncRoot = new object();

		private static QuarantineMailboxConfig instance;

		private string quarantineMailbox;

		private string[] quarantineMailboxProxies;
	}
}
