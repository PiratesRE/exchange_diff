using System;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public abstract class UMServiceTask : ManageUMService
	{
		protected override string ServiceExeName
		{
			get
			{
				return "umservice.exe";
			}
		}

		protected override string ServiceShortName
		{
			get
			{
				return "MSExchangeUM";
			}
		}

		protected override string ServiceDisplayName
		{
			get
			{
				return Strings.UmServiceName;
			}
		}

		protected override string ServiceDescription
		{
			get
			{
				return Strings.UmServiceDescription;
			}
		}

		protected override ExchangeFirewallRule FirewallRule
		{
			get
			{
				return new MSExchangeUMServiceNumbered();
			}
		}

		protected override string RelativeInstallPath
		{
			get
			{
				return "Bin";
			}
		}

		private const string BinFolderName = "Bin";
	}
}
