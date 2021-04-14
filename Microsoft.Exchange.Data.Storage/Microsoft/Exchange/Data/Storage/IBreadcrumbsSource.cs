using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IBreadcrumbsSource
	{
		IStorePropertyBag BackwardMessagePropertyBag { get; }

		Dictionary<StoreObjectId, List<IStorePropertyBag>> ForwardConversationRootMessagePropertyBags { get; }
	}
}
