using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct EXTENDED_NOTIFICATION
	{
		internal int ulEvent;

		internal int cb;

		internal IntPtr pbEventParameters;
	}
}
