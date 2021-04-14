using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class ExtensionsCacheEntry
	{
		public string ExtensionID { get; set; }

		public string MarketplaceAssetID { get; set; }

		public Version Version { get; set; }

		public RequestedCapabilities? RequestedCapabilities { get; set; }

		public OmexConstants.AppState State { get; set; }

		public byte[] Manifest { get; set; }

		public DateTime LastUpdateCheckTime { get; set; }

		public int Size { get; set; }

		public ExtensionsCacheEntry(string marketplaceAssetID, string extensionID, Version version, RequestedCapabilities? requestedCapabilities, OmexConstants.AppState state, byte[] manifest)
		{
			if (string.IsNullOrEmpty(marketplaceAssetID))
			{
				throw new ArgumentNullException("marketPlaceAssetID");
			}
			if (string.IsNullOrEmpty(extensionID))
			{
				throw new ArgumentNullException("extensionID");
			}
			this.MarketplaceAssetID = marketplaceAssetID;
			this.ExtensionID = extensionID;
			this.Version = version;
			this.RequestedCapabilities = requestedCapabilities;
			this.State = state;
			this.Manifest = manifest;
			this.SetLastUpdateCheckTime();
			this.SetSize();
		}

		private void SetSize()
		{
			int num = (this.Manifest != null) ? this.Manifest.Length : 0;
			int num2 = (this.ExtensionID != null) ? (this.ExtensionID.Length * 2) : 0;
			int num3 = 16;
			int num4 = (this.MarketplaceAssetID != null) ? (this.MarketplaceAssetID.Length * 2) : 0;
			this.Size = 8 + num + 4 + 4 + 4 + num3 + num2 + num4;
		}

		public void SetLastUpdateCheckTime()
		{
			this.LastUpdateCheckTime = DateTime.UtcNow;
		}

		public static byte[] ConvertManifestStringToBytes(string manifestString)
		{
			if (string.IsNullOrEmpty(manifestString))
			{
				throw new ArgumentNullException("manifestString");
			}
			return Encoding.UTF8.GetBytes(manifestString);
		}

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;
	}
}
