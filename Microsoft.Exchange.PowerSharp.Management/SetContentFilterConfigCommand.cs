using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetContentFilterConfigCommand : SyntheticCommandWithPipelineInputNoOutput<ContentFilterConfig>
	{
		private SetContentFilterConfigCommand() : base("Set-ContentFilterConfig")
		{
		}

		public SetContentFilterConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetContentFilterConfigCommand SetParameters(SetContentFilterConfigCommand.DefaultParameters parameters)
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

			public virtual AsciiString RejectionResponse
			{
				set
				{
					base.PowerSharpParameters["RejectionResponse"] = value;
				}
			}

			public virtual bool OutlookEmailPostmarkValidationEnabled
			{
				set
				{
					base.PowerSharpParameters["OutlookEmailPostmarkValidationEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> BypassedRecipients
			{
				set
				{
					base.PowerSharpParameters["BypassedRecipients"] = value;
				}
			}

			public virtual SmtpAddress? QuarantineMailbox
			{
				set
				{
					base.PowerSharpParameters["QuarantineMailbox"] = value;
				}
			}

			public virtual int SCLRejectThreshold
			{
				set
				{
					base.PowerSharpParameters["SCLRejectThreshold"] = value;
				}
			}

			public virtual bool SCLRejectEnabled
			{
				set
				{
					base.PowerSharpParameters["SCLRejectEnabled"] = value;
				}
			}

			public virtual int SCLDeleteThreshold
			{
				set
				{
					base.PowerSharpParameters["SCLDeleteThreshold"] = value;
				}
			}

			public virtual bool SCLDeleteEnabled
			{
				set
				{
					base.PowerSharpParameters["SCLDeleteEnabled"] = value;
				}
			}

			public virtual int SCLQuarantineThreshold
			{
				set
				{
					base.PowerSharpParameters["SCLQuarantineThreshold"] = value;
				}
			}

			public virtual bool SCLQuarantineEnabled
			{
				set
				{
					base.PowerSharpParameters["SCLQuarantineEnabled"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpAddress> BypassedSenders
			{
				set
				{
					base.PowerSharpParameters["BypassedSenders"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomainWithSubdomains> BypassedSenderDomains
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
