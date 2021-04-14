using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.AirSync
{
	internal interface IOrganizationSettingsData
	{
		DeviceAccessLevel DefaultAccessLevel { get; }

		string UserMailInsert { get; }

		IList<SmtpAddress> AdminMailRecipients { get; }

		bool AllowAccessForUnSupportedPlatform { get; }

		bool IsIntuneManaged { get; }

		string OtaNotificationMailInsert { get; }

		Dictionary<string, ActiveSyncDeviceFilter> DeviceFiltering { get; }

		bool IsRulesListEmpty { get; }

		MicrosoftExchangeRecipient GetExchangeRecipient();

		DeviceAccessRuleData EvaluateDevice(DeviceAccessCharacteristic characteristic, string queryString);

		void AddOrUpdateDeviceAccessRule(ActiveSyncDeviceAccessRule deviceAccessRule);

		void RemoveDeviceAccessRule(string distinguishedName);
	}
}
