using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "AuthServer", DefaultParameterSetName = "AuthMetadataUrlParameterSet", SupportsShouldProcess = true)]
	public sealed class SetAuthServer : SetSystemConfigurationObjectTask<AuthServerIdParameter, AuthServer>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetAuthServer(this.Identity.RawIdentity);
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override AuthServerIdParameter Identity
		{
			get
			{
				return (AuthServerIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet")]
		[Parameter(ParameterSetName = "NativeClientAuthServerParameterSet")]
		public string AuthMetadataUrl
		{
			get
			{
				return (string)base.Fields[AuthServerSchema.AuthMetadataUrl];
			}
			set
			{
				base.Fields[AuthServerSchema.AuthMetadataUrl] = value;
			}
		}

		[Parameter(ParameterSetName = "AuthMetadataUrlParameterSet")]
		[Parameter(ParameterSetName = "NativeClientAuthServerParameterSet")]
		public SwitchParameter TrustAnySSLCertificate { get; set; }

		[Parameter(ParameterSetName = "RefreshAuthMetadataParameterSet")]
		public SwitchParameter RefreshAuthMetadata { get; set; }

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

		[Parameter(ParameterSetName = "AppSecretParameterSet")]
		public string IssuerIdentifier
		{
			get
			{
				return (string)base.Fields[AuthServerSchema.IssuerIdentifier];
			}
			set
			{
				base.Fields[AuthServerSchema.IssuerIdentifier] = value;
			}
		}

		[Parameter(ParameterSetName = "AppSecretParameterSet")]
		public string TokenIssuingEndpoint
		{
			get
			{
				return (string)base.Fields[AuthServerSchema.TokenIssuingEndpoint];
			}
			set
			{
				base.Fields[AuthServerSchema.TokenIssuingEndpoint] = value;
			}
		}

		[Parameter(ParameterSetName = "AppSecretParameterSet")]
		public string ApplicationIdentifier
		{
			get
			{
				return (string)base.Fields[AuthServerSchema.ApplicationIdentifier];
			}
			set
			{
				base.Fields[AuthServerSchema.ApplicationIdentifier] = value;
			}
		}

		[Parameter(ParameterSetName = "NativeClientAuthServerParameterSet")]
		public bool IsDefaultAuthorizationEndpoint
		{
			get
			{
				return (bool)base.Fields[AuthServerSchema.IsDefaultAuthorizationEndpoint];
			}
			set
			{
				base.Fields[AuthServerSchema.IsDefaultAuthorizationEndpoint] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			AuthServer authServer = (AuthServer)base.PrepareDataObject();
			if ((base.ParameterSetName == "AppSecretParameterSet" && !SetAuthServer.IsOneOfAuthServerTypes(authServer.Type, new AuthServerType[]
			{
				AuthServerType.Facebook,
				AuthServerType.LinkedIn
			})) || (base.ParameterSetName == "AuthMetadataUrlParameterSet" && !SetAuthServer.IsOneOfAuthServerTypes(authServer.Type, new AuthServerType[]
			{
				AuthServerType.MicrosoftACS,
				AuthServerType.AzureAD,
				AuthServerType.ADFS
			})) || (base.ParameterSetName == "NativeClientAuthServerParameterSet" && !SetAuthServer.IsOneOfAuthServerTypes(authServer.Type, new AuthServerType[]
			{
				AuthServerType.AzureAD,
				AuthServerType.ADFS
			})))
			{
				base.WriteError(new TaskException(Strings.ErrorAuthServerCannotSwitchType), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.IsModified("AppSecretParameter"))
			{
				authServer.CurrentEncryptedAppSecret = OAuthTaskHelper.EncryptSecretWithDKM(this.AppSecret, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(AuthServerSchema.IssuerIdentifier))
			{
				authServer.IssuerIdentifier = this.IssuerIdentifier;
			}
			if (base.Fields.IsModified(AuthServerSchema.TokenIssuingEndpoint))
			{
				authServer.TokenIssuingEndpoint = this.TokenIssuingEndpoint;
			}
			if (base.Fields.IsModified(AuthServerSchema.ApplicationIdentifier))
			{
				authServer.ApplicationIdentifier = this.ApplicationIdentifier;
			}
			if (base.Fields.IsModified(AuthServerSchema.AuthMetadataUrl))
			{
				authServer.AuthMetadataUrl = this.AuthMetadataUrl;
				OAuthTaskHelper.FixAuthMetadataUrl(authServer, new Task.TaskErrorLoggingDelegate(base.WriteError));
				OAuthTaskHelper.FetchAuthMetadata(authServer, this.TrustAnySSLCertificate, false, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
				OAuthTaskHelper.ValidateAuthServerRealmAndUniqueness(authServer, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(AuthServerSchema.IsDefaultAuthorizationEndpoint))
			{
				authServer.IsDefaultAuthorizationEndpoint = this.IsDefaultAuthorizationEndpoint;
				OAuthTaskHelper.ValidateAuthServerAuthorizationEndpoint(authServer, this.ConfigurationSession, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			return authServer;
		}

		protected override void InternalProcessRecord()
		{
			if (this.RefreshAuthMetadata)
			{
				OAuthTaskHelper.FetchAuthMetadata(this.DataObject, this.TrustAnySSLCertificate, false, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			base.InternalProcessRecord();
		}

		private static bool IsOneOfAuthServerTypes(AuthServerType thisType, params AuthServerType[] authServerTypes)
		{
			foreach (AuthServerType authServerType in authServerTypes)
			{
				if (authServerType == thisType)
				{
					return true;
				}
			}
			return false;
		}
	}
}
