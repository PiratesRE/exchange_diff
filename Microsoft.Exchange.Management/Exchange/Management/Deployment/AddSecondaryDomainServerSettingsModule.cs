using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class AddSecondaryDomainServerSettingsModule : RunspaceServerSettingsInitModule
	{
		public AddSecondaryDomainServerSettingsModule(TaskContext context) : base(context)
		{
		}

		protected override ADServerSettings GetCmdletADServerSettings()
		{
			PropertyBag fields = base.CurrentTaskContext.InvocationInfo.Fields;
			SwitchParameter switchParameter = fields.Contains("IsDatacenter") ? ((SwitchParameter)fields["IsDatacenter"]) : new SwitchParameter(false);
			bool flag = fields.Contains("DomainController");
			OrganizationIdParameter organizationIdParameter = (OrganizationIdParameter)fields["PrimaryOrganization"];
			PartitionId partitionId = (organizationIdParameter != null) ? ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(organizationIdParameter.RawIdentity) : null;
			string value = null;
			ADServerSettings serverSettings = ExchangePropertyContainer.GetServerSettings(base.CurrentTaskContext.SessionState);
			if (serverSettings != null && partitionId != null)
			{
				value = serverSettings.PreferredGlobalCatalog(partitionId.ForestFQDN);
			}
			if (switchParameter && organizationIdParameter != null && string.IsNullOrEmpty(value) && partitionId != null && !flag)
			{
				if (this.domainBasedADServerSettings == null)
				{
					this.domainBasedADServerSettings = RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(organizationIdParameter.RawIdentity.ToLowerInvariant(), partitionId.ForestFQDN, false);
				}
				return this.domainBasedADServerSettings;
			}
			return base.GetCmdletADServerSettings();
		}

		private ADServerSettings domainBasedADServerSettings;
	}
}
