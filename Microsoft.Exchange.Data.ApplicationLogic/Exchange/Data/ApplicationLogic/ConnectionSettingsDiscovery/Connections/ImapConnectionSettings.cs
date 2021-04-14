using System;
using System.Security;
using System.Text;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Imap;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections
{
	internal class ImapConnectionSettings : ProtocolSpecificConnectionSettings
	{
		public ImapConnectionSettings(Fqdn serverName, int portNumber, ImapAuthenticationMechanism authentication, ImapSecurityMechanism security) : base(ConnectionSettingsType.Imap)
		{
			if (serverName == null)
			{
				throw new ArgumentNullException("serverName", "The serverName parameter cannot be null.");
			}
			if (portNumber < 0)
			{
				throw new ArgumentException("serverName", "The portNumber parameter must have a value greater than 0.");
			}
			this.ServerName = serverName;
			this.Port = portNumber;
			this.Authentication = authentication;
			this.Security = security;
		}

		public Fqdn ServerName { get; private set; }

		public int Port { get; private set; }

		public ImapAuthenticationMechanism Authentication { get; private set; }

		public ImapSecurityMechanism Security { get; private set; }

		public override string ToMultiLineString(string lineSeparator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Get {0}", base.ToMultiLineString(lineSeparator));
			stringBuilder.AppendFormat("Server name={0},{1}", this.ServerName, lineSeparator);
			stringBuilder.AppendFormat("Port={0},{1}", this.Port, lineSeparator);
			stringBuilder.AppendFormat("Authentication={0},{1}", this.Authentication, lineSeparator);
			stringBuilder.AppendFormat("Security={0}", this.Security);
			return stringBuilder.ToString();
		}

		protected override OperationStatusCode TestUserCanLogonWithCurrentSettings(SmtpAddress email, string userName, SecureString password)
		{
			OperationStatusCode result;
			using (ImapConnection imapConnection = ImapConnection.CreateInstance(this.connectionParameters))
			{
				ImapServerParameters serverParameters = new ImapServerParameters(this.ServerName, this.Port);
				result = imapConnection.TestLogon(serverParameters, new ImapAuthenticationParameters(userName, password, this.Authentication, this.Security), ImapConnectionSettings.requiredCapabilities);
			}
			return result;
		}

		private static readonly IServerCapabilities requiredCapabilities = new ImapServerCapabilities().Add("IMAP4REV1");

		private readonly ConnectionParameters connectionParameters = new ConnectionParameters(new UniquelyNamedObject(), new NullLog(), long.MaxValue, Convert.ToInt32(TimeSpan.FromSeconds(20.0).TotalMilliseconds));
	}
}
