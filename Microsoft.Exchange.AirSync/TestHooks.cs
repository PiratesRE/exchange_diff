using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.AAD;

namespace Microsoft.Exchange.AirSync
{
	internal static class TestHooks
	{
		public static Func<GlobalSettingsPropertyDefinition, string> GlobalSettings_GetAppSetting { get; set; }

		public static Func<GlobalSettingsPropertyDefinition, object> GlobalSettings_GetFlightingSetting { get; set; }

		public static Func<GlobalSettingsPropertyDefinition, string, object> GlobalSettings_GetRegistrySetting { get; set; }

		public static Func<GlobalSettingsPropertyDefinition, object> GlobalSettings_GetVDirSetting { get; set; }

		public static Func<bool> GccUtils_AreStoredSecurityKeysValid { get; set; }

		public static Action<ExEventLog.EventTuple, string, string[]> EventLog_LogEvent { get; set; }

		public static Func<Participant, Participant> EmailAddressConverter_ADLookup { get; set; }

		public static Func<OrganizationId, IAadClient> GraphApi_GetAadClient { get; set; }
	}
}
