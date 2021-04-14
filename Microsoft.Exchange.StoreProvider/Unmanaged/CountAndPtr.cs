using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct CountAndPtr
	{
		internal int count;

		internal IntPtr intPtr;
	}
}
