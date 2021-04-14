using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "PswsVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewPswsVirtualDirectory : NewPowerShellCommonVirtualDirectory<ADPswsVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewPswsVirtualDirectory(base.Name, base.Server.ToString());
			}
		}

		protected override string VirtualDirectoryPath
		{
			get
			{
				if (base.Role == VirtualDirectoryRole.ClientAccess)
				{
					return "FrontEnd\\HttpProxy\\Psws";
				}
				return "ClientAccess\\Psws";
			}
		}

		protected override string DefaultApplicationPoolId
		{
			get
			{
				if (base.Role == VirtualDirectoryRole.ClientAccess)
				{
					return "MSExchangePswsFrontEndAppPool";
				}
				return "MSExchangePswsAppPool";
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADPswsVirtualDirectory adpswsVirtualDirectory = (ADPswsVirtualDirectory)base.PrepareDataObject();
			adpswsVirtualDirectory.VirtualDirectoryType = PowerShellVirtualDirectoryType.Psws;
			return adpswsVirtualDirectory;
		}

		protected override void SetDefaultAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory)
		{
			virtualDirectory.BasicAuthentication = new bool?(false);
			virtualDirectory.WindowsAuthentication = new bool?(false);
		}

		protected override bool ShouldCreateVirtualDirectory()
		{
			bool flag = base.ShouldCreateVirtualDirectory();
			if (!flag && base.OwningServer.IsMailboxServer)
			{
				flag = true;
			}
			return flag;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(this.DataObject))
			{
				ExchangeServiceVDirHelper.ForceAnonymous(this.DataObject.MetabasePath);
			}
		}

		private const string PswsFrontEndVDirPath = "FrontEnd\\HttpProxy\\Psws";

		private const string PswsBackEndVDirPath = "ClientAccess\\Psws";

		private const string PswsFrontEndDefaultAppPoolId = "MSExchangePswsFrontEndAppPool";

		private const string PswsBackEndDefaultAppPoolId = "MSExchangePswsAppPool";
	}
}
