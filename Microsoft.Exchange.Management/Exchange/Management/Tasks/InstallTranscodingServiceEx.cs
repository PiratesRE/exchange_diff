using System;
using System.ComponentModel;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "TranscodingServiceEx")]
	[LocDescription(Strings.IDs.InstallTranscodingServiceEx)]
	public class InstallTranscodingServiceEx : Task
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ComRunAsPwdUtil.SetRunAsPassword(InstallTranscodingServiceEx.TranscodingServiceAppId, "");
			}
			catch (Win32Exception e)
			{
				base.WriteError(new TaskWin32Exception(e), ErrorCategory.WriteError, null);
			}
			TaskLogger.LogExit();
		}

		internal static string TranscodingServiceAppId = "{E1803C22-3538-4DCB-83A7-853214C9FF20}";
	}
}
