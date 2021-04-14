using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallCentralAdminServiceTask)]
	[Cmdlet("uninstall", "sqldatabase")]
	public class UninstallSqlDatabase : ManageSqlDatabase
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Uninstall();
			TaskLogger.LogExit();
		}
	}
}
