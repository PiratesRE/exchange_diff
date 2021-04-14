using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public class KilledExtensionEntry
	{
		public KilledExtensionEntry(string extensionId, string assetId)
		{
			if (string.IsNullOrWhiteSpace(extensionId))
			{
				throw new ArgumentException("The extension id is missing.");
			}
			if (string.IsNullOrWhiteSpace(assetId))
			{
				throw new ArgumentException("The asset id is missing.");
			}
			this.ExtensionId = ExtensionDataHelper.FormatExtensionId(extensionId);
			this.AssetId = assetId;
		}

		public string ExtensionId { get; private set; }

		public string AssetId { get; private set; }
	}
}
