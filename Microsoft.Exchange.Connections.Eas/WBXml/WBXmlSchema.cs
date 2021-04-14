using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class WBXmlSchema
	{
		protected WBXmlSchema(int airSyncVersion)
		{
			this.Version = airSyncVersion;
		}

		internal int Version { get; set; }

		internal abstract string GetName(int tag);

		internal abstract string GetNameSpace(int tag);

		internal abstract int GetTag(string nameSpace, string name);

		internal abstract bool IsTagSecure(int tag);

		internal abstract bool IsTagAnOpaqueBlob(int tag);
	}
}
