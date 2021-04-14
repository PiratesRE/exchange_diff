using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "AuthServer", DefaultParameterSetName = "AuthMetadataUrlParameterSet", SupportsShouldProcess = true)]
	public sealed class NewAuthServer : NewSystemConfigurationObjectTask<AuthServer>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAuthServer(base.Name);
			}
		}

		[Parameter(ParameterSetName = "AppSecretParameterSet")]
		public string AppSecret
		{
			get
			{
				return (string)base.Fields["AppSecretParameter"];
			}
			set
			{
				base.Fields["AppSecretParameter"] = value;
			}
		}

		[Parameter(ParameterSetName = "NativeClientAuthServerParameterSet", Mandatory = true)]
		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet", Mandatory = true)]
		public string AuthMetadataUrl
		{
			get
			{
				return this.DataObject.AuthMetadataUrl;
			}
			set
			{
				this.DataObject.AuthMetadataUrl = value;
			}
		}

		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet")]
		[Parameter(ParameterSetName = "NativeClientAuthServerParameterSet")]
		public SwitchParameter TrustAnySSLCertificate { get; set; }

		[Parameter(ParameterSetName = "AppSecretParameterSet", Mandatory = true)]
		public string IssuerIdentifier
		{
			get
			{
				return this.DataObject.IssuerIdentifier;
			}
			set
			{
				this.DataObject.IssuerIdentifier = value;
			}
		}

		[Parameter(ParameterSetName = "AppSecretParameterSet", Mandatory = true)]
		public string TokenIssuingEndpoint
		{
			get
			{
				return this.DataObject.TokenIssuingEndpoint;
			}
			set
			{
				this.DataObject.TokenIssuingEndpoint = value;
			}
		}

		[Parameter(ParameterSetName = "AppSecretParameterSet")]
		public string AuthorizationEndpoint
		{
			get
			{
				return this.DataObject.AuthorizationEndpoint;
			}
			set
			{
				this.DataObject.AuthorizationEndpoint = value;
			}
		}

		[Parameter(ParameterSetName = "AppSecretParameterSet")]
		public string ApplicationIdentifier
		{
			get
			{
				return this.DataObject.ApplicationIdentifier;
			}
			set
			{
				this.DataObject.ApplicationIdentifier = value;
			}
		}

		[Parameter]
		public bool Enabled
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

		[Parameter(ParameterSetName = "AppSecretParameterSet", Mandatory = true)]
		[Parameter(ParameterSetName = "NativeClientAuthServerParameterSet", Mandatory = true)]
		public AuthServerType Type
		{
			get
			{
				return this.DataObject.Type;
			}
			set
			{
				this.DataObject.Type = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.CreateAuthServersContainer();
			AuthServer authServer = (AuthServer)base.PrepareDataObject();
			ADObjectId containerId = AuthServer.GetContainerId(this.ConfigurationSession);
			authServer.SetId(containerId.GetChildId(authServer.Name));
			if (base.Fields.IsModified("AppSecretParameter"))
			{
				if (authServer.Type != AuthServerType.Facebook && authServer.Type != AuthServerType.LinkedIn)
				{
					base.WriteError(new TaskException(Strings.ErrorInvalidAuthServerTypeValue), ErrorCategory.InvalidArgument, null);
				}
				authServer.CurrentEncryptedAppSecret = OAuthTaskHelper.EncryptSecretWithDKM(this.AppSecret, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			else if (authServer.IsModified(AuthServerSchema.AuthMetadataUrl))
			{
				if (!authServer.IsModified(AuthServerSchema.Type))
				{
					authServer.Type = AuthServerType.MicrosoftACS;
				}
				else if (authServer.Type != AuthServerType.ADFS && authServer.Type != AuthServerType.AzureAD)
				{
					base.WriteError(new TaskException(Strings.ErrorInvalidAuthServerTypeValue), ErrorCategory.InvalidArgument, null);
				}
				OAuthTaskHelper.FixAuthMetadataUrl(authServer, new Task.TaskErrorLoggingDelegate(base.WriteError));
				OAuthTaskHelper.FetchAuthMetadata(authServer, this.TrustAnySSLCertificate, true, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			OAuthTaskHelper.ValidateAuthServerRealmAndUniqueness(authServer, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			return this.DataObject;
		}

		private void CreateAuthServersContainer()
		{
			ADObjectId containerId = AuthServer.GetContainerId(this.ConfigurationSession);
			if (this.ConfigurationSession.Read<Container>(containerId) == null)
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				Container container = new Container();
				container.SetId(containerId);
				configurationSession.Save(container);
			}
		}
	}
}
