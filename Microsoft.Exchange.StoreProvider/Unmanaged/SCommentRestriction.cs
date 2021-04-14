using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SCommentRestriction
	{
		internal int cValues;

		internal unsafe SRestriction* lpRes;

		internal unsafe SPropValue* lpProp;
	}
}
