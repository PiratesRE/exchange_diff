using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetHybridConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<HybridConfiguration>
	{
		private SetHybridConfigurationCommand() : base("Set-HybridConfiguration")
		{
		}

		public SetHybridConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetHybridConfigurationCommand SetParameters(SetHybridConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<ServerIdParameter> ClientAccessServers
			{
				set
				{
					base.PowerSharpParameters["ClientAccessServers"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> ReceivingTransportServers
			{
				set
				{
					base.PowerSharpParameters["ReceivingTransportServers"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> SendingTransportServers
			{
				set
				{
					base.PowerSharpParameters["SendingTransportServers"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> EdgeTransportServers
			{
				set
				{
					base.PowerSharpParameters["EdgeTransportServers"] = value;
				}
			}

			public virtual SmtpX509Identifier TlsCertificateName
			{
				set
				{
					base.PowerSharpParameters["TlsCertificateName"] = value;
				}
			}

			public virtual SmtpDomain OnPremisesSmartHost
			{
				set
				{
					base.PowerSharpParameters["OnPremisesSmartHost"] = value;
				}
			}

			public virtual MultiValuedProperty<AutoDiscoverSmtpDomain> Domains
			{
				set
				{
					base.PowerSharpParameters["Domains"] = value;
				}
			}

			public virtual MultiValuedProperty<HybridFeature> Features
			{
				set
				{
					base.PowerSharpParameters["Features"] = value;
				}
			}

			public virtual MultiValuedProperty<IPRange> ExternalIPAddresses
			{
				set
				{
					base.PowerSharpParameters["ExternalIPAddresses"] = value;
				}
			}

			public virtual int ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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
