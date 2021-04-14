using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.AAD
{
	internal sealed class AadDevice
	{
		public bool? AccountEnabled { get; set; }

		public bool? IsManaged { get; set; }

		public bool? IsCompliant { get; set; }

		public Guid? DeviceId { get; set; }

		public string DisplayName { get; set; }

		public List<string> ExchangeActiveSyncIds { get; set; }

		public DateTime LastUpdated { get; set; }
	}
}
