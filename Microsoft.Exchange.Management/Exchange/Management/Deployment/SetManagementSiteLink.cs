using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "ManagementSiteLink", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetManagementSiteLink : SetSystemConfigurationObjectTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		protected override IConfigurable PrepareDataObject()
		{
			ADSite localSite = ((ITopologyConfigurationSession)this.ConfigurationSession).GetLocalSite();
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)base.PrepareDataObject();
			exchangeConfigurationUnit.ManagementSiteLink = localSite.Id;
			return exchangeConfigurationUnit;
		}
	}
}
