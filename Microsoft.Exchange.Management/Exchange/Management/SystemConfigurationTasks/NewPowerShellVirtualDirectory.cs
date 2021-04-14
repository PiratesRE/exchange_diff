using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "PowerShellVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewPowerShellVirtualDirectory : NewPowerShellCommonVirtualDirectory<ADPowerShellVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewPowerShellVirtualDirectory(base.Name, base.Server.ToString());
			}
		}

		protected override string VirtualDirectoryPath
		{
			get
			{
				if (base.Role == VirtualDirectoryRole.ClientAccess)
				{
					return "FrontEnd\\HttpProxy\\PowerShell";
				}
				return "ClientAccess\\PowerShell";
			}
		}

		protected override string DefaultApplicationPoolId
		{
			get
			{
				if (base.Role == VirtualDirectoryRole.ClientAccess)
				{
					return "MSExchangePowerShellFrontEndAppPool";
				}
				return "MSExchangePowerShellAppPool";
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSSL
		{
			get
			{
				return base.Fields["RequireSSL"] == null || (bool)base.Fields["RequireSSL"];
			}
			set
			{
				base.Fields["RequireSSL"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADPowerShellVirtualDirectory adpowerShellVirtualDirectory = (ADPowerShellVirtualDirectory)base.PrepareDataObject();
			adpowerShellVirtualDirectory.VirtualDirectoryType = PowerShellVirtualDirectoryType.PowerShell;
			return adpowerShellVirtualDirectory;
		}

		protected override void SetDefaultAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory)
		{
			virtualDirectory.BasicAuthentication = new bool?(true);
			virtualDirectory.WindowsAuthentication = new bool?(false);
			virtualDirectory.DigestAuthentication = new bool?(false);
			virtualDirectory.LiveIdBasicAuthentication = new bool?(false);
			virtualDirectory.LiveIdNegotiateAuthentication = new bool?(false);
			virtualDirectory.WSSecurityAuthentication = new bool?(false);
		}

		protected override bool VerifyRoleConsistency()
		{
			if (base.Role == VirtualDirectoryRole.ClientAccess && !base.OwningServer.IsCafeServer)
			{
				base.WriteError(new ArgumentException("Argument: -Role ClientAccess"), ErrorCategory.InvalidArgument, null);
				return false;
			}
			if (base.Role == VirtualDirectoryRole.Mailbox && !base.OwningServer.IsHubTransportServer && !base.OwningServer.IsMailboxServer && !base.OwningServer.IsUnifiedMessagingServer && !base.OwningServer.IsFrontendTransportServer && !base.OwningServer.IsFfoWebServiceRole && !base.OwningServer.IsOSPRole && !base.OwningServer.IsClientAccessServer)
			{
				base.WriteError(new ArgumentException("Argument: -Role Mailbox"), ErrorCategory.InvalidArgument, null);
				return false;
			}
			return true;
		}

		protected override bool ShouldCreateVirtualDirectory()
		{
			base.ShouldCreateVirtualDirectory();
			return this.VerifyRoleConsistency();
		}

		protected override void InternalProcessComplete()
		{
			base.InternalProcessComplete();
			this.DataObject.RequireSSL = new bool?(true);
			if (base.Fields["RequireSSL"] != null)
			{
				ExchangeServiceVDirHelper.SetSSLRequired(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.ErrorUpdatingVDir(this.DataObject.MetabasePath, string.Empty), (bool)base.Fields["RequireSSL"]);
			}
		}

		private const string PowerShellFrontEndVDirPath = "FrontEnd\\HttpProxy\\PowerShell";

		private const string PowerShellBackEndVDirPath = "ClientAccess\\PowerShell";

		private const string PowerShellFrontEndDefaultAppPoolId = "MSExchangePowerShellFrontEndAppPool";

		private const string PowerShellBackEndDefaultAppPoolId = "MSExchangePowerShellAppPool";
	}
}
