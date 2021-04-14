using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "ComplianceService")]
	[LocDescription(Strings.IDs.InstallComplianceServiceTask)]
	public sealed class InstallComplianceService : ManageComplianceService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
