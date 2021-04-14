using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "Responses")]
	public class Responses
	{
		public Responses()
		{
			this.Add = new List<AddResponse>();
			this.Change = new List<ChangeResponse>();
			this.Fetch = new List<FetchResponse>();
		}

		[XmlElement("Add")]
		public List<AddResponse> Add { get; set; }

		[XmlElement("Change")]
		public List<ChangeResponse> Change { get; set; }

		[XmlElement("Fetch")]
		public List<FetchResponse> Fetch { get; set; }
	}
}
