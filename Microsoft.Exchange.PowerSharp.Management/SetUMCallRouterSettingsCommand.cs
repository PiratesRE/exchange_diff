using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMCallRouterSettingsCommand : SyntheticCommandWithPipelineInputNoOutput<SIPFEServerConfiguration>
	{
		private SetUMCallRouterSettingsCommand() : base("Set-UMCallRouterSettings")
		{
		}

		public SetUMCallRouterSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMCallRouterSettingsCommand SetParameters(SetUMCallRouterSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual MultiValuedProperty<UMDialPlanIdParameter> DialPlans
			{
				set
				{
					base.PowerSharpParameters["DialPlans"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int? MaxCallsAllowed
			{
				set
				{
					base.PowerSharpParameters["MaxCallsAllowed"] = value;
				}
			}

			public virtual int SipTcpListeningPort
			{
				set
				{
					base.PowerSharpParameters["SipTcpListeningPort"] = value;
				}
			}

			public virtual int SipTlsListeningPort
			{
				set
				{
					base.PowerSharpParameters["SipTlsListeningPort"] = value;
				}
			}

			public virtual UMSmartHost ExternalHostFqdn
			{
				set
				{
					base.PowerSharpParameters["ExternalHostFqdn"] = value;
				}
			}

			public virtual UMSmartHost ExternalServiceFqdn
			{
				set
				{
					base.PowerSharpParameters["ExternalServiceFqdn"] = value;
				}
			}

			public virtual string UMPodRedirectTemplate
			{
				set
				{
					base.PowerSharpParameters["UMPodRedirectTemplate"] = value;
				}
			}

			public virtual string UMForwardingAddressTemplate
			{
				set
				{
					base.PowerSharpParameters["UMForwardingAddressTemplate"] = value;
				}
			}

			public virtual UMStartupMode UMStartupMode
			{
				set
				{
					base.PowerSharpParameters["UMStartupMode"] = value;
				}
			}

			public virtual bool IPAddressFamilyConfigurable
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamilyConfigurable"] = value;
				}
			}

			public virtual IPAddressFamily IPAddressFamily
			{
				set
				{
					base.PowerSharpParameters["IPAddressFamily"] = value;
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
