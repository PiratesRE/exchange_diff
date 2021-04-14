using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationMessagePersistable : IMigrationPersistable, IMigrationSerializable
	{
		StoreObjectId MessageId { get; }
	}
}
