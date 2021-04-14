using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetActiveSyncVirtualDirectoryCommand : SyntheticCommandWithPipelineInputNoOutput<ADMobileVirtualDirectory>
	{
		private SetActiveSyncVirtualDirectoryCommand() : base("Set-ActiveSyncVirtualDirectory")
		{
		}

		public SetActiveSyncVirtualDirectoryCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetActiveSyncVirtualDirectoryCommand SetParameters(SetActiveSyncVirtualDirectoryCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetActiveSyncVirtualDirectoryCommand SetParameters(SetActiveSyncVirtualDirectoryCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ActiveSyncServer
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncServer"] = value;
				}
			}

			public virtual bool MobileClientCertificateProvisioningEnabled
			{
				set
				{
					base.PowerSharpParameters["MobileClientCertificateProvisioningEnabled"] = value;
				}
			}

			public virtual bool BadItemReportingEnabled
			{
				set
				{
					base.PowerSharpParameters["BadItemReportingEnabled"] = value;
				}
			}

			public virtual bool SendWatsonReport
			{
				set
				{
					base.PowerSharpParameters["SendWatsonReport"] = value;
				}
			}

			public virtual string MobileClientCertificateAuthorityURL
			{
				set
				{
					base.PowerSharpParameters["MobileClientCertificateAuthorityURL"] = value;
				}
			}

			public virtual string MobileClientCertTemplateName
			{
				set
				{
					base.PowerSharpParameters["MobileClientCertTemplateName"] = value;
				}
			}

			public virtual ClientCertAuthTypes ClientCertAuth
			{
				set
				{
					base.PowerSharpParameters["ClientCertAuth"] = value;
				}
			}

			public virtual bool BasicAuthEnabled
			{
				set
				{
					base.PowerSharpParameters["BasicAuthEnabled"] = value;
				}
			}

			public virtual bool WindowsAuthEnabled
			{
				set
				{
					base.PowerSharpParameters["WindowsAuthEnabled"] = value;
				}
			}

			public virtual bool CompressionEnabled
			{
				set
				{
					base.PowerSharpParameters["CompressionEnabled"] = value;
				}
			}

			public virtual RemoteDocumentsActions? RemoteDocumentsActionForUnknownServers
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsActionForUnknownServers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RemoteDocumentsAllowedServers
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsAllowedServers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RemoteDocumentsBlockedServers
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsBlockedServers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RemoteDocumentsInternalDomainSuffixList
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsInternalDomainSuffixList"] = value;
				}
			}

			public virtual bool InstallIsapiFilter
			{
				set
				{
					base.PowerSharpParameters["InstallIsapiFilter"] = value;
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

			public virtual MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
			{
				set
				{
					base.PowerSharpParameters["InternalAuthenticationMethods"] = value;
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

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

			public virtual string ActiveSyncServer
			{
				set
				{
					base.PowerSharpParameters["ActiveSyncServer"] = value;
				}
			}

			public virtual bool MobileClientCertificateProvisioningEnabled
			{
				set
				{
					base.PowerSharpParameters["MobileClientCertificateProvisioningEnabled"] = value;
				}
			}

			public virtual bool BadItemReportingEnabled
			{
				set
				{
					base.PowerSharpParameters["BadItemReportingEnabled"] = value;
				}
			}

			public virtual bool SendWatsonReport
			{
				set
				{
					base.PowerSharpParameters["SendWatsonReport"] = value;
				}
			}

			public virtual string MobileClientCertificateAuthorityURL
			{
				set
				{
					base.PowerSharpParameters["MobileClientCertificateAuthorityURL"] = value;
				}
			}

			public virtual string MobileClientCertTemplateName
			{
				set
				{
					base.PowerSharpParameters["MobileClientCertTemplateName"] = value;
				}
			}

			public virtual ClientCertAuthTypes ClientCertAuth
			{
				set
				{
					base.PowerSharpParameters["ClientCertAuth"] = value;
				}
			}

			public virtual bool BasicAuthEnabled
			{
				set
				{
					base.PowerSharpParameters["BasicAuthEnabled"] = value;
				}
			}

			public virtual bool WindowsAuthEnabled
			{
				set
				{
					base.PowerSharpParameters["WindowsAuthEnabled"] = value;
				}
			}

			public virtual bool CompressionEnabled
			{
				set
				{
					base.PowerSharpParameters["CompressionEnabled"] = value;
				}
			}

			public virtual RemoteDocumentsActions? RemoteDocumentsActionForUnknownServers
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsActionForUnknownServers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RemoteDocumentsAllowedServers
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsAllowedServers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RemoteDocumentsBlockedServers
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsBlockedServers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> RemoteDocumentsInternalDomainSuffixList
			{
				set
				{
					base.PowerSharpParameters["RemoteDocumentsInternalDomainSuffixList"] = value;
				}
			}

			public virtual bool InstallIsapiFilter
			{
				set
				{
					base.PowerSharpParameters["InstallIsapiFilter"] = value;
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

			public virtual MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
			{
				set
				{
					base.PowerSharpParameters["InternalAuthenticationMethods"] = value;
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

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
