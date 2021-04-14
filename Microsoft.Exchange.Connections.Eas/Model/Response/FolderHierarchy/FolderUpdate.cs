using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderUpdate")]
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderUpdate")]
	public class FolderUpdate
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }
	}
}
