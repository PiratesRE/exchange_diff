using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetHybridMailflowCommand : SyntheticCommand<object>
	{
		private SetHybridMailflowCommand() : base("Set-HybridMailflow")
		{
		}

		public SetHybridMailflowCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetHybridMailflowCommand SetParameters(SetHybridMailflowCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SmtpDomainWithSubdomains OutboundDomains
			{
				set
				{
					base.PowerSharpParameters["OutboundDomains"] = value;
				}
			}

			public virtual IPRange InboundIPs
			{
				set
				{
					base.PowerSharpParameters["InboundIPs"] = value;
				}
			}

			public virtual Fqdn OnPremisesFQDN
			{
				set
				{
					base.PowerSharpParameters["OnPremisesFQDN"] = value;
				}
			}

			public virtual string CertificateSubject
			{
				set
				{
					base.PowerSharpParameters["CertificateSubject"] = value;
				}
			}

			public virtual bool? SecureMailEnabled
			{
				set
				{
					base.PowerSharpParameters["SecureMailEnabled"] = value;
				}
			}

			public virtual bool? CentralizedTransportEnabled
			{
				set
				{
					base.PowerSharpParameters["CentralizedTransportEnabled"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
