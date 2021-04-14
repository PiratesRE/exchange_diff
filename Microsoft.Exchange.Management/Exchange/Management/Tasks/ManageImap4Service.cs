using System;
using System.ServiceProcess;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageImap4Service : ManagePopImapService
	{
		protected ManageImap4Service()
		{
			base.Account = ServiceAccount.LocalSystem;
			base.AddFirewallRule(new MSExchangeIMAP4FirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeIMAP4";
			}
		}

		protected override string ServiceDisplayName
		{
			get
			{
				return Strings.Imap4ServiceDisplayName;
			}
		}

		protected override string ServiceDescription
		{
			get
			{
				return Strings.Imap4ServiceDescription;
			}
		}

		protected override string ServiceFile
		{
			get
			{
				return "Microsoft.Exchange.Imap4Service.exe";
			}
		}

		protected override string ServiceCategoryName
		{
			get
			{
				return "MSExchange IMAP4 service";
			}
		}

		protected override string WorkingProcessFile
		{
			get
			{
				return "Microsoft.Exchange.Imap4.exe";
			}
		}

		protected override string WorkingProcessEventMessageFile
		{
			get
			{
				return "Microsoft.Exchange.Imap4.EventLog.dll";
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
