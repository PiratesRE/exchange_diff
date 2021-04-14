using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SPropProblemArray
	{
		internal int cProblem;

		internal SPropProblem aProblem;
	}
}
