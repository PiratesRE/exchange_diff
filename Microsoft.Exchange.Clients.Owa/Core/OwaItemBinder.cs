using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaItemBinder : StoreSession.IItemBinder
	{
		public OwaItemBinder(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
		}

		public Item BindItem(StoreObjectId itemId, bool isPublic, StoreObjectId folderId)
		{
			OwaStoreObjectId owaStoreObjectId = OwaStoreObjectId.CreateFromItemId(itemId, OwaStoreObjectId.CreateFromFolderId(folderId, isPublic ? OwaStoreObjectIdType.PublicStoreFolder : OwaStoreObjectIdType.MailBoxObject));
			try
			{
				return Utilities.GetItem<Item>(this.userContext, owaStoreObjectId, new PropertyDefinition[0]);
			}
			catch (StoragePermanentException)
			{
			}
			catch (StorageTransientException)
			{
			}
			catch (OwaPermanentException)
			{
			}
			catch (OwaTransientException)
			{
			}
			return null;
		}

		private UserContext userContext;
	}
}
