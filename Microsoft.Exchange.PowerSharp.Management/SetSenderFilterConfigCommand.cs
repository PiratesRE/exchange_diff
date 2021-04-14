using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetSenderFilterConfigCommand : SyntheticCommandWithPipelineInputNoOutput<SenderFilterConfig>
	{
		private SetSenderFilterConfigCommand() : base("Set-SenderFilterConfig")
		{
		}

		public SetSenderFilterConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetSenderFilterConfigCommand SetParameters(SetSenderFilterConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> BlockedSenders
			{
				set
				{
					base.PowerSharpParameters["BlockedSenders"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomain> BlockedDomains
			{
				set
				{
					base.PowerSharpParameters["BlockedDomains"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomain> BlockedDomainsAndSubdomains
			{
				set
				{
					base.PowerSharpParameters["BlockedDomainsAndSubdomains"] = value;
				}
			}

			public virtual BlockedSenderAction Action
			{
				set
				{
					base.PowerSharpParameters["Action"] = value;
				}
			}

			public virtual bool BlankSenderBlockingEnabled
			{
				set
				{
					base.PowerSharpParameters["BlankSenderBlockingEnabled"] = value;
				}
			}

			public virtual RecipientBlockedSenderAction RecipientBlockedSenderAction
			{
				set
				{
					base.PowerSharpParameters["RecipientBlockedSenderAction"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool ExternalMailEnabled
			{
				set
				{
					base.PowerSharpParameters["ExternalMailEnabled"] = value;
				}
			}

			public virtual bool InternalMailEnabled
			{
				set
				{
					base.PowerSharpParameters["InternalMailEnabled"] = value;
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
