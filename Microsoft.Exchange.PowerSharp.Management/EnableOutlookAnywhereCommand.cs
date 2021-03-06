using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class EnableOutlookAnywhereCommand : SyntheticCommandWithPipelineInput<ADRpcHttpVirtualDirectory, ADRpcHttpVirtualDirectory>
	{
		private EnableOutlookAnywhereCommand() : base("Enable-OutlookAnywhere")
		{
		}

		public EnableOutlookAnywhereCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual EnableOutlookAnywhereCommand SetParameters(EnableOutlookAnywhereCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool SSLOffloading
			{
				set
				{
					base.PowerSharpParameters["SSLOffloading"] = value;
				}
			}

			public virtual Hostname ExternalHostname
			{
				set
				{
					base.PowerSharpParameters["ExternalHostname"] = value;
				}
			}

			public virtual Hostname InternalHostname
			{
				set
				{
					base.PowerSharpParameters["InternalHostname"] = value;
				}
			}

			public virtual AuthenticationMethod DefaultAuthenticationMethod
			{
				set
				{
					base.PowerSharpParameters["DefaultAuthenticationMethod"] = value;
				}
			}

			public virtual AuthenticationMethod ExternalClientAuthenticationMethod
			{
				set
				{
					base.PowerSharpParameters["ExternalClientAuthenticationMethod"] = value;
				}
			}

			public virtual AuthenticationMethod InternalClientAuthenticationMethod
			{
				set
				{
					base.PowerSharpParameters["InternalClientAuthenticationMethod"] = value;
				}
			}

			public virtual MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
			{
				set
				{
					base.PowerSharpParameters["IISAuthenticationMethods"] = value;
				}
			}

			public virtual Uri XropUrl
			{
				set
				{
					base.PowerSharpParameters["XropUrl"] = value;
				}
			}

			public virtual bool ExternalClientsRequireSsl
			{
				set
				{
					base.PowerSharpParameters["ExternalClientsRequireSsl"] = value;
				}
			}

			public virtual bool InternalClientsRequireSsl
			{
				set
				{
					base.PowerSharpParameters["InternalClientsRequireSsl"] = value;
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

			public virtual VirtualDirectoryRole Role
			{
				set
				{
					base.PowerSharpParameters["Role"] = value;
				}
			}

			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
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
