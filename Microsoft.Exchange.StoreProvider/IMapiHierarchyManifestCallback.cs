using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMapiHierarchyManifestCallback
	{
		ManifestCallbackStatus Change(PropValue[] props);

		ManifestCallbackStatus Delete(byte[] data);
	}
}
