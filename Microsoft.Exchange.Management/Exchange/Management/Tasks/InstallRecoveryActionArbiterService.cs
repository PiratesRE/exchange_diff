using System;
using System.Management.Automation;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.InstallRecoveryActionArbiterServiceTask)]
	[Cmdlet("Install", "RecoveryActionArbiterService")]
	public class InstallRecoveryActionArbiterService : ManageRecoveryActionArbiterService
	{
		[Parameter(Mandatory = false)]
		public string UserName
		{
			get
			{
				return (string)base.Fields["UserName"];
			}
			set
			{
				base.Fields["UserName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Domain
		{
			get
			{
				return (string)base.Fields["Domain"];
			}
			set
			{
				base.Fields["Domain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Password
		{
			get
			{
				return (string)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (!string.IsNullOrEmpty(this.UserName))
			{
				base.Account = ServiceAccount.User;
				base.ServiceInstallContext.Parameters["Username"] = string.Format("{0}\\{1}", this.Domain, this.UserName);
				base.ServiceInstallContext.Parameters["Password"] = this.Password;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
