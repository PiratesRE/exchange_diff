using System;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public class Slot
	{
		public string Key { get; set; }

		public string Version { get; set; }

		public bool Removed { get; set; }
	}
}
