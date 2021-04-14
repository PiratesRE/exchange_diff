using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Properties
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IWriteableProperty<TItem, TPropertyType>
	{
		void WriteProperty(TItem item, TPropertyType value);
	}
}
