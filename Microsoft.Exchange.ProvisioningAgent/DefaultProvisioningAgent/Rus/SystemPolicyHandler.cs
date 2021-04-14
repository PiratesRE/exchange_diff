using System;
using System.DirectoryServices;
using System.Globalization;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class SystemPolicyHandler : RusDataHandler
	{
		public SystemPolicyHandler(string configurationDomainController, string recipientDomainController, string globalCatalog, NetworkCredential credential, PartitionId partitionId, UserScope userScope, ProvisioningCache provisioningCache, LogMessageDelegate logger) : base(configurationDomainController, recipientDomainController, globalCatalog, credential, partitionId, provisioningCache, logger)
		{
		}

		public bool UpdateRecipient(ADRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			ExTraceGlobals.RusTracer.TraceDebug<string>((long)this.GetHashCode(), "SystemPolicyHandler.UpdateRecipient: recipient={0}", recipient.Identity.ToString());
			if (string.IsNullOrEmpty(recipient.Alias))
			{
				return false;
			}
			if (base.RecipientDomainController == null)
			{
				base.RecipientSession.DomainController = recipient.OriginatingServer;
			}
			base.CurrentOrganizationId = recipient.OrganizationId;
			ADUser aduser = recipient as ADUser;
			if (aduser != null && aduser.Database != null)
			{
				UserAccountControlFlags userAccountControl = aduser.UserAccountControl;
				UserAccountControlFlags userAccountControlFlags = aduser.ExchangeUserAccountControl;
				userAccountControlFlags &= ~UserAccountControlFlags.AccountDisabled;
				userAccountControlFlags |= (UserAccountControlFlags.AccountDisabled & userAccountControl);
				aduser.ExchangeUserAccountControl = userAccountControlFlags;
				if (aduser.ExchangeGuid == Guid.Empty)
				{
					aduser.ExchangeGuid = Guid.NewGuid();
					if (base.Logger != null)
					{
						base.Logger(Strings.VerboseSettingExchangeGuid(aduser.ExchangeGuid.ToString()));
					}
				}
				if (aduser.ExchangeSecurityDescriptor == null)
				{
					if (SystemPolicyHandler.defaultExchangeSecurityDescriptor == null)
					{
						ActiveDirectoryAccessRule rule = new ActiveDirectoryAccessRule(new SecurityIdentifier(WellKnownSidType.SelfSid, null), ActiveDirectoryRights.ReadControl | ActiveDirectoryRights.CreateChild, AccessControlType.Allow, ActiveDirectorySecurityInheritance.All, Guid.Empty);
						ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
						activeDirectorySecurity.AddAccessRule(rule);
						activeDirectorySecurity.SetOwner(new SecurityIdentifier(WellKnownSidType.SelfSid, null));
						activeDirectorySecurity.SetGroup(new SecurityIdentifier(WellKnownSidType.SelfSid, null));
						SystemPolicyHandler.defaultExchangeSecurityDescriptor = new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
					}
					aduser.ExchangeSecurityDescriptor = SystemPolicyHandler.defaultExchangeSecurityDescriptor;
				}
			}
			if (string.IsNullOrEmpty(recipient.LegacyExchangeDN))
			{
				if (this.administrativeGroupLegDN == null)
				{
					this.administrativeGroupLegDN = base.ProvisioningCache.TryAddAndGetGlobalData<string>(CannedProvisioningCacheKeys.AdministrativeGroupLegDN, () => base.RootOrgConfigurationSession.GetAdministrativeGroup().LegacyExchangeDN);
				}
				string text = string.Format(CultureInfo.InvariantCulture, "{0}/cn=Recipients", new object[]
				{
					this.administrativeGroupLegDN
				});
				int num = 128;
				string text2 = null;
				Organization orgContainer = base.ConfigurationSession.GetOrgContainer();
				if (!orgContainer.IsGuidPrefixedLegacyDnDisabled)
				{
					text2 = string.Format("{0}-{1}", Guid.NewGuid().ToString("N"), recipient.Name);
				}
				if (string.IsNullOrEmpty(text2))
				{
					if (Datacenter.IsMicrosoftHostedOnly(true))
					{
						int minorPartnerId = LocalSiteCache.LocalSite.MinorPartnerId;
						text2 = string.Format("{0}-{1}{2}", minorPartnerId, recipient.Name, Guid.NewGuid().ToString("N").Substring(0, 3));
					}
					else
					{
						text2 = recipient.Name + Guid.NewGuid().ToString("N").Substring(0, 3);
					}
					if (recipient.OrganizationId != OrganizationId.ForestWideOrgId)
					{
						string text3 = recipient.OrganizationId.OrganizationalUnit.Name;
						if (num > 0 && text3.Length > num - text.Length - 12)
						{
							text3 = text3.GetHashCode().ToString(CultureInfo.InvariantCulture);
						}
						text2 = string.Format("{0}-{1}", text3, text2);
					}
				}
				recipient.LegacyExchangeDN = LegacyDN.GenerateLegacyDN(text, num, recipient, true, new LegacyDN.LegacyDNIsUnique(this.LegacyDNIsUnique), text2);
				if (base.Logger != null)
				{
					base.Logger(Strings.VerboseSettingLegacyExchangeDN(recipient.LegacyExchangeDN));
				}
			}
			if (string.IsNullOrEmpty(recipient.DisplayName))
			{
				if (base.Logger != null)
				{
					base.Logger(Strings.VerboseSettingDisplayName(recipient.Name));
				}
				recipient.DisplayName = recipient.Name;
			}
			return true;
		}

		private bool LegacyDNIsUnique(string legacyDN)
		{
			if (!Datacenter.IsMultiTenancyEnabled() && base.GlobalCatalogSession.IsLegacyDNInUse(legacyDN))
			{
				return false;
			}
			if (string.IsNullOrEmpty(base.RecipientSession.DomainController) || !StringComparer.InvariantCultureIgnoreCase.Equals(base.RecipientSession.DomainController, base.GlobalCatalogSession.DomainController))
			{
				ADSessionSettings sessionSettings = Datacenter.IsMultiTenancyEnabled() ? ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(base.CurrentOrganizationId) : ADSessionSettings.FromRootOrgScopeSet();
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.RecipientSession.DomainController, base.RecipientSession.ReadOnly, base.RecipientSession.ConsistencyMode, base.RecipientSession.NetworkCredential, sessionSettings, 260, "LegacyDNIsUnique", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\Rus\\SystemPolicyHandler.cs");
				if (tenantOrRootOrgRecipientSession.IsLegacyDNInUse(legacyDN))
				{
					return false;
				}
			}
			return true;
		}

		private const int lengthReservedForCnPartInLegacyDN = 12;

		private static RawSecurityDescriptor defaultExchangeSecurityDescriptor;

		private string administrativeGroupLegDN;
	}
}
