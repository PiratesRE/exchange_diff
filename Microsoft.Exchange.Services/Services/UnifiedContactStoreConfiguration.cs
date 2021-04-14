using System;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Services
{
	public class UnifiedContactStoreConfiguration : IUnifiedContactStoreConfiguration
	{
		internal UnifiedContactStoreConfiguration()
		{
			this.MaxImGroups = Global.GetAppSettingAsInt(UnifiedContactStoreConfiguration.MaxImGroupsAppSettingsName, 64);
			this.MaxImContacts = Global.GetAppSettingAsInt(UnifiedContactStoreConfiguration.MaxImContactsAppSettingsName, 1000);
		}

		public int MaxImGroups { get; private set; }

		public int MaxImContacts { get; private set; }

		public static readonly string MaxImGroupsAppSettingsName = "MaxImGroups";

		public static readonly string MaxImContactsAppSettingsName = "MaxImContacts";
	}
}
