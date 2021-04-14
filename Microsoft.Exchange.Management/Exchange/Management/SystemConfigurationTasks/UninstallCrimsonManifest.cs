using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Uninstall", "CrimsonManifest")]
	public class UninstallCrimsonManifest : Task
	{
		[Parameter(Mandatory = true)]
		public string DefinitionXml
		{
			get
			{
				return (string)base.Fields["DefinitionXml"];
			}
			set
			{
				base.Fields["DefinitionXml"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string manifestName = Path.Combine(ConfigurationContext.Setup.InstallPath, this.DefinitionXml);
			try
			{
				ManageEventManifest.Uninstall(manifestName);
			}
			catch (InvalidOperationException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}
	}
}
