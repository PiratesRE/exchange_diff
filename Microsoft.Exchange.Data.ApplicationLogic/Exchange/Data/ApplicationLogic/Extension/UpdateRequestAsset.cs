using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class UpdateRequestAsset : IAppStateRequestAsset, IDownloadAppRequestAsset
	{
		public string MarketplaceContentMarket { get; set; }

		public string ExtensionID { get; set; }

		public string MarketplaceAssetID { get; set; }

		public Version Version { get; set; }

		public RequestedCapabilities RequestedCapabilities { get; set; }

		public DisableReasonType DisableReason { get; set; }

		public bool Enabled { get; set; }

		public ExtensionInstallScope Scope { get; set; }

		public OmexConstants.AppState State { get; set; }

		public string Etoken { get; set; }
	}
}
