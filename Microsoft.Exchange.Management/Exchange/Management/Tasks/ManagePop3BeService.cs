using System;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class ManagePop3BeService : ManagePopImapService
	{
		protected ManagePop3BeService()
		{
			base.AddFirewallRule(new MSExchangePOP3BeFirewallRule());
		}

		protected override string Name
		{
			get
			{
				return "MSExchangePOP3BE";
			}
		}

		protected override string ServiceDisplayName
		{
			get
			{
				return Strings.Pop3BeServiceDisplayName;
			}
		}

		protected override string ServiceDescription
		{
			get
			{
				return Strings.Pop3BeServiceDescription;
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
				return "MSExchange POP3 backend service";
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
				return "ClientAccess\\PopImap";
			}
		}
	}
}
