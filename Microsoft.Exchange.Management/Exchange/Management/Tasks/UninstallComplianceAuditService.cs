using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.UninstallComplianceAuditServiceTask)]
	[Cmdlet("Uninstall", "ComplianceAuditService")]
	public class UninstallComplianceAuditService : ManageComplianceAuditService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
