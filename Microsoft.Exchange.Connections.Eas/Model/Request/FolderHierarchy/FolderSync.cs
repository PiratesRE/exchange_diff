using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.FolderHierarchy
{
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderSync")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FolderSync
	{
		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }
	}
}
