using System;
using System.IO;
using Microsoft.Exchange.Security.WindowsFirewall;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public abstract class UMCallRouterTask : ManageUMService
	{
		protected override string ServiceExeName
		{
			get
			{
				return "Microsoft.Exchange.UM.CallRouter.exe";
			}
		}

		protected override string ServiceShortName
		{
			get
			{
				return "MSExchangeUMCR";
			}
		}

		protected override string ServiceDisplayName
		{
			get
			{
				return Strings.UmCallRouterName;
			}
		}

		protected override string ServiceDescription
		{
			get
			{
				return Strings.UmCallRouterDescription;
			}
		}

		protected override ExchangeFirewallRule FirewallRule
		{
			get
			{
				return new MSExchangeUMCallRouterNumbered();
			}
		}

		protected override string RelativeInstallPath
		{
			get
			{
				return Path.Combine("FrontEnd", "CallRouter");
			}
		}

		private const string FrontEndFolderName = "FrontEnd";

		private const string CallRouterFolderName = "CallRouter";
	}
}
