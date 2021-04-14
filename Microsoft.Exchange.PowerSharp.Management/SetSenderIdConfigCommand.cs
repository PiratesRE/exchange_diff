﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetSenderIdConfigCommand : SyntheticCommandWithPipelineInputNoOutput<SenderIdConfig>
	{
		private SetSenderIdConfigCommand() : base("Set-SenderIdConfig")
		{
		}

		public SetSenderIdConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetSenderIdConfigCommand SetParameters(SetSenderIdConfigCommand.DefaultParameters parameters)
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

			public virtual SenderIdAction SpoofedDomainAction
			{
				set
				{
					base.PowerSharpParameters["SpoofedDomainAction"] = value;
				}
			}

			public virtual SenderIdAction TempErrorAction
			{
				set
				{
					base.PowerSharpParameters["TempErrorAction"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> BypassedRecipients
			{
				set
				{
					base.PowerSharpParameters["BypassedRecipients"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomain> BypassedSenderDomains
			{
				set
				{
					base.PowerSharpParameters["BypassedSenderDomains"] = value;
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
