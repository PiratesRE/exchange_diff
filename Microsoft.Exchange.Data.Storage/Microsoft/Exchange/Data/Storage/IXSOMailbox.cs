using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IXSOMailbox : IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag
	{
		void Save();
	}
}
