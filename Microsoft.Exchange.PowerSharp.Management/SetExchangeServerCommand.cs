using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetExchangeServerCommand : SyntheticCommandWithPipelineInputNoOutput<ExchangeServer>
	{
		private SetExchangeServerCommand() : base("Set-ExchangeServer")
		{
		}

		public SetExchangeServerCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetExchangeServerCommand SetParameters(SetExchangeServerCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeServerCommand SetParameters(SetExchangeServerCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ProductKey ProductKey
			{
				set
				{
					base.PowerSharpParameters["ProductKey"] = value;
				}
			}

			public virtual bool ErrorReportingEnabled
			{
				set
				{
					base.PowerSharpParameters["ErrorReportingEnabled"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxProvisioningAttributes MailboxProvisioningAttributes
			{
				set
				{
					base.PowerSharpParameters["MailboxProvisioningAttributes"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool? CustomerFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["CustomerFeedbackEnabled"] = value;
				}
			}

			public virtual Uri InternetWebProxy
			{
				set
				{
					base.PowerSharpParameters["InternetWebProxy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> StaticDomainControllers
			{
				set
				{
					base.PowerSharpParameters["StaticDomainControllers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> StaticGlobalCatalogs
			{
				set
				{
					base.PowerSharpParameters["StaticGlobalCatalogs"] = value;
				}
			}

			public virtual string StaticConfigDomainController
			{
				set
				{
					base.PowerSharpParameters["StaticConfigDomainController"] = value;
				}
			}

			public virtual MultiValuedProperty<string> StaticExcludedDomainControllers
			{
				set
				{
					base.PowerSharpParameters["StaticExcludedDomainControllers"] = value;
				}
			}

			public virtual string MonitoringGroup
			{
				set
				{
					base.PowerSharpParameters["MonitoringGroup"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual ServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual ProductKey ProductKey
			{
				set
				{
					base.PowerSharpParameters["ProductKey"] = value;
				}
			}

			public virtual bool ErrorReportingEnabled
			{
				set
				{
					base.PowerSharpParameters["ErrorReportingEnabled"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxProvisioningAttributes MailboxProvisioningAttributes
			{
				set
				{
					base.PowerSharpParameters["MailboxProvisioningAttributes"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool? CustomerFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["CustomerFeedbackEnabled"] = value;
				}
			}

			public virtual Uri InternetWebProxy
			{
				set
				{
					base.PowerSharpParameters["InternetWebProxy"] = value;
				}
			}

			public virtual MultiValuedProperty<string> StaticDomainControllers
			{
				set
				{
					base.PowerSharpParameters["StaticDomainControllers"] = value;
				}
			}

			public virtual MultiValuedProperty<string> StaticGlobalCatalogs
			{
				set
				{
					base.PowerSharpParameters["StaticGlobalCatalogs"] = value;
				}
			}

			public virtual string StaticConfigDomainController
			{
				set
				{
					base.PowerSharpParameters["StaticConfigDomainController"] = value;
				}
			}

			public virtual MultiValuedProperty<string> StaticExcludedDomainControllers
			{
				set
				{
					base.PowerSharpParameters["StaticExcludedDomainControllers"] = value;
				}
			}

			public virtual string MonitoringGroup
			{
				set
				{
					base.PowerSharpParameters["MonitoringGroup"] = value;
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
