using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SSubRestriction
	{
		internal int ulSubObject;

		internal unsafe SRestriction* lpRes;
	}
}
