using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "OrganizationalUnit", SupportsShouldProcess = true)]
	public sealed class UninstallOrganizationalUnitTask : RemoveSystemConfigurationObjectTask<OrganizationalUnitIdParameter, ADOrganizationalUnit>
	{
		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.CreateSession();
			configurationSession.UseConfigNC = false;
			return configurationSession;
		}

		protected override void InternalProcessRecord()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			configurationSession.DeleteTree(base.DataObject, delegate(ADTreeDeleteNotFinishedException de)
			{
				if (de != null)
				{
					base.WriteVerbose(de.LocalizedString);
				}
			});
		}
	}
}
