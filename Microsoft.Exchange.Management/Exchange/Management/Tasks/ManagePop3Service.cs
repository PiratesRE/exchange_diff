using System;
using System.ServiceProcess;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManagePop3Service : ManagePopImapService
	{
		protected ManagePop3Service()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.AddFirewallRule(new MSExchangePOP3FirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangePOP3";
			}
		}

		protected override string ServiceDisplayName
		{
			get
			{
				return Strings.Pop3ServiceDisplayName;
			}
		}

		protected override string ServiceDescription
		{
			get
			{
				return Strings.Pop3ServiceDescription;
			}
		}

		protected override string ServiceFile
		{
			get
			{
				return "Microsoft.Exchange.Pop3Service.exe";
			}
		}

		protected override string ServiceCategoryName
		{
			get
			{
				return "MSExchange POP3 service";
			}
		}

		protected override string WorkingProcessFile
		{
			get
			{
				return "Microsoft.Exchange.Pop3.exe";
			}
		}

		protected override string WorkingProcessEventMessageFile
		{
			get
			{
				return "Microsoft.Exchange.Pop3.EventLog.dll";
			}
		}

		protected override string RelativeInstallPath
		{
			get
			{
				return "FrontEnd\\PopImap";
			}
		}
	}
}
