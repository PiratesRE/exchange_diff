using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorPersistable : IAnchorSerializable
	{
		StoreObjectId StoreObjectId { get; }
	}
}
