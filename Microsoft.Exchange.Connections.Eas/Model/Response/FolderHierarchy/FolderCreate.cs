using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderCreate")]
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderCreate")]
	public class FolderCreate
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }

		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }
	}
}
