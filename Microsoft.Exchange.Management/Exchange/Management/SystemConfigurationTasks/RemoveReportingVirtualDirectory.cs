using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "ReportingVirtualDirectory", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveReportingVirtualDirectory : ManageReportingVirtualDirectory
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveReportingVirtualDirectory;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			DeleteVirtualDirectory deleteVirtualDirectory = new DeleteVirtualDirectory();
			deleteVirtualDirectory.Name = "Reporting";
			deleteVirtualDirectory.Parent = "IIS://localhost/W3SVC/1/Root";
			deleteVirtualDirectory.Initialize();
			deleteVirtualDirectory.Execute();
			TaskLogger.LogExit();
		}
	}
}
