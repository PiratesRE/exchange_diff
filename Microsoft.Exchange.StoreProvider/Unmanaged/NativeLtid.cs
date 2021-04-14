using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct NativeLtid
	{
		internal Guid replGuid;

		internal long globCountAndPadding;
	}
}
