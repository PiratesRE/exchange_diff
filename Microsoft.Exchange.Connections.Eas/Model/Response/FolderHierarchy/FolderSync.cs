using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy
{
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderSync")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderSync")]
	public class FolderSync
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }

		[XmlElement(ElementName = "Changes")]
		public Changes Changes { get; set; }
	}
}
