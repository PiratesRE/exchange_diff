using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy
{
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderDelete")]
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderDelete")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FolderDelete
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }
	}
}
