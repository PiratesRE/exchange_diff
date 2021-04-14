using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SPropertyRestriction
	{
		internal int relop;

		internal int ulPropTag;

		internal unsafe SPropValue* lpProp;
	}
}
