using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SPropProblem
	{
		internal int ulIndex;

		internal int ulPropTag;

		internal int scode;
	}
}
