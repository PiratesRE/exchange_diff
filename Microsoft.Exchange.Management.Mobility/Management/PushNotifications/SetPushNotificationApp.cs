using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Cmdlet("Set", "PushNotificationApp", SupportsShouldProcess = true, DefaultParameterSetName = "AzureSend")]
	public sealed class SetPushNotificationApp : SetSystemConfigurationObjectTask<PushNotificationAppIdParameter, PushNotificationApp>
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override PushNotificationAppIdParameter Identity
		{
			get
			{
				return (PushNotificationAppIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public string CertificateThumbprint
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public string CertificateThumbprintFallback
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKeyFallback];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKeyFallback] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public string ApnsHost
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.Url];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.Url] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public int? ApnsPort
		{
			get
			{
				return (int?)base.Fields[PushNotificationAppSchema.Port];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.Port] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public string FeedbackHost
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.SecondaryUrl];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.SecondaryUrl] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Apns")]
		public int? FeedbackPort
		{
			get
			{
				return (int?)base.Fields[PushNotificationAppSchema.SecondaryPort];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.SecondaryPort] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Wns")]
		public string AppSid
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationId];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationId] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Wns")]
		public string AppSecret
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Wns")]
		public string AuthenticationUri
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.SecondaryUrl];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.SecondaryUrl] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		[Parameter(Mandatory = false, ParameterSetName = "Wns")]
		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		public string Sender
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationId];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationId] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		public string SenderAuthToken
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Gcm")]
		public string GcmServiceUri
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.Url];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.Url] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Proxy")]
		public string Uri
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.Url];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.Url] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Proxy")]
		public string Organization
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string SasKeyName
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationId];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationId] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string SasKeyValue
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string UriTemplate
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.UriTemplate];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.UriTemplate] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string RegistrationTemplate
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.RegistrationTemplate];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.RegistrationTemplate] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public bool? RegistrationEnabled
		{
			get
			{
				return (bool?)base.Fields[PushNotificationAppSchema.RegistrationEnabled];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.RegistrationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public bool? MultifactorRegistrationEnabled
		{
			get
			{
				return (bool?)base.Fields[PushNotificationAppSchema.MultifactorRegistrationEnabled];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.MultifactorRegistrationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public string PartitionName
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.PartitionName];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.PartitionName] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureSend")]
		public bool? IsDefaultPartitionName
		{
			get
			{
				return (bool?)base.Fields[PushNotificationAppSchema.IsDefaultPartitionName];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.IsDefaultPartitionName] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsUserName
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationId];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationId] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsUserPassword
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.AuthenticationKey] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsUriTemplate
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.Url];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.Url] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AzureHubCreation")]
		public string AcsScopeUriTemplate
		{
			get
			{
				return (string)base.Fields[PushNotificationAppSchema.SecondaryUrl];
			}
			set
			{
				base.Fields[PushNotificationAppSchema.SecondaryUrl] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetApp(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 357, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\PushNotifications\\SetPushNotificationApp.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			PushNotificationApp dataObject = (PushNotificationApp)base.PrepareDataObject();
			if (base.ParameterSetName == "Apns")
			{
				this.SetIfModified<string>(PushNotificationAppSchema.AuthenticationKey, delegate(string x)
				{
					dataObject.AuthenticationKey = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.AuthenticationKeyFallback, delegate(string x)
				{
					dataObject.AuthenticationKeyFallback = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.Url, delegate(string x)
				{
					dataObject.Url = x;
				});
				this.SetIfModified<int?>(PushNotificationAppSchema.Port, delegate(int? x)
				{
					dataObject.Port = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.SecondaryUrl, delegate(string x)
				{
					dataObject.SecondaryUrl = x;
				});
				this.SetIfModified<int?>(PushNotificationAppSchema.SecondaryPort, delegate(int? x)
				{
					dataObject.SecondaryPort = x;
				});
			}
			if (base.ParameterSetName == "Wns")
			{
				this.SetIfModified<string>(PushNotificationAppSchema.AuthenticationId, delegate(string x)
				{
					dataObject.AuthenticationId = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.SecondaryUrl, delegate(string x)
				{
					dataObject.SecondaryUrl = x;
				});
				this.SetEncryptedAuthenticationKeyIfModified(dataObject);
			}
			if (base.ParameterSetName == "Gcm")
			{
				this.SetIfModified<string>(PushNotificationAppSchema.AuthenticationId, delegate(string x)
				{
					dataObject.AuthenticationId = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.Url, delegate(string x)
				{
					dataObject.Url = x;
				});
				this.SetEncryptedAuthenticationKeyIfModified(dataObject);
			}
			if (base.ParameterSetName == "Proxy")
			{
				this.SetIfModified<string>(PushNotificationAppSchema.AuthenticationKey, delegate(string x)
				{
					dataObject.AuthenticationKey = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.Url, delegate(string x)
				{
					dataObject.Url = x;
				});
			}
			if (base.ParameterSetName == "AzureSend")
			{
				this.SetIfModified<string>(PushNotificationAppSchema.AuthenticationId, delegate(string x)
				{
					dataObject.AuthenticationId = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.Url, delegate(string x)
				{
					dataObject.Url = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.UriTemplate, delegate(string x)
				{
					dataObject.UriTemplate = x;
				});
				this.SetIfModified<bool?>(PushNotificationAppSchema.RegistrationEnabled, delegate(bool? x)
				{
					dataObject.RegistrationEnabled = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.RegistrationTemplate, delegate(string x)
				{
					dataObject.RegistrationTemplate = x;
				});
				this.SetIfModified<bool?>(PushNotificationAppSchema.MultifactorRegistrationEnabled, delegate(bool? x)
				{
					dataObject.MultifactorRegistrationEnabled = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.PartitionName, delegate(string x)
				{
					dataObject.PartitionName = x;
				});
				this.SetIfModified<bool?>(PushNotificationAppSchema.IsDefaultPartitionName, delegate(bool? x)
				{
					dataObject.IsDefaultPartitionName = x;
				});
				this.SetEncryptedAuthenticationKeyIfModified(dataObject);
			}
			if (base.ParameterSetName == "AzureHubCreation")
			{
				this.SetIfModified<string>(PushNotificationAppSchema.AuthenticationId, delegate(string x)
				{
					dataObject.AuthenticationId = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.Url, delegate(string x)
				{
					dataObject.Url = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.UriTemplate, delegate(string x)
				{
					dataObject.UriTemplate = x;
				});
				this.SetIfModified<string>(PushNotificationAppSchema.SecondaryUrl, delegate(string x)
				{
					dataObject.SecondaryUrl = x;
				});
				this.SetEncryptedAuthenticationKeyIfModified(dataObject);
			}
			return dataObject;
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

		protected override bool IsKnownException(Exception exception)
		{
			return exception is PushNotificationConfigurationException || base.IsKnownException(exception);
		}

		private void SetIfModified<T>(ADPropertyDefinition property, Action<T> setValue)
		{
			if (base.Fields.IsModified(property))
			{
				setValue((T)((object)base.Fields[property]));
			}
		}

		private void SetEncryptedAuthenticationKeyIfModified(PushNotificationApp dataObject)
		{
			if (base.Fields.IsModified(PushNotificationAppSchema.AuthenticationKey))
			{
				dataObject.IsAuthenticationKeyEncrypted = new bool?(!this.UseClearTextAuthenticationKeys);
				if (dataObject.IsAuthenticationKeyEncrypted.Value)
				{
					PushNotificationDataProtector pushNotificationDataProtector = new PushNotificationDataProtector(null);
					dataObject.AuthenticationKey = pushNotificationDataProtector.Encrypt((string)base.Fields[PushNotificationAppSchema.AuthenticationKey]);
					return;
				}
				dataObject.AuthenticationKey = (string)base.Fields[PushNotificationAppSchema.AuthenticationKey];
			}
		}
	}
}
