using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "AddressRewriteConfigContainer")]
	public sealed class InstallAddressRewriteConfigContainerTask : InstallContainerTaskBase<AddressRewriteConfigContainer>
	{
		protected override ADObjectId GetBaseContainer()
		{
			return new ADObjectId("OU=MSExchangeGateway");
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.CreateSession();
			configurationSession.UseConfigNC = false;
			return configurationSession;
		}
	}
}
