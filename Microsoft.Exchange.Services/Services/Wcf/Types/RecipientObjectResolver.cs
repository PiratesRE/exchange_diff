using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal class RecipientObjectResolver
	{
		private RecipientObjectResolver()
		{
		}

		internal static RecipientObjectResolver Instance { get; set; } = new RecipientObjectResolver();

		private ADSessionSettings TenantSessionSetting
		{
			get
			{
				return ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), CallContext.Current.AccessingPrincipal.MailboxInfo.OrganizationId, null, false);
			}
		}

		internal IEnumerable<ADRecipient> ResolveLegacyDNs(IEnumerable<string> legacyDNs)
		{
			if (legacyDNs != null && legacyDNs.Any<string>())
			{
				IRecipientSession recipientSession = (IRecipientSession)this.CreateAdSession();
				return from recipient in recipientSession.FindADRecipientsByLegacyExchangeDNs(legacyDNs.ToArray<string>())
				where recipient.Data != null
				select recipient.Data;
			}
			return null;
		}

		internal IEnumerable<ADRecipient> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			IRecipientSession session = (IRecipientSession)this.CreateAdSession();
			return from e in identities
			select session.Read(e);
		}

		internal IEnumerable<ADRecipient> ResolveSmtpAddress(IEnumerable<string> addresses)
		{
			if (addresses != null && addresses.Any<string>())
			{
				return from recipient in this.ResolveProxyAddresses(from address in addresses
				select ProxyAddress.Parse(address))
				where recipient != null
				select recipient;
			}
			return null;
		}

		private IEnumerable<ADRecipient> ResolveProxyAddresses(IEnumerable<ProxyAddress> proxyAddresses)
		{
			if (proxyAddresses != null && proxyAddresses.Any<ProxyAddress>())
			{
				IRecipientSession recipientSession = (IRecipientSession)this.CreateAdSession();
				return from recipient in recipientSession.FindByProxyAddresses(proxyAddresses.ToArray<ProxyAddress>())
				select recipient.Data;
			}
			return null;
		}

		private IDirectorySession CreateAdSession()
		{
			IDirectorySession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, this.TenantSessionSetting, 158, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\services\\src\\Services\\jsonservice\\types\\RecipientObjectResolver.cs");
			tenantOrRootOrgRecipientSession.SessionSettings.IncludeInactiveMailbox = true;
			return tenantOrRootOrgRecipientSession;
		}
	}
}
