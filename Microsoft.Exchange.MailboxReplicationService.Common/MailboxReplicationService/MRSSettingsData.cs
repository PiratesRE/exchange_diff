using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct MRSSettingsData
	{
		public string Context { get; set; }

		public string SettingName { get; set; }

		public string SettingValue { get; set; }
	}
}
