using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetHostedEncryptionVirtualDirectoryCommand : SyntheticCommandWithPipelineInputNoOutput<ADE4eVirtualDirectory>
	{
		private SetHostedEncryptionVirtualDirectoryCommand() : base("Set-HostedEncryptionVirtualDirectory")
		{
		}

		public SetHostedEncryptionVirtualDirectoryCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetHostedEncryptionVirtualDirectoryCommand SetParameters(SetHostedEncryptionVirtualDirectoryCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetHostedEncryptionVirtualDirectoryCommand SetParameters(SetHostedEncryptionVirtualDirectoryCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<string> AllowedFileTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedFileTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveFileTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveMimeTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveMimeTypes"] = value;
				}
			}

			public virtual bool? AlwaysShowBcc
			{
				set
				{
					base.PowerSharpParameters["AlwaysShowBcc"] = value;
				}
			}

			public virtual bool? CheckForForgottenAttachments
			{
				set
				{
					base.PowerSharpParameters["CheckForForgottenAttachments"] = value;
				}
			}

			public virtual bool? HideMailTipsByDefault
			{
				set
				{
					base.PowerSharpParameters["HideMailTipsByDefault"] = value;
				}
			}

			public virtual uint? MailTipsLargeAudienceThreshold
			{
				set
				{
					base.PowerSharpParameters["MailTipsLargeAudienceThreshold"] = value;
				}
			}

			public virtual int? MaxRecipientsPerMessage
			{
				set
				{
					base.PowerSharpParameters["MaxRecipientsPerMessage"] = value;
				}
			}

			public virtual int? MaxMessageSizeInKb
			{
				set
				{
					base.PowerSharpParameters["MaxMessageSizeInKb"] = value;
				}
			}

			public virtual string ComposeFontColor
			{
				set
				{
					base.PowerSharpParameters["ComposeFontColor"] = value;
				}
			}

			public virtual string ComposeFontName
			{
				set
				{
					base.PowerSharpParameters["ComposeFontName"] = value;
				}
			}

			public virtual int? ComposeFontSize
			{
				set
				{
					base.PowerSharpParameters["ComposeFontSize"] = value;
				}
			}

			public virtual int? MaxImageSizeKB
			{
				set
				{
					base.PowerSharpParameters["MaxImageSizeKB"] = value;
				}
			}

			public virtual int? MaxAttachmentSizeKB
			{
				set
				{
					base.PowerSharpParameters["MaxAttachmentSizeKB"] = value;
				}
			}

			public virtual int? MaxEncryptedContentSizeKB
			{
				set
				{
					base.PowerSharpParameters["MaxEncryptedContentSizeKB"] = value;
				}
			}

			public virtual int? MaxEmailStringSize
			{
				set
				{
					base.PowerSharpParameters["MaxEmailStringSize"] = value;
				}
			}

			public virtual int? MaxPortalStringSize
			{
				set
				{
					base.PowerSharpParameters["MaxPortalStringSize"] = value;
				}
			}

			public virtual int? MaxFwdAllowed
			{
				set
				{
					base.PowerSharpParameters["MaxFwdAllowed"] = value;
				}
			}

			public virtual int? PortalInactivityTimeout
			{
				set
				{
					base.PowerSharpParameters["PortalInactivityTimeout"] = value;
				}
			}

			public virtual int? TDSTimeOut
			{
				set
				{
					base.PowerSharpParameters["TDSTimeOut"] = value;
				}
			}

			public virtual bool BasicAuthentication
			{
				set
				{
					base.PowerSharpParameters["BasicAuthentication"] = value;
				}
			}

			public virtual bool WindowsAuthentication
			{
				set
				{
					base.PowerSharpParameters["WindowsAuthentication"] = value;
				}
			}

			public virtual bool LiveIdAuthentication
			{
				set
				{
					base.PowerSharpParameters["LiveIdAuthentication"] = value;
				}
			}

			public virtual GzipLevel GzipLevel
			{
				set
				{
					base.PowerSharpParameters["GzipLevel"] = value;
				}
			}

			public virtual ExtendedProtectionTokenCheckingMode ExtendedProtectionTokenChecking
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionTokenChecking"] = value;
				}
			}

			public virtual MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlags
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionFlags"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExtendedProtectionSPNList
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionSPNList"] = value;
				}
			}

			public virtual Uri InternalUrl
			{
				set
				{
					base.PowerSharpParameters["InternalUrl"] = value;
				}
			}

			public virtual Uri ExternalUrl
			{
				set
				{
					base.PowerSharpParameters["ExternalUrl"] = value;
				}
			}

			public virtual MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
			{
				set
				{
					base.PowerSharpParameters["ExternalAuthenticationMethods"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual VirtualDirectoryIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedFileTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AllowedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["AllowedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedFileTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> BlockedMimeTypes
			{
				set
				{
					base.PowerSharpParameters["BlockedMimeTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveFileTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveFileTypes"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ForceSaveMimeTypes
			{
				set
				{
					base.PowerSharpParameters["ForceSaveMimeTypes"] = value;
				}
			}

			public virtual bool? AlwaysShowBcc
			{
				set
				{
					base.PowerSharpParameters["AlwaysShowBcc"] = value;
				}
			}

			public virtual bool? CheckForForgottenAttachments
			{
				set
				{
					base.PowerSharpParameters["CheckForForgottenAttachments"] = value;
				}
			}

			public virtual bool? HideMailTipsByDefault
			{
				set
				{
					base.PowerSharpParameters["HideMailTipsByDefault"] = value;
				}
			}

			public virtual uint? MailTipsLargeAudienceThreshold
			{
				set
				{
					base.PowerSharpParameters["MailTipsLargeAudienceThreshold"] = value;
				}
			}

			public virtual int? MaxRecipientsPerMessage
			{
				set
				{
					base.PowerSharpParameters["MaxRecipientsPerMessage"] = value;
				}
			}

			public virtual int? MaxMessageSizeInKb
			{
				set
				{
					base.PowerSharpParameters["MaxMessageSizeInKb"] = value;
				}
			}

			public virtual string ComposeFontColor
			{
				set
				{
					base.PowerSharpParameters["ComposeFontColor"] = value;
				}
			}

			public virtual string ComposeFontName
			{
				set
				{
					base.PowerSharpParameters["ComposeFontName"] = value;
				}
			}

			public virtual int? ComposeFontSize
			{
				set
				{
					base.PowerSharpParameters["ComposeFontSize"] = value;
				}
			}

			public virtual int? MaxImageSizeKB
			{
				set
				{
					base.PowerSharpParameters["MaxImageSizeKB"] = value;
				}
			}

			public virtual int? MaxAttachmentSizeKB
			{
				set
				{
					base.PowerSharpParameters["MaxAttachmentSizeKB"] = value;
				}
			}

			public virtual int? MaxEncryptedContentSizeKB
			{
				set
				{
					base.PowerSharpParameters["MaxEncryptedContentSizeKB"] = value;
				}
			}

			public virtual int? MaxEmailStringSize
			{
				set
				{
					base.PowerSharpParameters["MaxEmailStringSize"] = value;
				}
			}

			public virtual int? MaxPortalStringSize
			{
				set
				{
					base.PowerSharpParameters["MaxPortalStringSize"] = value;
				}
			}

			public virtual int? MaxFwdAllowed
			{
				set
				{
					base.PowerSharpParameters["MaxFwdAllowed"] = value;
				}
			}

			public virtual int? PortalInactivityTimeout
			{
				set
				{
					base.PowerSharpParameters["PortalInactivityTimeout"] = value;
				}
			}

			public virtual int? TDSTimeOut
			{
				set
				{
					base.PowerSharpParameters["TDSTimeOut"] = value;
				}
			}

			public virtual bool BasicAuthentication
			{
				set
				{
					base.PowerSharpParameters["BasicAuthentication"] = value;
				}
			}

			public virtual bool WindowsAuthentication
			{
				set
				{
					base.PowerSharpParameters["WindowsAuthentication"] = value;
				}
			}

			public virtual bool LiveIdAuthentication
			{
				set
				{
					base.PowerSharpParameters["LiveIdAuthentication"] = value;
				}
			}

			public virtual GzipLevel GzipLevel
			{
				set
				{
					base.PowerSharpParameters["GzipLevel"] = value;
				}
			}

			public virtual ExtendedProtectionTokenCheckingMode ExtendedProtectionTokenChecking
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionTokenChecking"] = value;
				}
			}

			public virtual MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlags
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionFlags"] = value;
				}
			}

			public virtual MultiValuedProperty<string> ExtendedProtectionSPNList
			{
				set
				{
					base.PowerSharpParameters["ExtendedProtectionSPNList"] = value;
				}
			}

			public virtual Uri InternalUrl
			{
				set
				{
					base.PowerSharpParameters["InternalUrl"] = value;
				}
			}

			public virtual Uri ExternalUrl
			{
				set
				{
					base.PowerSharpParameters["ExternalUrl"] = value;
				}
			}

			public virtual MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
			{
				set
				{
					base.PowerSharpParameters["ExternalAuthenticationMethods"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
