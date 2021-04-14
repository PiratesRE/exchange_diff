using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct NOTIFICATION
	{
		internal int ulEventType;

		internal int ulAlignPad;

		internal NOTIFICATION_UNION info;
	}
}
