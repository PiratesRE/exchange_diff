using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class ContentFilterRegistration : Task
	{
		internal ContentFilterRegistration(bool register)
		{
			this.register = register;
		}

		protected override void InternalProcessRecord()
		{
			Process process = new Process();
			process.StartInfo.FileName = Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\agents\\Hygiene\\Microsoft.Exchange.ContentFilter.Wrapper.exe");
			process.StartInfo.Arguments = (this.register ? "-regserver" : "-unregserver");
			LocalizedString localizedString = this.register ? Strings.RegisterFilterFailed : Strings.UnregisterFilterFailed;
			try
			{
				process.Start();
				if (!process.WaitForExit(60000))
				{
					process.Kill();
					process.Close();
					base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidOperation, null);
				}
			}
			catch (Win32Exception innerException)
			{
				base.ThrowTerminatingError(new LocalizedException(localizedString, innerException), ErrorCategory.InvalidOperation, null);
			}
			finally
			{
				process.Close();
			}
		}

		private readonly bool register;
	}
}
