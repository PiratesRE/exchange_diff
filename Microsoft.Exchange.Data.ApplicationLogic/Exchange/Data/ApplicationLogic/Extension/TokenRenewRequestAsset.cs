using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class TokenRenewRequestAsset
	{
		public string MarketplaceContentMarket { get; set; }

		public string ExtensionID { get; set; }

		public string MarketplaceAssetID { get; set; }

		public Version Version { get; set; }

		public ExtensionInstallScope Scope { get; set; }

		public string Etoken { get; set; }

		public bool IsResponseFound { get; set; }
	}
}
