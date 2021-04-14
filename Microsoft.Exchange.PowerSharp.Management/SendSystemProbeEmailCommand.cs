using System;
using System.Management.Automation;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SendSystemProbeEmailCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private SendSystemProbeEmailCommand() : base("Send-SystemProbeEmail")
		{
		}

		public SendSystemProbeEmailCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SendSystemProbeEmailCommand SetParameters(SendSystemProbeEmailCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Subject
			{
				set
				{
					base.PowerSharpParameters["Subject"] = value;
				}
			}

			public virtual string Body
			{
				set
				{
					base.PowerSharpParameters["Body"] = value;
				}
			}

			public virtual string Attachments
			{
				set
				{
					base.PowerSharpParameters["Attachments"] = value;
				}
			}

			public virtual string SmtpServer
			{
				set
				{
					base.PowerSharpParameters["SmtpServer"] = value;
				}
			}

			public virtual string SmtpUser
			{
				set
				{
					base.PowerSharpParameters["SmtpUser"] = value;
				}
			}

			public virtual string SmtpPassword
			{
				set
				{
					base.PowerSharpParameters["SmtpPassword"] = value;
				}
			}

			public virtual string From
			{
				set
				{
					base.PowerSharpParameters["From"] = value;
				}
			}

			public virtual string To
			{
				set
				{
					base.PowerSharpParameters["To"] = value;
				}
			}

			public virtual string CC
			{
				set
				{
					base.PowerSharpParameters["CC"] = value;
				}
			}

			public virtual bool Html
			{
				set
				{
					base.PowerSharpParameters["Html"] = value;
				}
			}

			public virtual Guid ProbeGuid
			{
				set
				{
					base.PowerSharpParameters["ProbeGuid"] = value;
				}
			}

			public virtual bool UseSsl
			{
				set
				{
					base.PowerSharpParameters["UseSsl"] = value;
				}
			}

			public virtual int Port
			{
				set
				{
					base.PowerSharpParameters["Port"] = value;
				}
			}

			public virtual SwitchParameter UseXheader
			{
				set
				{
					base.PowerSharpParameters["UseXheader"] = value;
				}
			}

			public virtual bool TestContext
			{
				set
				{
					base.PowerSharpParameters["TestContext"] = value;
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
		}
	}
}
