using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy
{
	[XmlType(Namespace = "FolderHierarchy", TypeName = "Update")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Update
	{
		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }

		[XmlElement(ElementName = "ParentId")]
		public string ParentId { get; set; }

		[XmlElement(ElementName = "DisplayName")]
		public string DisplayName { get; set; }

		[XmlElement(ElementName = "Type")]
		public int Type { get; set; }
	}
}
