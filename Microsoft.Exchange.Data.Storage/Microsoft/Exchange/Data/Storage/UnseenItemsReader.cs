using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnseenItemsReader : DisposableObject, IUnseenItemsReader, IDisposable
	{
		private UnseenItemsReader(IFolder inboxFolder)
		{
			ArgumentValidator.ThrowIfNull("inboxFolder", inboxFolder);
			this.inboxFolder = inboxFolder;
		}

		public static IUnseenItemsReader Create(IMailboxSession mailboxSession)
		{
			return UnseenItemsReader.Create(mailboxSession, XSOFactory.Default);
		}

		public static IUnseenItemsReader Create(IMailboxSession mailboxSession, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			IFolder folder = xsoFactory.BindToFolder(mailboxSession, defaultFolderId);
			return new UnseenItemsReader(folder);
		}

		public void LoadLastNItemReceiveDates(IMailboxSession mailboxSession)
		{
			this.lastNItemReceiveDatesUtc.Clear();
			using (IQueryResult queryResult = this.inboxFolder.IConversationItemQuery(null, UnseenItemsReader.UnseenSortBy, UnseenItemsReader.UnseenItemProperties))
			{
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(100);
				if (propertyBags.Length > 0)
				{
					foreach (IStorePropertyBag storePropertyBag in propertyBags)
					{
						ExDateTime item = storePropertyBag.GetValueOrDefault<ExDateTime>(ConversationItemSchema.ConversationLastDeliveryTime, ExDateTime.MinValue).ToUtc();
						this.lastNItemReceiveDatesUtc.Add(item);
					}
				}
			}
		}

		public int GetUnseenItemCount(ExDateTime lastVisitedDate)
		{
			ExDateTime lastVisitedDateUtc = lastVisitedDate.ToUtc();
			int num = this.lastNItemReceiveDatesUtc.FindIndex((ExDateTime itemReceiveDate) => itemReceiveDate <= lastVisitedDateUtc);
			if (num >= 0)
			{
				return num;
			}
			return this.lastNItemReceiveDatesUtc.Count;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UnseenItemsReader>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.inboxFolder != null)
			{
				this.inboxFolder.Dispose();
				this.inboxFolder = null;
			}
			base.InternalDispose(disposing);
		}

		private const int MaxUnseenItems = 100;

		private static readonly PropertyDefinition[] UnseenItemProperties = new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationId,
			ConversationItemSchema.ConversationLastDeliveryTime
		};

		private static readonly SortBy[] UnseenSortBy = new SortBy[]
		{
			new SortBy(ConversationItemSchema.ConversationLastDeliveryTime, SortOrder.Descending)
		};

		private IFolder inboxFolder;

		private List<ExDateTime> lastNItemReceiveDatesUtc = new List<ExDateTime>(100);
	}
}
