using System;
using System.Security;
using System.Text;
using Microsoft.Exchange.Connections.Common;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections
{
	internal class SmtpConnectionSettings : ProtocolSpecificConnectionSettings
	{
		public SmtpConnectionSettings(Fqdn serverName, int portNumber) : base(ConnectionSettingsType.Smtp)
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
		}

		public Fqdn ServerName { get; private set; }

		public int Port { get; private set; }

		public override string ToMultiLineString(string lineSeparator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Send {0}", base.ToMultiLineString(lineSeparator));
			stringBuilder.AppendFormat("Server name={0},{1}", this.ServerName, lineSeparator);
			stringBuilder.AppendFormat("Port={0}", this.Port);
			return stringBuilder.ToString();
		}

		protected override OperationStatusCode TestUserCanLogonWithCurrentSettings(SmtpAddress email, string userName, SecureString password)
		{
			return OperationStatusCode.Success;
		}
	}
}
