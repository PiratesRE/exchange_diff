using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SSizeRestriction
	{
		internal int relop;

		internal int ulPropTag;

		internal int cb;
	}
}
