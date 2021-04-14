using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "PowerShellVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetPowerShellVirtualDirectory : SetPowerShellCommonVirtualDirectory<ADPowerShellVirtualDirectory>
	{
		internal new bool DigestAuthentication
		{
			get
			{
				return base.DigestAuthentication;
			}
			set
			{
				base.DigestAuthentication = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableSessionKeyRedirectionModule
		{
			get
			{
				return base.Fields["EnableSessionKeyRedirectionModule"] != null && (bool)base.Fields["EnableSessionKeyRedirectionModule"];
			}
			set
			{
				base.Fields["EnableSessionKeyRedirectionModule"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableDelegatedAuthModule
		{
			get
			{
				return base.Fields["EnableDelegatedAuthModule"] != null && (bool)base.Fields["EnableDelegatedAuthModule"];
			}
			set
			{
				base.Fields["EnableDelegatedAuthModule"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireSSL
		{
			get
			{
				return base.Fields["RequireSSL"] != null && (bool)base.Fields["RequireSSL"];
			}
			set
			{
				base.Fields["RequireSSL"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPowerShellVirtualDirectory(this.DataObject.Name, this.DataObject.Server.ToString());
			}
		}

		protected override PowerShellVirtualDirectoryType AllowedVirtualDirectoryType
		{
			get
			{
				return PowerShellVirtualDirectoryType.PowerShell;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			base.InternalEnableLiveIdNegotiateAuxiliaryModule();
			if (base.Fields["EnableSessionKeyRedirectionModule"] != null)
			{
				base.SetSessionKeyRedirectionModule(this.EnableSessionKeyRedirectionModule, false);
			}
			if (base.Fields["EnableDelegatedAuthModule"] != null)
			{
				base.SetDelegatedAuthenticationModule(this.EnableDelegatedAuthModule, false);
				base.SetPowerShellRequestFilterModule(this.EnableDelegatedAuthModule, false);
			}
			if (base.Fields["RequireSSL"] != null)
			{
				ExchangeServiceVDirHelper.SetSSLRequired(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.ErrorUpdatingVDir(this.DataObject.MetabasePath, string.Empty), (bool)base.Fields["RequireSSL"]);
			}
		}
	}
}
