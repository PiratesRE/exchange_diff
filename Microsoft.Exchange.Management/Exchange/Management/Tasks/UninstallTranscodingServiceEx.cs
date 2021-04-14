using System;
using System.ComponentModel;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Uninstall", "TranscodingServiceEx")]
	[LocDescription(Strings.IDs.UninstallTranscodingServiceEx)]
	public class UninstallTranscodingServiceEx : Task
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ComRunAsPwdUtil.SetRunAsPassword(InstallTranscodingServiceEx.TranscodingServiceAppId, null);
			}
			catch (Win32Exception e)
			{
				base.WriteError(new TaskWin32Exception(e), ErrorCategory.WriteError, null);
			}
			TaskLogger.LogExit();
		}
	}
}
