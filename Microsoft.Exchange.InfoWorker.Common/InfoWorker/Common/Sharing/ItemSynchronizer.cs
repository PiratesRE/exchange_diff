using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Sharing;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal abstract class ItemSynchronizer
	{
		public ItemSynchronizer(LocalFolder localFolder)
		{
			this.localFolder = localFolder;
		}

		public static ItemSynchronizer Create(LocalFolder localFolder, CultureInfo clientCulture)
		{
			switch (localFolder.Type)
			{
			case StoreObjectType.CalendarFolder:
				ItemSynchronizer.Tracer.TraceDebug(0L, "ItemSynchronizer.Create: Creating synchronizer for Calendar folder.");
				return new CalendarItemSynchronizer(localFolder, clientCulture);
			case StoreObjectType.ContactsFolder:
				ItemSynchronizer.Tracer.TraceDebug(0L, "ItemSynchronizer.Create: Creating synchronizer for Contacts folder.");
				return new ContactItemSynchronizer(localFolder);
			default:
				ItemSynchronizer.Tracer.TraceWarning<StoreObjectType>(0L, "ItemSynchronizer.Create: No custom synchronizer for type {0}. Creating default synchronizer.", localFolder.Type);
				return new DefaultItemSynchronizer(localFolder);
			}
		}

		public abstract void Sync(ItemType item, MailboxSession mailboxSession, ExchangeService exchangeService);

		protected abstract Item Bind(MailboxSession mailboxSession, StoreId storeId);

		public virtual void EnforceLevelOfDetails(MailboxSession mailboxSession, StoreId itemId, LevelOfDetails levelOfDetails)
		{
		}

		protected Item FindLocalCopy(string itemId, MailboxSession mailboxSession)
		{
			StoreId localIdFromRemoteId = this.localFolder.GetLocalIdFromRemoteId(itemId);
			if (localIdFromRemoteId == null)
			{
				return null;
			}
			Item item = null;
			Exception ex = null;
			try
			{
				item = this.Bind(mailboxSession, localIdFromRemoteId);
				bool flag = false;
				try
				{
					item.OpenAsReadWrite();
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						item.Dispose();
					}
				}
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (CorruptDataException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ItemSynchronizer.Tracer.TraceError<ItemSynchronizer, StoreId, Exception>((long)this.GetHashCode(), "{0}: Error binding local item {1}. Exception: {2}", this, localIdFromRemoteId, ex);
				if (ex is CorruptDataException)
				{
					this.localFolder.SelectItemToDelete(localIdFromRemoteId);
					return null;
				}
			}
			return item;
		}

		protected static readonly Trace Tracer = ExTraceGlobals.SharingEngineTracer;

		protected LocalFolder localFolder;
	}
}
