using System;
using System.Security;
using System.Text;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Pop;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections
{
	internal class PopConnectionSettings : ProtocolSpecificConnectionSettings
	{
		public PopConnectionSettings(Fqdn serverName, int portNumber, Pop3AuthenticationMechanism authentication, Pop3SecurityMechanism security) : base(ConnectionSettingsType.Pop)
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

		public Pop3AuthenticationMechanism Authentication { get; private set; }

		public Pop3SecurityMechanism Security { get; private set; }

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
			return OperationStatusCode.Success;
		}
	}
}
