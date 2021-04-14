using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	public class MonadConnectionInfo : RemoteConnectionInfo
	{
		static MonadConnectionInfo()
		{
			if (ExchangeSetupContext.InstalledVersion != null)
			{
				MonadConnectionInfo.exchangeClientVersion = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					ExchangeSetupContext.InstalledVersion.Major,
					ExchangeSetupContext.InstalledVersion.Minor,
					ExchangeSetupContext.InstalledVersion.Build,
					ExchangeSetupContext.InstalledVersion.Revision
				});
			}
		}

		public MonadConnectionInfo(Uri server, PSCredential credentials) : this(server, credentials, "http://schemas.microsoft.com/powershell/Microsoft.Exchange")
		{
		}

		public MonadConnectionInfo(Uri server, PSCredential credentials, string shellUri) : this(server, credentials, shellUri, null, AuthenticationMechanism.Kerberos)
		{
		}

		public MonadConnectionInfo(Uri server, PSCredential credentials, string shellUri, string typesFile, AuthenticationMechanism authenticationMechanism) : this(server, credentials, shellUri, typesFile, authenticationMechanism, ExchangeRunspaceConfigurationSettings.SerializationLevel.Partial)
		{
		}

		public MonadConnectionInfo(Uri server, PSCredential credentials, string shellUri, string typesFile, AuthenticationMechanism authenticationMechanism, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel) : this(server, credentials, shellUri, typesFile, authenticationMechanism, serializationLevel, ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown)
		{
		}

		public MonadConnectionInfo(Uri server, PSCredential credentials, string shellUri, string typesFile, AuthenticationMechanism authenticationMechanism, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication) : this(server, credentials, shellUri, typesFile, authenticationMechanism, serializationLevel, clientApplication, string.Empty)
		{
		}

		public MonadConnectionInfo(Uri server, PSCredential credentials, string shellUri, string typesFile, AuthenticationMechanism authenticationMechanism, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string clientVersion) : this(server, credentials, shellUri, typesFile, authenticationMechanism, serializationLevel, clientApplication, clientVersion, 0, true)
		{
		}

		public MonadConnectionInfo(Uri server, PSCredential credentials, string shellUri, string typesFile, AuthenticationMechanism authenticationMechanism, ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel, ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication, string clientVersion, int maxRedirectionCount, bool skipCertificateCheck) : base(server, credentials, shellUri, typesFile, authenticationMechanism, skipCertificateCheck, maxRedirectionCount)
		{
			Uri uri = server;
			if (serializationLevel != ExchangeRunspaceConfigurationSettings.SerializationLevel.Partial)
			{
				uri = MonadConnectionInfo.AppendUriProperty(uri, "serializationLevel".ToString(), serializationLevel.ToString());
			}
			if (clientApplication != ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown)
			{
				uri = MonadConnectionInfo.AppendUriProperty(uri, "clientApplication".ToString(), clientApplication.ToString());
			}
			if (MonadConnectionInfo.exchangeClientVersion != null)
			{
				uri = MonadConnectionInfo.AppendUriProperty(uri, "ExchClientVer", MonadConnectionInfo.exchangeClientVersion);
			}
			this.serverUri = uri;
			this.clientApplication = clientApplication;
			this.serializationLevel = serializationLevel;
			this.clientVersion = clientVersion;
		}

		public MonadConnectionInfo(Uri server, string certificateThumbprint, string shellUri) : this(server, certificateThumbprint, shellUri, false)
		{
		}

		internal MonadConnectionInfo(Uri server, string certificateThumbprint, string shellUri, bool skipCerificateChecks) : base(server, certificateThumbprint, shellUri, null, AuthenticationMechanism.Default, true, 0)
		{
			this.serverUri = server;
			if (MonadConnectionInfo.exchangeClientVersion != null)
			{
				this.serverUri = MonadConnectionInfo.AppendUriProperty(this.serverUri, "ExchClientVer", MonadConnectionInfo.exchangeClientVersion);
			}
		}

		private static Uri AppendUriProperty(Uri serverUri, string propertyName, string propertyValue)
		{
			if (serverUri.Query == null || !serverUri.Query.Contains(propertyName))
			{
				StringBuilder stringBuilder = new StringBuilder(serverUri.OriginalString);
				if (serverUri.OriginalString.Contains("?"))
				{
					stringBuilder.Append(";");
				}
				else
				{
					stringBuilder.Append("?");
				}
				stringBuilder.Append(propertyName);
				stringBuilder.Append("=");
				stringBuilder.Append(propertyValue);
				serverUri = new Uri(stringBuilder.ToString());
			}
			return serverUri;
		}

		public override Uri ServerUri
		{
			get
			{
				return this.serverUri;
			}
		}

		public ExchangeRunspaceConfigurationSettings.SerializationLevel SerializationLevel
		{
			get
			{
				return this.serializationLevel;
			}
		}

		public ExchangeRunspaceConfigurationSettings.ExchangeApplication ClientApplication
		{
			get
			{
				return this.clientApplication;
			}
		}

		public string ClientVersion
		{
			get
			{
				return this.clientVersion;
			}
		}

		public const string DefaultShellUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

		private ExchangeRunspaceConfigurationSettings.SerializationLevel serializationLevel;

		private ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication;

		private Uri serverUri;

		private string clientVersion;

		private static string exchangeClientVersion;
	}
}
