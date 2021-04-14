using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetPushNotificationAppCommand : SyntheticCommandWithPipelineInputNoOutput<PushNotificationApp>
	{
		private SetPushNotificationAppCommand() : base("Set-PushNotificationApp")
		{
		}

		public SetPushNotificationAppCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetPushNotificationAppCommand SetParameters(SetPushNotificationAppCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPushNotificationAppCommand SetParameters(SetPushNotificationAppCommand.ApnsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPushNotificationAppCommand SetParameters(SetPushNotificationAppCommand.WnsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPushNotificationAppCommand SetParameters(SetPushNotificationAppCommand.AzureHubCreationParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPushNotificationAppCommand SetParameters(SetPushNotificationAppCommand.GcmParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPushNotificationAppCommand SetParameters(SetPushNotificationAppCommand.AzureSendParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPushNotificationAppCommand SetParameters(SetPushNotificationAppCommand.ProxyParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual int? QueueSize
			{
				set
				{
					base.PowerSharpParameters["QueueSize"] = value;
				}
			}

			public virtual int? NumberOfChannels
			{
				set
				{
					base.PowerSharpParameters["NumberOfChannels"] = value;
				}
			}

			public virtual int? BackOffTimeInSeconds
			{
				set
				{
					base.PowerSharpParameters["BackOffTimeInSeconds"] = value;
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

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual int? QueueSize
			{
				set
				{
					base.PowerSharpParameters["QueueSize"] = value;
				}
			}

			public virtual int? NumberOfChannels
			{
				set
				{
					base.PowerSharpParameters["NumberOfChannels"] = value;
				}
			}

			public virtual int? BackOffTimeInSeconds
			{
				set
				{
					base.PowerSharpParameters["BackOffTimeInSeconds"] = value;
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

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual int? QueueSize
			{
				set
				{
					base.PowerSharpParameters["QueueSize"] = value;
				}
			}

			public virtual int? NumberOfChannels
			{
				set
				{
					base.PowerSharpParameters["NumberOfChannels"] = value;
				}
			}

			public virtual int? BackOffTimeInSeconds
			{
				set
				{
					base.PowerSharpParameters["BackOffTimeInSeconds"] = value;
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

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual int? QueueSize
			{
				set
				{
					base.PowerSharpParameters["QueueSize"] = value;
				}
			}

			public virtual int? NumberOfChannels
			{
				set
				{
					base.PowerSharpParameters["NumberOfChannels"] = value;
				}
			}

			public virtual int? BackOffTimeInSeconds
			{
				set
				{
					base.PowerSharpParameters["BackOffTimeInSeconds"] = value;
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

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual int? QueueSize
			{
				set
				{
					base.PowerSharpParameters["QueueSize"] = value;
				}
			}

			public virtual int? NumberOfChannels
			{
				set
				{
					base.PowerSharpParameters["NumberOfChannels"] = value;
				}
			}

			public virtual int? BackOffTimeInSeconds
			{
				set
				{
					base.PowerSharpParameters["BackOffTimeInSeconds"] = value;
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

			public virtual bool? RegistrationEnabled
			{
				set
				{
					base.PowerSharpParameters["RegistrationEnabled"] = value;
				}
			}

			public virtual bool? MultifactorRegistrationEnabled
			{
				set
				{
					base.PowerSharpParameters["MultifactorRegistrationEnabled"] = value;
				}
			}

			public virtual string PartitionName
			{
				set
				{
					base.PowerSharpParameters["PartitionName"] = value;
				}
			}

			public virtual bool? IsDefaultPartitionName
			{
				set
				{
					base.PowerSharpParameters["IsDefaultPartitionName"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual int? QueueSize
			{
				set
				{
					base.PowerSharpParameters["QueueSize"] = value;
				}
			}

			public virtual int? NumberOfChannels
			{
				set
				{
					base.PowerSharpParameters["NumberOfChannels"] = value;
				}
			}

			public virtual int? BackOffTimeInSeconds
			{
				set
				{
					base.PowerSharpParameters["BackOffTimeInSeconds"] = value;
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

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PushNotificationAppIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

			public virtual int? QueueSize
			{
				set
				{
					base.PowerSharpParameters["QueueSize"] = value;
				}
			}

			public virtual int? NumberOfChannels
			{
				set
				{
					base.PowerSharpParameters["NumberOfChannels"] = value;
				}
			}

			public virtual int? BackOffTimeInSeconds
			{
				set
				{
					base.PowerSharpParameters["BackOffTimeInSeconds"] = value;
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
