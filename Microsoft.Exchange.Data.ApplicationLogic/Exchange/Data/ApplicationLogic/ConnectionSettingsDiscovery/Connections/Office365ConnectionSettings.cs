using System;
using System.Security;
using System.Text;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections
{
	internal class Office365ConnectionSettings : ProtocolSpecificConnectionSettings
	{
		public Office365ConnectionSettings() : base(ConnectionSettingsType.Office365)
		{
		}

		public Office365ConnectionSettings(MiniRecipient adUser) : base(ConnectionSettingsType.Office365)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser", "The adUser cannot be null.");
			}
			this.AdUser = adUser;
		}

		public MiniRecipient AdUser { get; private set; }

		public bool IsSameAccount(Office365ConnectionSettings otherConnectionSettings)
		{
			return otherConnectionSettings != null && otherConnectionSettings.AdUser != null && this.AdUser != null && this.AdUser.DistinguishedName.Equals(otherConnectionSettings.AdUser.DistinguishedName, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToMultiLineString(string lineSeparator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Get {0}", base.ToMultiLineString(lineSeparator));
			return stringBuilder.ToString();
		}

		protected override OperationStatusCode TestUserCanLogonWithCurrentSettings(SmtpAddress email, string userName, SecureString password)
		{
			return OperationStatusCode.Success;
		}
	}
}
