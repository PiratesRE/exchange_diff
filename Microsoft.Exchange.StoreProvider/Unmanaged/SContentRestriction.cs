using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SContentRestriction
	{
		internal int ulFuzzyLevel;

		internal int ulPropTag;

		internal unsafe SPropValue* lpProp;
	}
}
