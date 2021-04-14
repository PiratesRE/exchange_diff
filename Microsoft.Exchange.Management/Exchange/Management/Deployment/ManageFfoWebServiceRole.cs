using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageFfoWebServiceRole : ManageRole
	{
		protected override void InternalBeginProcessing()
		{
			this.ResolveDomainController();
			base.InternalBeginProcessing();
			base.ExecuteScript("Add-PSSnapin -Name Microsoft.Exchange.Management.PowerShell.Setup", true, 0, LocalizedString.Empty);
		}

		private void ResolveDomainController()
		{
			if (base.DomainController == null)
			{
				base.DomainController = new Fqdn(DirectoryUtilities.PickLocalDomainController().DnsHostName);
			}
			ManageFfoWebServiceRole.SetPreferredDC(base.DomainController.Domain);
		}

		private static void SetPreferredDC(string domainController)
		{
			string dnsHostName = DirectoryUtilities.PickGlobalCatalog(domainController).DnsHostName;
			SetupServerSettings setupServerSettings = SetupServerSettings.CreateSetupServerSettings();
			setupServerSettings.SetConfigurationDomainController(TopologyProvider.LocalForestFqdn, new Fqdn(domainController));
			setupServerSettings.SetPreferredGlobalCatalog(TopologyProvider.LocalForestFqdn, new Fqdn(dnsHostName));
			setupServerSettings.AddPreferredDC(new Fqdn(domainController));
			ADSessionSettings.SetProcessADContext(new ADDriverContext(setupServerSettings, ContextMode.Setup));
		}
	}
}
