using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderUpdate
{
	[XmlRoot(ElementName = "FolderUpdate", Namespace = "FolderHierarchy", IsNullable = false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FolderUpdateRequest : FolderUpdate
	{
	}
}
