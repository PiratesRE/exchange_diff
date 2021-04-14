using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IUpdatableItem
	{
		bool UpdateWith(IUpdatableItem newItem);
	}
}
