using System;
using System.Management.Automation;
using System.Net.Sockets;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration.DataAccessLayer
{
	internal class ImapEndpoint : MigrationEndpointBase
	{
		public ImapEndpoint(MigrationEndpoint presentationObject) : base(presentationObject)
		{
		}

		public ImapEndpoint() : base(MigrationType.IMAP)
		{
		}

		public int Port
		{
			get
			{
				return base.ExtendedProperties.Get<int>("Port", (this.Security == IMAPSecurityMechanism.Ssl) ? 993 : 143);
			}
			set
			{
				base.ExtendedProperties.Set<int>("Port", value);
			}
		}

		public IMAPAuthenticationMechanism Authentication
		{
			get
			{
				switch (base.AuthenticationMethod)
				{
				case AuthenticationMethod.Basic:
					return IMAPAuthenticationMechanism.Basic;
				case AuthenticationMethod.Ntlm:
					return IMAPAuthenticationMechanism.Ntlm;
				}
				throw new AuthenticationMethodNotSupportedException(base.AuthenticationMethod.ToString(), this.PreferredMigrationType.ToString(), string.Join<AuthenticationMethod>(",", this.SupportedAuthenticationMethods));
			}
			set
			{
				if (value == IMAPAuthenticationMechanism.Basic)
				{
					base.AuthenticationMethod = AuthenticationMethod.Basic;
					return;
				}
				if (value != IMAPAuthenticationMechanism.Ntlm)
				{
					throw new AuthenticationMethodNotSupportedException(base.AuthenticationMethod.ToString(), this.PreferredMigrationType.ToString(), string.Join<AuthenticationMethod>(",", this.SupportedAuthenticationMethods));
				}
				base.AuthenticationMethod = AuthenticationMethod.Ntlm;
			}
		}

		public IMAPSecurityMechanism Security
		{
			get
			{
				return base.ExtendedProperties.Get<IMAPSecurityMechanism>("Security", IMAPSecurityMechanism.Ssl);
			}
			set
			{
				base.ExtendedProperties.Set<IMAPSecurityMechanism>("Security", value);
			}
		}

		public override ConnectionSettingsBase ConnectionSettings
		{
			get
			{
				return new IMAPConnectionSettings
				{
					Server = this.RemoteServer,
					Port = this.Port,
					Authentication = this.Authentication,
					Security = this.Security
				};
			}
		}

		public override MigrationType PreferredMigrationType
		{
			get
			{
				return MigrationType.IMAP;
			}
		}

		public override void InitializeFromAutoDiscover(SmtpAddress emailAddress, PSCredential credentials)
		{
			throw new AutoDiscoverNotSupportedException(base.EndpointType);
		}

		public override void VerifyConnectivity()
		{
			using (TcpClient tcpClient = new TcpClient())
			{
				IAsyncResult asyncResult = tcpClient.BeginConnect(this.RemoteServer, this.Port, null, null);
				if (!asyncResult.AsyncWaitHandle.WaitOne(ImapEndpoint.ConnectionTimeout))
				{
					throw new MigrationServerConnectionTimeoutException(this.RemoteServer, ImapEndpoint.ConnectionTimeout);
				}
				if (!asyncResult.IsCompleted)
				{
					throw new MigrationServerConnectionFailedException(this.RemoteServer);
				}
				try
				{
					tcpClient.EndConnect(asyncResult);
				}
				catch (SocketException innerException)
				{
					throw new MigrationServerConnectionFailedException(this.RemoteServer, innerException);
				}
			}
		}

		protected override void ApplyAutodiscoverSettings(AutodiscoverClientResponse response)
		{
			throw new AutoDiscoverNotSupportedException(base.EndpointType);
		}

		protected override void ApplyAdditionalProperties(MigrationEndpoint presentationObject)
		{
			presentationObject.Authentication = new AuthenticationMethod?(base.AuthenticationMethod);
			presentationObject.Security = new IMAPSecurityMechanism?(this.Security);
			presentationObject.Port = new int?(this.Port);
			base.ApplyAdditionalProperties(presentationObject);
		}

		protected override void InitializeFromPresentationObject(MigrationEndpoint endpoint)
		{
			base.InitializeFromPresentationObject(endpoint);
			if (endpoint.Port != null)
			{
				this.Port = endpoint.Port.Value;
				this.Security = endpoint.Security.Value;
			}
		}

		private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(15.0);
	}
}
