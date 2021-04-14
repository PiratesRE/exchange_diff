using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISharingSubscriptionData<TKey> : ISharingSubscriptionData
	{
		TKey Key { get; }
	}
}
