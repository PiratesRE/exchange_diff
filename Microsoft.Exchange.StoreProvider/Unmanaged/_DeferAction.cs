using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _DeferAction
	{
		internal int cb;

		internal unsafe byte* lpb;
	}
}
