using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "PswsVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetPswsVirtualDirectory : SetPowerShellCommonVirtualDirectory<ADPswsVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetPswsVirtualDirectory(this.DataObject.Name, this.DataObject.Server.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public bool OAuthAuthentication
		{
			get
			{
				return base.Fields["OAuthAuthentication"] != null && (bool)base.Fields["OAuthAuthentication"];
			}
			set
			{
				base.Fields["OAuthAuthentication"] = value;
			}
		}

		protected override PowerShellVirtualDirectoryType AllowedVirtualDirectoryType
		{
			get
			{
				return PowerShellVirtualDirectoryType.Psws;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADPswsVirtualDirectory adpswsVirtualDirectory = (ADPswsVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			adpswsVirtualDirectory.OAuthAuthentication = (bool?)base.Fields["OAuthAuthentication"];
			return adpswsVirtualDirectory;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(this.DataObject))
			{
				ExchangeServiceVDirHelper.ForceAnonymous(this.DataObject.MetabasePath);
			}
		}
	}
}
