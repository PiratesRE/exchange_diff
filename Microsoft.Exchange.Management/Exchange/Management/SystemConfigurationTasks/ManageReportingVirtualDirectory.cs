using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class ManageReportingVirtualDirectory : Task
	{
		protected const string ReportingVDirName = "Reporting";

		protected const string ReportingVDirPath = "ClientAccess\\Reporting";

		protected const string ReportingDefaultAppPoolId = "MSExchangeReportingAppPool";
	}
}
