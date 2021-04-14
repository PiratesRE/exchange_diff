using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "TeanantObjectVersion", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetTenantObjectVersion : SetSystemConfigurationObjectTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		protected override IConfigurable PrepareDataObject()
		{
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)base.PrepareDataObject();
			exchangeConfigurationUnit.ObjectVersion = Organization.OrgConfigurationVersion;
			exchangeConfigurationUnit.SetBuildVersion(OrganizationTaskHelper.ManagementDllVersion.FileBuildPart, OrganizationTaskHelper.ManagementDllVersion.FilePrivatePart);
			return exchangeConfigurationUnit;
		}
	}
}
