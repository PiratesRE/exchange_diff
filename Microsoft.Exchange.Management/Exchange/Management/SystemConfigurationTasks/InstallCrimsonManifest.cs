using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "CrimsonManifest")]
	public class InstallCrimsonManifest : Task
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

		[Parameter(Mandatory = true)]
		public string MessageDll
		{
			get
			{
				return (string)base.Fields["MessageDll"];
			}
			set
			{
				base.Fields["MessageDll"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ProviderName
		{
			get
			{
				return (string)base.Fields["ProviderName"];
			}
			set
			{
				base.Fields["ProviderName"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string text = Path.Combine(ConfigurationContext.Setup.InstallPath, this.DefinitionXml);
			string text2 = Path.Combine(ConfigurationContext.Setup.InstallPath, this.MessageDll);
			try
			{
				if (ManageEventManifest.UpdateMessageDllPath(text, text2, this.ProviderName))
				{
					ManageEventManifest.Install(text);
				}
				else
				{
					base.WriteError(new InvalidOperationException(Strings.EventManifestNotUpdated(text, text2, this.ProviderName)), ErrorCategory.InvalidOperation, null);
				}
			}
			catch (InvalidOperationException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}
	}
}
