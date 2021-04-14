using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Serializable]
	public class DurationInfo
	{
		[XmlIgnore]
		[IgnoreDataMember]
		public TimeSpan Duration { get; set; }

		[DataMember(Name = "Name")]
		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "DurationTicks", IsRequired = false)]
		[XmlElement(ElementName = "DurationTicks")]
		public long DurationTicks
		{
			get
			{
				return this.Duration.Ticks;
			}
			set
			{
				this.Duration = new TimeSpan(value);
			}
		}
	}
}
