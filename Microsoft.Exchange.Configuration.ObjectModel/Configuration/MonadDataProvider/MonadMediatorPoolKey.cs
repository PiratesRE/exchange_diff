using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadMediatorPoolKey : IEquatable<MonadMediatorPoolKey>
	{
		public MonadMediatorPoolKey(MonadConnectionInfo connectionInfo, RunspaceServerSettingsPresentationObject serverSettings)
		{
			if (connectionInfo == null)
			{
				throw new ArgumentNullException("connectionInfo");
			}
			this.connectionInfo = connectionInfo;
			this.serverSettings = serverSettings;
		}

		public MonadMediatorPoolKey(MonadConnectionInfo connectionInfo) : this(connectionInfo, null)
		{
		}

		public MonadConnectionInfo ConnectionInfo
		{
			get
			{
				return this.connectionInfo;
			}
		}

		public RunspaceServerSettingsPresentationObject ServerSettings
		{
			get
			{
				return this.serverSettings;
			}
		}

		public bool Equals(MonadMediatorPoolKey other)
		{
			return other != null && (other.ConnectionInfo.SerializationLevel == this.ConnectionInfo.SerializationLevel && other.ConnectionInfo.ServerUri != null && other.ConnectionInfo.ServerUri.Equals(this.ConnectionInfo.ServerUri) && other.ConnectionInfo.ShellUri != null && other.ConnectionInfo.ShellUri.Equals(this.ConnectionInfo.ShellUri) && MonadMediatorPoolKey.EqualCredentials(this.ConnectionInfo, other.ConnectionInfo)) && ((this.serverSettings == null && other.serverSettings == null) || (other.ServerSettings != null && other.serverSettings.Equals(this.serverSettings)));
		}

		private static bool EqualCredentials(MonadConnectionInfo connInfo, MonadConnectionInfo otherConnInfo)
		{
			if (!string.IsNullOrEmpty(connInfo.CertificateThumbprint) || !string.IsNullOrEmpty(otherConnInfo.CertificateThumbprint))
			{
				return string.Equals(connInfo.CertificateThumbprint, otherConnInfo.CertificateThumbprint);
			}
			if (connInfo.AuthenticationMechanism != otherConnInfo.AuthenticationMechanism)
			{
				return false;
			}
			if (connInfo.Credentials != null && otherConnInfo.Credentials != null)
			{
				return string.Equals(connInfo.Credentials.UserName, otherConnInfo.Credentials.UserName, StringComparison.OrdinalIgnoreCase);
			}
			return connInfo.Credentials == null && otherConnInfo.Credentials == null;
		}

		private MonadConnectionInfo connectionInfo;

		private RunspaceServerSettingsPresentationObject serverSettings;
	}
}
