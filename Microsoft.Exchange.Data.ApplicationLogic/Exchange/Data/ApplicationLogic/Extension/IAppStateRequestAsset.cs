using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal interface IAppStateRequestAsset
	{
		string MarketplaceAssetID { get; set; }

		string MarketplaceContentMarket { get; set; }
	}
}
