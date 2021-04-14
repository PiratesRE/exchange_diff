using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Properties
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReadableProperty<TItem, TPropertyType>
	{
		TPropertyType ReadProperty(TItem item);
	}
}
