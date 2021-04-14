using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct TABLE_NOTIFICATION
	{
		internal int ulTableEvent;

		internal int hResult;

		internal SPropValue propIndex;

		internal SPropValue propPrior;

		internal SRow row;

		internal int ulPad;
	}
}
