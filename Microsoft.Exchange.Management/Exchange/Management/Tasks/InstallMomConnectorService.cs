using System;
using System.Management.Automation;
using System.Security;
using System.ServiceProcess;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "MomConnectorService")]
	[LocDescription(Strings.IDs.InstallMomConnectorServiceTask)]
	public class InstallMomConnectorService : ManageMomConnectorService
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
		public SecureString Password
		{
			get
			{
				return (SecureString)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!string.IsNullOrEmpty(this.UserName))
			{
				base.Account = ServiceAccount.User;
				base.ServiceInstallContext.Parameters["Username"] = string.Format("{0}\\{1}", this.Domain, this.UserName);
				if (this.Password != null)
				{
					base.ServiceInstallContext.Parameters["Password"] = this.Password.ConvertToUnsecureString();
				}
			}
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
