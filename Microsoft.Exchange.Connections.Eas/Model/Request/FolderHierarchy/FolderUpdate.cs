using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.FolderHierarchy
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderUpdate")]
	public class FolderUpdate
	{
		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }

		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }

		[XmlElement(ElementName = "ParentId")]
		public string ParentId { get; set; }

		[XmlElement(ElementName = "DisplayName")]
		public string DisplayName { get; set; }
	}
}
