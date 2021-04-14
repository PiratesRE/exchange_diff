using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ItemInformation
	{
		public ItemId Id { get; set; }

		public byte[] Data { get; set; }

		public ExportException Error { get; set; }
	}
}
