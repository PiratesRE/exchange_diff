using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "Commands")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Commands
	{
		public Commands()
		{
			this.Add = new List<AddCommand>();
			this.Change = new List<ChangeCommand>();
			this.Delete = new List<DeleteCommand>();
			this.SoftDelete = new List<SoftDeleteCommand>();
		}

		[XmlElement(ElementName = "Add")]
		public List<AddCommand> Add { get; set; }

		[XmlElement(ElementName = "Change")]
		public List<ChangeCommand> Change { get; set; }

		[XmlElement(ElementName = "Delete")]
		public List<DeleteCommand> Delete { get; set; }

		[XmlElement(ElementName = "SoftDelete")]
		public List<SoftDeleteCommand> SoftDelete { get; set; }
	}
}
