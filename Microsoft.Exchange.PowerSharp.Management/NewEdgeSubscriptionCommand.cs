using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewEdgeSubscriptionCommand : SyntheticCommandWithPipelineInput<Server, Server>
	{
		private NewEdgeSubscriptionCommand() : base("New-EdgeSubscription")
		{
		}

		public NewEdgeSubscriptionCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewEdgeSubscriptionCommand SetParameters(NewEdgeSubscriptionCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual TimeSpan AccountExpiryDuration
			{
				set
				{
					base.PowerSharpParameters["AccountExpiryDuration"] = value;
				}
			}

			public virtual LongPath FileName
			{
				set
				{
					base.PowerSharpParameters["FileName"] = value;
				}
			}

			public virtual string Site
			{
				set
				{
					base.PowerSharpParameters["Site"] = ((value != null) ? new AdSiteIdParameter(value) : null);
				}
			}

			public virtual bool CreateInternetSendConnector
			{
				set
				{
					base.PowerSharpParameters["CreateInternetSendConnector"] = value;
				}
			}

			public virtual bool CreateInboundSendConnector
			{
				set
				{
					base.PowerSharpParameters["CreateInboundSendConnector"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual byte FileData
			{
				set
				{
					base.PowerSharpParameters["FileData"] = value;
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
