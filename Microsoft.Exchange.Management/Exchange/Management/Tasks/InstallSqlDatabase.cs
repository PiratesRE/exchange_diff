using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "sqldatabase")]
	[LocDescription(Strings.IDs.InstallCentralAdminServiceTask)]
	public class InstallSqlDatabase : ManageSqlDatabase
	{
		[Parameter(Mandatory = false)]
		public string DatabasePath
		{
			get
			{
				return (string)base.Fields["DatabasePath"];
			}
			set
			{
				base.Fields["DatabasePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TransactionLogPath
		{
			get
			{
				return (string)base.Fields["TransactionLogPath"];
			}
			set
			{
				base.Fields["TransactionLogPath"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install(this.DatabasePath, this.TransactionLogPath);
			TaskLogger.LogExit();
		}
	}
}
