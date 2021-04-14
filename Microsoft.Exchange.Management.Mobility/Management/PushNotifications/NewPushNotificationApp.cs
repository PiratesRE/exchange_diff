using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Cmdlet("New", "PushNotificationApp", DefaultParameterSetName = "Default", SupportsShouldProcess = true)]
	public sealed class NewPushNotificationApp : NewSystemConfigurationObjectTask<PushNotificationApp>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewApp(base.Name);
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return this.DataObject.DisplayName;
			}
			set
			{
				this.DataObject.DisplayName = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version ExchangeMinimumVersion
		{
			get
			{
				return this.DataObject.ExchangeMinimumVersion;
			}
			set
			{
				this.DataObject.ExchangeMinimumVersion = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Version ExchangeMaximumVersion
		{
			get
			{
				return this.DataObject.ExchangeMaximumVersion;
			}
			set
			{
				this.DataObject.ExchangeMaximumVersion = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Apns")]
		public SwitchParameter AsApns
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.APNS);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.APNS;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Apns")]
		public string CertificateThumbprint
		{
			get
			{
				return this.DataObject.AuthenticationKey;
			}
			set
			{
				this.DataObject.AuthenticationKey = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public string CertificateThumbprintFallback
		{
			get
			{
				return this.DataObject.AuthenticationKeyFallback;
			}
			set
			{
				this.DataObject.AuthenticationKeyFallback = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public string ApnsHost
		{
			get
			{
				return this.DataObject.Url;
			}
			set
			{
				this.DataObject.Url = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public int? ApnsPort
		{
			get
			{
				return this.DataObject.Port;
			}
			set
			{
				this.DataObject.Port = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public string FeedbackHost
		{
			get
			{
				return this.DataObject.SecondaryUrl;
			}
			set
			{
				this.DataObject.SecondaryUrl = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public int? FeedbackPort
		{
			get
			{
				return this.DataObject.SecondaryPort;
			}
			set
			{
				this.DataObject.SecondaryPort = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Wns")]
		public SwitchParameter AsWns
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.WNS);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.WNS;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Wns")]
		public string AppSid
		{
			get
			{
				return this.DataObject.AuthenticationId;
			}
			set
			{
				this.DataObject.AuthenticationId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Wns")]
		public string AppSecret
		{
			get
			{
				return this.DataObject.AuthenticationKey;
			}
			set
			{
				this.DataObject.AuthenticationKey = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Wns")]
		public string AuthenticationUri
		{
			get
			{
				return this.DataObject.SecondaryUrl;
			}
			set
			{
				this.DataObject.SecondaryUrl = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		[Parameter(Mandatory = false, ParameterSetName = "Wns")]
		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public SwitchParameter UseClearTextAuthenticationKeys
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseClearTextAuthenticationKeys"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseClearTextAuthenticationKeys"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Gcm")]
		public SwitchParameter AsGcm
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.GCM);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.GCM;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		public string Sender
		{
			get
			{
				return this.DataObject.AuthenticationId;
			}
			set
			{
				this.DataObject.AuthenticationId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		public string SenderAuthToken
		{
			get
			{
				return this.DataObject.AuthenticationKey;
			}
			set
			{
				this.DataObject.AuthenticationKey = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		public string GcmServiceUri
		{
			get
			{
				return this.DataObject.Url;
			}
			set
			{
				this.DataObject.Url = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PendingGet")]
		public SwitchParameter AsPendingGet
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.PendingGet);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.PendingGet;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "WebApp")]
		public SwitchParameter AsWebApp
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.WebApp);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.WebApp;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Proxy")]
		public SwitchParameter AsProxy
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.Proxy);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.Proxy;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Proxy")]
		public string Uri
		{
			get
			{
				return this.DataObject.Url;
			}
			set
			{
				this.DataObject.Url = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Proxy")]
		public string Organization
		{
			get
			{
				return this.DataObject.AuthenticationKey;
			}
			set
			{
				this.DataObject.AuthenticationKey = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AzureSend")]
		public SwitchParameter AsAzureSend
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.Azure);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.Azure;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string SasKeyName
		{
			get
			{
				return this.DataObject.AuthenticationId;
			}
			set
			{
				this.DataObject.AuthenticationId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string SasKeyValue
		{
			get
			{
				return this.DataObject.AuthenticationKey;
			}
			set
			{
				this.DataObject.AuthenticationKey = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string UriTemplate
		{
			get
			{
				return this.DataObject.UriTemplate;
			}
			set
			{
				this.DataObject.UriTemplate = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string RegistrationTemplate
		{
			get
			{
				return this.DataObject.RegistrationTemplate;
			}
			set
			{
				this.DataObject.RegistrationTemplate = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AzureHubCreation")]
		public SwitchParameter AsAzureHubCreation
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.AzureHubCreation);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.AzureHubCreation;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsUserName
		{
			get
			{
				return this.DataObject.AuthenticationId;
			}
			set
			{
				this.DataObject.AuthenticationId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsUserPassword
		{
			get
			{
				return this.DataObject.AuthenticationKey;
			}
			set
			{
				this.DataObject.AuthenticationKey = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsUriTemplate
		{
			get
			{
				return this.DataObject.Url;
			}
			set
			{
				this.DataObject.Url = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsScopeUriTemplate
		{
			get
			{
				return this.DataObject.SecondaryUrl;
			}
			set
			{
				this.DataObject.SecondaryUrl = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AzureChallengeRequest")]
		public SwitchParameter AsAzureChallengeRequest
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.AzureChallengeRequest);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.AzureChallengeRequest;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AzureDeviceRegistration")]
		public SwitchParameter AsAzureDeviceRegistration
		{
			get
			{
				return this.DataObject.Platform.Equals(PushNotificationPlatform.AzureDeviceRegistration);
			}
			set
			{
				this.DataObject.Platform = PushNotificationPlatform.AzureDeviceRegistration;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 467, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\PushNotifications\\NewPushNotificationApp.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			PushNotificationApp pushNotificationApp = (PushNotificationApp)base.PrepareDataObject();
			pushNotificationApp.SetId((IConfigurationSession)base.DataSession, base.Name);
			if (string.IsNullOrEmpty(pushNotificationApp.DisplayName))
			{
				pushNotificationApp.DisplayName = pushNotificationApp.Name;
			}
			if (base.ParameterSetName == "Wns" || base.ParameterSetName == "Gcm" || base.ParameterSetName == "AzureSend" || base.ParameterSetName == "AzureHubCreation")
			{
				pushNotificationApp.IsAuthenticationKeyEncrypted = new bool?(!this.UseClearTextAuthenticationKeys);
				if (pushNotificationApp.IsAuthenticationKeyEncrypted.Value)
				{
					PushNotificationDataProtector pushNotificationDataProtector = new PushNotificationDataProtector(null);
					pushNotificationApp.AuthenticationKey = pushNotificationDataProtector.Encrypt(pushNotificationApp.AuthenticationKey);
				}
			}
			return pushNotificationApp;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			PushNotificationPublisherSettingsFactory pushNotificationPublisherSettingsFactory = new PushNotificationPublisherSettingsFactory();
			PushNotificationPublisherSettings pushNotificationPublisherSettings = pushNotificationPublisherSettingsFactory.Create(this.DataObject);
			try
			{
				pushNotificationPublisherSettings.Validate();
			}
			catch (PushNotificationConfigurationException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerOperation, null);
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PushNotificationApp pushNotificationApp = dataObject as PushNotificationApp;
			base.WriteResult(new PushNotificationAppPresentationObject(pushNotificationApp));
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is PushNotificationConfigurationException || base.IsKnownException(exception);
		}
	}
}
