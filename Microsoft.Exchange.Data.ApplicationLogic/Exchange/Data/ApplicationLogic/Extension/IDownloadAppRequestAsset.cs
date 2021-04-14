using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal interface IDownloadAppRequestAsset
	{
		string MarketplaceAssetID { get; set; }

		string MarketplaceContentMarket { get; set; }

		DisableReasonType DisableReason { get; set; }

		bool Enabled { get; set; }

		ExtensionInstallScope Scope { get; set; }

		string Etoken { get; set; }
	}
}
