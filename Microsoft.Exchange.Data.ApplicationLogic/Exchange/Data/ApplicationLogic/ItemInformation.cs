using System;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class ItemInformation
	{
		public string Id { get; set; }

		public byte[] Data { get; set; }

		public Exception Error { get; set; }
	}
}
