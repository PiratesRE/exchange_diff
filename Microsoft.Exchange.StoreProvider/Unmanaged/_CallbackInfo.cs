using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _CallbackInfo
	{
		internal int cb;

		internal unsafe byte* pb;

		internal long id;
	}
}
