using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal delegate int ChangeTrackingDelegate(MailboxSession mbxsession, StoreObjectId folderId, IStorePropertyBag propertyBag);
}
