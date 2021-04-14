using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public class TopologyInfo
	{
		public bool IsConfigured { get; set; }

		public bool IsAllMembersVersionCompatible { get; set; }

		public string Name { get; set; }

		public string[] Members { get; set; }
	}
}
