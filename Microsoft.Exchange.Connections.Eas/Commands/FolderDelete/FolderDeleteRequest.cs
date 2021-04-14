using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderDelete
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlRoot(ElementName = "FolderDelete", Namespace = "FolderHierarchy", IsNullable = false)]
	public class FolderDeleteRequest : FolderDelete
	{
	}
}
