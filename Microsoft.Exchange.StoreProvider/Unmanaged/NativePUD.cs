using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct NativePUD
	{
		internal NativeLtid ltid;

		internal Guid replGuid;
	}
}
