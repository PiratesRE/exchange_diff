using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "WlmHealthStats")]
	[Serializable]
	public sealed class WlmHealthStatistics : XMLSerializableBase
	{
		[XmlElement(ElementName = "A5M")]
		public WlmHealthStatistics.HealthAverages Avg5Min { get; set; }

		[XmlElement(ElementName = "A1H")]
		public WlmHealthStatistics.HealthAverages Avg1Hour { get; set; }

		[XmlElement(ElementName = "A1D")]
		public WlmHealthStatistics.HealthAverages Avg1Day { get; set; }

		public override string ToString()
		{
			return string.Format("A5M:[{0}]; A1H:[{1}]; A1D:[{2}]", this.Avg5Min.ToString(), this.Avg1Hour.ToString(), this.Avg1Day.ToString());
		}

		[Serializable]
		public sealed class HealthAverages : XMLSerializableBase
		{
			[XmlAttribute(AttributeName = "G")]
			public int Underloaded { get; set; }

			[XmlAttribute(AttributeName = "Y")]
			public int Full { get; set; }

			[XmlAttribute(AttributeName = "R")]
			public int Overloaded { get; set; }

			[XmlAttribute(AttributeName = "B")]
			public int Critical { get; set; }

			[XmlAttribute(AttributeName = "U")]
			public int Unknown { get; set; }

			[XmlAttribute(AttributeName = "Capacity")]
			public float AverageCapacity { get; set; }
		}
	}
}
