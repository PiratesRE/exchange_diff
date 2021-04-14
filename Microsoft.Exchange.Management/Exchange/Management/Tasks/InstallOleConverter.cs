using System;
using System.ComponentModel;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.ComAdminInstallOleConverter)]
	[Cmdlet("Install", "OleConverter")]
	public class InstallOleConverter : Task
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				OleConverterRegistry.RegisterOleConverter();
			}
			catch (Win32Exception e)
			{
				base.WriteError(new TaskWin32Exception(e), ErrorCategory.WriteError, null);
			}
			catch (SecurityException exception)
			{
				base.WriteError(exception, ErrorCategory.WriteError, null);
			}
			TaskLogger.LogExit();
		}
	}
}
