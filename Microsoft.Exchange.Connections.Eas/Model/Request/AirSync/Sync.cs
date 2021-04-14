using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "Sync")]
	public class Sync
	{
		[XmlElement(ElementName = "Collections", Type = typeof(List<Collection>))]
		public List<Collection> Collections { get; set; }

		[XmlElement(ElementName = "Wait")]
		public int? Wait { get; set; }

		[XmlElement(ElementName = "HeartbeatInterval")]
		public int? HeartbeatInterval { get; set; }

		[XmlElement(ElementName = "WindowSize")]
		public string WindowSize { get; set; }

		[XmlElement(ElementName = "Partial")]
		public object Partial { get; set; }

		[XmlIgnore]
		public bool WaitSpecified
		{
			get
			{
				return this.Wait != null;
			}
		}

		[XmlIgnore]
		public bool HeartbeatIntervalSpecified
		{
			get
			{
				return this.HeartbeatInterval != null;
			}
		}
	}
}
