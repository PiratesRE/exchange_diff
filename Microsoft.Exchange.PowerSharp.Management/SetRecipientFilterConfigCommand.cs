﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetRecipientFilterConfigCommand : SyntheticCommandWithPipelineInputNoOutput<RecipientFilterConfig>
	{
		private SetRecipientFilterConfigCommand() : base("Set-RecipientFilterConfig")
		{
		}

		public SetRecipientFilterConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetRecipientFilterConfigCommand SetParameters(SetRecipientFilterConfigCommand.DefaultParameters parameters)
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

			public virtual MultiValuedProperty<SmtpAddress> BlockedRecipients
			{
				set
				{
					base.PowerSharpParameters["BlockedRecipients"] = value;
				}
			}

			public virtual bool RecipientValidationEnabled
			{
				set
				{
					base.PowerSharpParameters["RecipientValidationEnabled"] = value;
				}
			}

			public virtual bool BlockListEnabled
			{
				set
				{
					base.PowerSharpParameters["BlockListEnabled"] = value;
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
