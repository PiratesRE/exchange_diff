using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderSync
{
	[XmlRoot(ElementName = "FolderSync", Namespace = "FolderHierarchy", IsNullable = false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FolderSyncRequest : FolderSync
	{
		[XmlIgnore]
		internal static FolderSyncRequest InitialSyncRequest { get; set; } = new FolderSyncRequest
		{
			SyncKey = "0"
		};

		internal const string PrimingFolderSyncKey = "0";
	}
}
