using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Properties
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IProperty<TItem, TPropertyType> : IWriteableProperty<TItem, TPropertyType>, IReadableProperty<TItem, TPropertyType>
	{
	}
}
