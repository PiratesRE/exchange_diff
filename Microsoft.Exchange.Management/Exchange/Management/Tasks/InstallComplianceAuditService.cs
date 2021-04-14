using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "ComplianceAuditService")]
	[LocDescription(Strings.IDs.InstallComplianceAuditServiceTask)]
	public class InstallComplianceAuditService : ManageComplianceAuditService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
