using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderCreate
{
	[XmlRoot(ElementName = "FolderCreate", Namespace = "FolderHierarchy", IsNullable = false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FolderCreateRequest : FolderCreate
	{
	}
}
