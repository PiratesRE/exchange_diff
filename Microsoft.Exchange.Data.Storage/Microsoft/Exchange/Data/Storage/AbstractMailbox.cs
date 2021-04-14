using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractMailbox : AbstractStorePropertyBag, IXSOMailbox, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag
	{
		public virtual void Save()
		{
			throw new NotImplementedException();
		}
	}
}
