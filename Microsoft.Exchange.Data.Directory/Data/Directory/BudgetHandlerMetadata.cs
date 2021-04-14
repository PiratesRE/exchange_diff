using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory
{
	public class BudgetHandlerMetadata
	{
		public int OutstandingActions { get; set; }

		public string Key { get; set; }

		public string Snapshot { get; set; }

		[XmlAttribute]
		public bool Locked { get; set; }

		[XmlAttribute]
		public string LockedAt { get; set; }

		[XmlAttribute]
		public string LockedUntil { get; set; }
	}
}
