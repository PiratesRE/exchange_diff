using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "FolderHierarchy", TypeName = "Changes")]
	public class Changes
	{
		[XmlElement(ElementName = "Count")]
		public uint Count { get; set; }

		[XmlIgnore]
		public bool CountSpecified { get; set; }

		[XmlElement(ElementName = "Add")]
		public List<Add> Additions { get; set; }

		[XmlElement(ElementName = "Update")]
		public List<Update> Updates { get; set; }

		[XmlElement(ElementName = "Delete")]
		public List<Delete> Deletions { get; set; }
	}
}
