using System;
using System.Globalization;
using System.Security;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections
{
	internal class ConnectionSettings
	{
		public ConnectionSettings(IConnectionSettingsReadProvider provider, ProtocolSpecificConnectionSettings incomingSettings, SmtpConnectionSettings outgoingSettings)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider", "The provider parameter cannot be null.");
			}
			this.Initialize(provider, incomingSettings, outgoingSettings);
		}

		public ConnectionSettings(ProtocolSpecificConnectionSettings incomingSettings, SmtpConnectionSettings outgoingSettings)
		{
			this.Initialize(null, incomingSettings, outgoingSettings);
		}

		public string SourceId { get; private set; }

		public SmtpConnectionSettings OutgoingConnectionSettings { get; private set; }

		public ProtocolSpecificConnectionSettings IncomingConnectionSettings { get; private set; }

		public bool TestUserCanLogon(SmtpAddress email, ref string userName, SecureString password)
		{
			bool flag = this.IncomingConnectionSettings.TestUserCanLogon(email, ref userName, password);
			if (this.OutgoingConnectionSettings != null)
			{
				string text = userName;
				flag &= this.OutgoingConnectionSettings.TestUserCanLogon(email, ref text, password);
			}
			return flag;
		}

		public string ToMultiLineString(string lineSeparator)
		{
			return this.IncomingConnectionSettings.ToMultiLineString(lineSeparator) + ((this.OutgoingConnectionSettings != null) ? (lineSeparator + this.OutgoingConnectionSettings.ToMultiLineString(lineSeparator)) : string.Empty);
		}

		public override string ToString()
		{
			return this.ToMultiLineString(" ");
		}

		private void Initialize(IConnectionSettingsReadProvider provider, ProtocolSpecificConnectionSettings incomingSettings, SmtpConnectionSettings outgoingSettings)
		{
			if (incomingSettings == null)
			{
				throw new ArgumentNullException("incomingSettings", "The incomingSettings parameter cannot be null.");
			}
			ConnectionSettingsType connectionType = incomingSettings.ConnectionType;
			if (connectionType <= ConnectionSettingsType.ExchangeActiveSync)
			{
				if (connectionType == ConnectionSettingsType.Office365 || connectionType == ConnectionSettingsType.ExchangeActiveSync)
				{
					if (outgoingSettings != null)
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The outgoingSettings parameter is invalid (ConnectionType: {0}). It should be set to null for {1} settings.", new object[]
						{
							outgoingSettings.ConnectionType,
							incomingSettings.ConnectionType
						}));
					}
					goto IL_10B;
				}
			}
			else if (connectionType != ConnectionSettingsType.Imap && connectionType != ConnectionSettingsType.Pop)
			{
				if (connectionType == ConnectionSettingsType.Smtp)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The incomingSettings parameter cannot be {0}. That is an outgoing protocol.", new object[]
					{
						incomingSettings.ConnectionType
					}));
				}
			}
			else
			{
				if (outgoingSettings == null)
				{
					throw new ArgumentNullException(string.Format(CultureInfo.InvariantCulture, "The outgoingSettings parameter cannot be null for {0} settings. It must be Smtp.", new object[]
					{
						incomingSettings.ConnectionType
					}));
				}
				goto IL_10B;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The incommingSettings argument has an unexpected ConnectionType: {0}.", new object[]
			{
				incomingSettings.ConnectionType
			}));
			IL_10B:
			this.SourceId = ((provider == null) ? ConnectionSettings.UserSpecified : provider.SourceId);
			this.OutgoingConnectionSettings = outgoingSettings;
			this.IncomingConnectionSettings = incomingSettings;
		}

		public static string UserSpecified = "UserSpecified";
	}
}
