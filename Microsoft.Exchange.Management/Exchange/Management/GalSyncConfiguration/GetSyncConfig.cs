using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.GalSyncConfiguration
{
	[Cmdlet("Get", "SyncConfig")]
	public sealed class GetSyncConfig : GetMultitenancySingletonSystemConfigurationObjectTask<SyncOrganization>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			AcceptedDomain acceptedDomain = null;
			SyncOrganization syncOrganization = (SyncOrganization)dataObject;
			SmtpDomain federatedNamespace = null;
			SmtpDomainWithSubdomains provisioningDomain = null;
			if (syncOrganization.FederatedTenant)
			{
				acceptedDomain = this.ResolveDefaultAcceptedDomain();
				federatedNamespace = acceptedDomain.DomainName.SmtpDomain;
			}
			if (syncOrganization.ProvisioningDomain == null)
			{
				if (syncOrganization.FederatedTenant)
				{
					provisioningDomain = acceptedDomain.DomainName;
				}
				else
				{
					acceptedDomain = this.ResolveDefaultAcceptedDomain();
					provisioningDomain = acceptedDomain.DomainName;
				}
			}
			base.WriteResult(new SyncConfig((SyncOrganization)dataObject, federatedNamespace, provisioningDomain));
			TaskLogger.LogExit();
		}

		private AcceptedDomain ResolveDefaultAcceptedDomain()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			AcceptedDomain defaultAcceptedDomain = configurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorNoDefaultAcceptedDomainFound(configurationSession.SessionSettings.CurrentOrganizationId.ConfigurationUnit.ToString())), (ErrorCategory)1001, null);
			}
			return defaultAcceptedDomain;
		}
	}
}
