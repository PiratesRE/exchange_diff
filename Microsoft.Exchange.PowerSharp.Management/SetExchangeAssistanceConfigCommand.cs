using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetExchangeAssistanceConfigCommand : SyntheticCommandWithPipelineInputNoOutput<ExchangeAssistance>
	{
		private SetExchangeAssistanceConfigCommand() : base("Set-ExchangeAssistanceConfig")
		{
		}

		public SetExchangeAssistanceConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetExchangeAssistanceConfigCommand SetParameters(SetExchangeAssistanceConfigCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeAssistanceConfigCommand SetParameters(SetExchangeAssistanceConfigCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool ExchangeHelpAppOnline
			{
				set
				{
					base.PowerSharpParameters["ExchangeHelpAppOnline"] = value;
				}
			}

			public virtual Uri ControlPanelHelpURL
			{
				set
				{
					base.PowerSharpParameters["ControlPanelHelpURL"] = value;
				}
			}

			public virtual Uri ControlPanelFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["ControlPanelFeedbackURL"] = value;
				}
			}

			public virtual bool ControlPanelFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["ControlPanelFeedbackEnabled"] = value;
				}
			}

			public virtual Uri ManagementConsoleHelpURL
			{
				set
				{
					base.PowerSharpParameters["ManagementConsoleHelpURL"] = value;
				}
			}

			public virtual Uri ManagementConsoleFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["ManagementConsoleFeedbackURL"] = value;
				}
			}

			public virtual bool ManagementConsoleFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["ManagementConsoleFeedbackEnabled"] = value;
				}
			}

			public virtual Uri OWAHelpURL
			{
				set
				{
					base.PowerSharpParameters["OWAHelpURL"] = value;
				}
			}

			public virtual Uri OWAFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["OWAFeedbackURL"] = value;
				}
			}

			public virtual bool OWAFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["OWAFeedbackEnabled"] = value;
				}
			}

			public virtual Uri OWALightHelpURL
			{
				set
				{
					base.PowerSharpParameters["OWALightHelpURL"] = value;
				}
			}

			public virtual Uri OWALightFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["OWALightFeedbackURL"] = value;
				}
			}

			public virtual bool OWALightFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["OWALightFeedbackEnabled"] = value;
				}
			}

			public virtual Uri WindowsLiveAccountPageURL
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveAccountPageURL"] = value;
				}
			}

			public virtual bool WindowsLiveAccountURLEnabled
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveAccountURLEnabled"] = value;
				}
			}

			public virtual Uri PrivacyStatementURL
			{
				set
				{
					base.PowerSharpParameters["PrivacyStatementURL"] = value;
				}
			}

			public virtual bool PrivacyLinkDisplayEnabled
			{
				set
				{
					base.PowerSharpParameters["PrivacyLinkDisplayEnabled"] = value;
				}
			}

			public virtual Uri CommunityURL
			{
				set
				{
					base.PowerSharpParameters["CommunityURL"] = value;
				}
			}

			public virtual bool CommunityLinkDisplayEnabled
			{
				set
				{
					base.PowerSharpParameters["CommunityLinkDisplayEnabled"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool ExchangeHelpAppOnline
			{
				set
				{
					base.PowerSharpParameters["ExchangeHelpAppOnline"] = value;
				}
			}

			public virtual Uri ControlPanelHelpURL
			{
				set
				{
					base.PowerSharpParameters["ControlPanelHelpURL"] = value;
				}
			}

			public virtual Uri ControlPanelFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["ControlPanelFeedbackURL"] = value;
				}
			}

			public virtual bool ControlPanelFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["ControlPanelFeedbackEnabled"] = value;
				}
			}

			public virtual Uri ManagementConsoleHelpURL
			{
				set
				{
					base.PowerSharpParameters["ManagementConsoleHelpURL"] = value;
				}
			}

			public virtual Uri ManagementConsoleFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["ManagementConsoleFeedbackURL"] = value;
				}
			}

			public virtual bool ManagementConsoleFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["ManagementConsoleFeedbackEnabled"] = value;
				}
			}

			public virtual Uri OWAHelpURL
			{
				set
				{
					base.PowerSharpParameters["OWAHelpURL"] = value;
				}
			}

			public virtual Uri OWAFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["OWAFeedbackURL"] = value;
				}
			}

			public virtual bool OWAFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["OWAFeedbackEnabled"] = value;
				}
			}

			public virtual Uri OWALightHelpURL
			{
				set
				{
					base.PowerSharpParameters["OWALightHelpURL"] = value;
				}
			}

			public virtual Uri OWALightFeedbackURL
			{
				set
				{
					base.PowerSharpParameters["OWALightFeedbackURL"] = value;
				}
			}

			public virtual bool OWALightFeedbackEnabled
			{
				set
				{
					base.PowerSharpParameters["OWALightFeedbackEnabled"] = value;
				}
			}

			public virtual Uri WindowsLiveAccountPageURL
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveAccountPageURL"] = value;
				}
			}

			public virtual bool WindowsLiveAccountURLEnabled
			{
				set
				{
					base.PowerSharpParameters["WindowsLiveAccountURLEnabled"] = value;
				}
			}

			public virtual Uri PrivacyStatementURL
			{
				set
				{
					base.PowerSharpParameters["PrivacyStatementURL"] = value;
				}
			}

			public virtual bool PrivacyLinkDisplayEnabled
			{
				set
				{
					base.PowerSharpParameters["PrivacyLinkDisplayEnabled"] = value;
				}
			}

			public virtual Uri CommunityURL
			{
				set
				{
					base.PowerSharpParameters["CommunityURL"] = value;
				}
			}

			public virtual bool CommunityLinkDisplayEnabled
			{
				set
				{
					base.PowerSharpParameters["CommunityLinkDisplayEnabled"] = value;
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
