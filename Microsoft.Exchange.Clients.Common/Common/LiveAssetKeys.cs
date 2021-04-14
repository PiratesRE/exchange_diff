using System;

namespace Microsoft.Exchange.Clients.Common
{
	internal static class LiveAssetKeys
	{
		public static string GetAssetKeyString(LiveAssetKey assetKey)
		{
			return LiveAssetKeys.liveAssetKeyMap[(int)assetKey];
		}

		private static readonly string[] liveAssetKeyMap = new string[]
		{
			"Live.Shared.MarketInfo.Header.HideTabs",
			"Live.Shared.MarketInfo.Header.Tabs",
			"Live.Shared.GlobalSettings.Items.Cobrand.Jewel.Header",
			"Live.Shared.GlobalSettings.Header.Cobrand.OpenLinksInNewWindow"
		};
	}
}
