using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMessageChangePartial : IDisposable
	{
		PropertyGroupMapping PropertyGroupMapping { get; }

		IEnumerable<int> ChangedPropGroups { get; }

		IEnumerable<PropertyTag> OtherGroupPropTags { get; }
	}
}
