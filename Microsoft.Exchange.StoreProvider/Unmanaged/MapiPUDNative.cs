using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct MapiPUDNative
	{
		internal unsafe MapiPUDNative(NativePUD* ppud)
		{
			this.replGuid = ppud->replGuid;
			this.ltid = new MapiLtidNative(&ppud->ltid);
		}

		internal MapiLtidNative ltid;

		internal Guid replGuid;

		internal static readonly int Size = 16 + MapiLtidNative.Size;
	}
}
