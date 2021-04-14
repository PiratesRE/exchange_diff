using System;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManageImap4BeService : ManagePopImapService
	{
		protected ManageImap4BeService()
		{
			base.AddFirewallRule(new MSExchangeIMAP4BeFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangeIMAP4BE";
			}
		}

		protected override string ServiceDisplayName
		{
			get
			{
				return Strings.Imap4BeServiceDisplayName;
			}
		}

		protected override string ServiceDescription
		{
			get
			{
				return Strings.Imap4BeServiceDescription;
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
				return "MSExchange IMAP4 backend service";
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
				return "ClientAccess\\PopImap";
			}
		}
	}
}
