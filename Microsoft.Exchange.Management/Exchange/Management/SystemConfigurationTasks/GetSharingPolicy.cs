using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "SharingPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetSharingPolicy : GetMultitenancySystemConfigurationObjectTask<SharingPolicyIdParameter, SharingPolicy>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			SharingPolicy sharingPolicy = (SharingPolicy)dataObject;
			sharingPolicy.Default = sharingPolicy.Id.Equals(this.FederatedOrganizationId.DefaultSharingPolicyLink);
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		private FederatedOrganizationId FederatedOrganizationId
		{
			get
			{
				if (this.federatedOrganizationId == null)
				{
					IConfigurationSession configurationSession;
					if (base.SharedConfiguration != null)
					{
						configurationSession = SharedConfiguration.CreateScopedToSharedConfigADSession(base.CurrentOrganizationId);
					}
					else
					{
						configurationSession = this.ConfigurationSession;
						configurationSession.SessionSettings.IsSharedConfigChecked = true;
					}
					this.federatedOrganizationId = configurationSession.GetFederatedOrganizationId(configurationSession.SessionSettings.CurrentOrganizationId);
				}
				return this.federatedOrganizationId;
			}
		}

		private FederatedOrganizationId federatedOrganizationId;
	}
}
