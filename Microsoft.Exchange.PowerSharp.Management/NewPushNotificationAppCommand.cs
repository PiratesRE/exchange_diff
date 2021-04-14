using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewPushNotificationAppCommand : SyntheticCommandWithPipelineInput<PushNotificationApp, PushNotificationApp>
	{
		private NewPushNotificationAppCommand() : base("New-PushNotificationApp")
		{
		}

		public NewPushNotificationAppCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.ApnsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.WnsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.AzureSendParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.GcmParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.AzureHubCreationParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.PendingGetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.WebAppParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.ProxyParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.AzureChallengeRequestParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewPushNotificationAppCommand SetParameters(NewPushNotificationAppCommand.AzureDeviceRegistrationParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class ApnsParameters : ParametersBase
		{
			public virtual SwitchParameter AsApns
			{
				set
				{
					base.PowerSharpParameters["AsApns"] = value;
				}
			}

			public virtual string CertificateThumbprint
			{
				set
				{
					base.PowerSharpParameters["CertificateThumbprint"] = value;
				}
			}

			public virtual string CertificateThumbprintFallback
			{
				set
				{
					base.PowerSharpParameters["CertificateThumbprintFallback"] = value;
				}
			}

			public virtual string ApnsHost
			{
				set
				{
					base.PowerSharpParameters["ApnsHost"] = value;
				}
			}

			public virtual int? ApnsPort
			{
				set
				{
					base.PowerSharpParameters["ApnsPort"] = value;
				}
			}

			public virtual string FeedbackHost
			{
				set
				{
					base.PowerSharpParameters["FeedbackHost"] = value;
				}
			}

			public virtual int? FeedbackPort
			{
				set
				{
					base.PowerSharpParameters["FeedbackPort"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class WnsParameters : ParametersBase
		{
			public virtual SwitchParameter AsWns
			{
				set
				{
					base.PowerSharpParameters["AsWns"] = value;
				}
			}

			public virtual string AppSid
			{
				set
				{
					base.PowerSharpParameters["AppSid"] = value;
				}
			}

			public virtual string AppSecret
			{
				set
				{
					base.PowerSharpParameters["AppSecret"] = value;
				}
			}

			public virtual string AuthenticationUri
			{
				set
				{
					base.PowerSharpParameters["AuthenticationUri"] = value;
				}
			}

			public virtual SwitchParameter UseClearTextAuthenticationKeys
			{
				set
				{
					base.PowerSharpParameters["UseClearTextAuthenticationKeys"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class AzureSendParameters : ParametersBase
		{
			public virtual SwitchParameter UseClearTextAuthenticationKeys
			{
				set
				{
					base.PowerSharpParameters["UseClearTextAuthenticationKeys"] = value;
				}
			}

			public virtual SwitchParameter AsAzureSend
			{
				set
				{
					base.PowerSharpParameters["AsAzureSend"] = value;
				}
			}

			public virtual string SasKeyName
			{
				set
				{
					base.PowerSharpParameters["SasKeyName"] = value;
				}
			}

			public virtual string SasKeyValue
			{
				set
				{
					base.PowerSharpParameters["SasKeyValue"] = value;
				}
			}

			public virtual string UriTemplate
			{
				set
				{
					base.PowerSharpParameters["UriTemplate"] = value;
				}
			}

			public virtual string RegistrationTemplate
			{
				set
				{
					base.PowerSharpParameters["RegistrationTemplate"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class GcmParameters : ParametersBase
		{
			public virtual SwitchParameter UseClearTextAuthenticationKeys
			{
				set
				{
					base.PowerSharpParameters["UseClearTextAuthenticationKeys"] = value;
				}
			}

			public virtual SwitchParameter AsGcm
			{
				set
				{
					base.PowerSharpParameters["AsGcm"] = value;
				}
			}

			public virtual string Sender
			{
				set
				{
					base.PowerSharpParameters["Sender"] = value;
				}
			}

			public virtual string SenderAuthToken
			{
				set
				{
					base.PowerSharpParameters["SenderAuthToken"] = value;
				}
			}

			public virtual string GcmServiceUri
			{
				set
				{
					base.PowerSharpParameters["GcmServiceUri"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class AzureHubCreationParameters : ParametersBase
		{
			public virtual SwitchParameter UseClearTextAuthenticationKeys
			{
				set
				{
					base.PowerSharpParameters["UseClearTextAuthenticationKeys"] = value;
				}
			}

			public virtual string UriTemplate
			{
				set
				{
					base.PowerSharpParameters["UriTemplate"] = value;
				}
			}

			public virtual SwitchParameter AsAzureHubCreation
			{
				set
				{
					base.PowerSharpParameters["AsAzureHubCreation"] = value;
				}
			}

			public virtual string AcsUserName
			{
				set
				{
					base.PowerSharpParameters["AcsUserName"] = value;
				}
			}

			public virtual string AcsUserPassword
			{
				set
				{
					base.PowerSharpParameters["AcsUserPassword"] = value;
				}
			}

			public virtual string AcsUriTemplate
			{
				set
				{
					base.PowerSharpParameters["AcsUriTemplate"] = value;
				}
			}

			public virtual string AcsScopeUriTemplate
			{
				set
				{
					base.PowerSharpParameters["AcsScopeUriTemplate"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class PendingGetParameters : ParametersBase
		{
			public virtual SwitchParameter AsPendingGet
			{
				set
				{
					base.PowerSharpParameters["AsPendingGet"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class WebAppParameters : ParametersBase
		{
			public virtual SwitchParameter AsWebApp
			{
				set
				{
					base.PowerSharpParameters["AsWebApp"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class ProxyParameters : ParametersBase
		{
			public virtual SwitchParameter AsProxy
			{
				set
				{
					base.PowerSharpParameters["AsProxy"] = value;
				}
			}

			public virtual string Uri
			{
				set
				{
					base.PowerSharpParameters["Uri"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class AzureChallengeRequestParameters : ParametersBase
		{
			public virtual SwitchParameter AsAzureChallengeRequest
			{
				set
				{
					base.PowerSharpParameters["AsAzureChallengeRequest"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class AzureDeviceRegistrationParameters : ParametersBase
		{
			public virtual SwitchParameter AsAzureDeviceRegistration
			{
				set
				{
					base.PowerSharpParameters["AsAzureDeviceRegistration"] = value;
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool? Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Version ExchangeMinimumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMinimumVersion"] = value;
				}
			}

			public virtual Version ExchangeMaximumVersion
			{
				set
				{
					base.PowerSharpParameters["ExchangeMaximumVersion"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
