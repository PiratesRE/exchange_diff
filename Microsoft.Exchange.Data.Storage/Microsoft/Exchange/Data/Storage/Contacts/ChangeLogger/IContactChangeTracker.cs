using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Contacts.ChangeLogger
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IContactChangeTracker
	{
		string Name { get; }

		bool ShouldLoadPropertiesForFurtherCheck(COWTriggerAction operation, string itemClass, StoreObjectId itemId, CoreItem item);

		StorePropertyDefinition[] GetProperties(StoreObjectId itemId, CoreItem item);

		bool ShouldLogContact(StoreObjectId itemId, CoreItem item);

		bool ShouldLogGroupOperation(COWTriggerAction operation, StoreSession sourceSession, StoreObjectId sourceFolderId, StoreSession destinationSession, StoreObjectId destinationFolderId, ICollection<StoreObjectId> itemIds);
	}
}
