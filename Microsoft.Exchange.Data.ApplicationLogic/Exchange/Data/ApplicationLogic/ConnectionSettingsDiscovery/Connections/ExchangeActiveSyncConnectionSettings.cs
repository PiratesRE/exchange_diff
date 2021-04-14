using System;
using System.Net;
using System.Security;
using System.Text;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections
{
	internal class ExchangeActiveSyncConnectionSettings : ProtocolSpecificConnectionSettings
	{
		public ExchangeActiveSyncConnectionSettings() : base(ConnectionSettingsType.ExchangeActiveSync)
		{
		}

		public string EndpointAddressOverride { get; set; }

		public override string ToMultiLineString(string lineSeparator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Get {0}", base.ToMultiLineString(lineSeparator));
			return stringBuilder.ToString();
		}

		protected override OperationStatusCode TestUserCanLogonWithCurrentSettings(SmtpAddress email, string userName, SecureString password)
		{
			EasAuthenticationParameters authenticationParameters = new EasAuthenticationParameters(new NetworkCredential(userName, password), email.Local, email.Domain, string.IsNullOrEmpty(this.EndpointAddressOverride) ? null : this.EndpointAddressOverride);
			IEasConnection easConnection = EasConnection.CreateInstance(this.connectionParameters, authenticationParameters, this.deviceParameters);
			return easConnection.TestLogon();
		}

		private const string DeviceId = "REALM_DISCOVERY0";

		private const string DeviceType = "RealmDiscoveryEasDeviceType";

		private const string UserAgent = "ExchangeTestConnectionSettingsAgent";

		private readonly EasConnectionParameters connectionParameters = new EasConnectionParameters(new UniquelyNamedObject(), new NullLog(), EasProtocolVersion.Version140, false, false, null);

		private readonly EasDeviceParameters deviceParameters = new EasDeviceParameters("REALM_DISCOVERY0", "RealmDiscoveryEasDeviceType", "ExchangeTestConnectionSettingsAgent", "");
	}
}
