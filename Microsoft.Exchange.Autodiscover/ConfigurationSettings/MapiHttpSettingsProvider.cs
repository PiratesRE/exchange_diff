using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal class MapiHttpSettingsProvider
	{
		public MapiHttpSettingsProvider(MapiHttpSettingsProvider.DiscoverServiceStrategy discoveryStrategy)
		{
			this.discoveryStrategy = discoveryStrategy;
		}

		public void DiscoverSettings(HashSet<UserConfigurationSettingName> requestedSettings, string mailboxId, UserConfigurationSettings settings)
		{
			if (string.IsNullOrEmpty(mailboxId))
			{
				throw new ArgumentException("mailboxId is empty or null.", "mailboxId");
			}
			if (requestedSettings.Contains(UserConfigurationSettingName.MapiHttpUrls))
			{
				MapiHttpService mapiHttpService = this.discoveryStrategy(ClientAccessType.Internal);
				MapiHttpService mapiHttpService2 = this.discoveryStrategy(ClientAccessType.External);
				DateTime lastConfigurationTime = DateTime.MinValue;
				if ((mapiHttpService ?? mapiHttpService2) != null)
				{
					lastConfigurationTime = (mapiHttpService ?? mapiHttpService2).LastConfigurationTime;
				}
				MapiHttpProtocolUrls value = new MapiHttpProtocolUrls(this.GetUrlFromService(mapiHttpService), this.GetUrlFromService(mapiHttpService2), mailboxId, lastConfigurationTime);
				settings.Add(UserConfigurationSettingName.MapiHttpUrls, value);
			}
		}

		private Uri GetUrlFromService(MapiHttpService service)
		{
			if (service == null)
			{
				return null;
			}
			return service.Url;
		}

		private readonly MapiHttpSettingsProvider.DiscoverServiceStrategy discoveryStrategy;

		public delegate MapiHttpService DiscoverServiceStrategy(ClientAccessType clientAccessType);
	}
}
