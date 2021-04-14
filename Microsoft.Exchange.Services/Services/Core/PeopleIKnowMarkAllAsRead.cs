using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeopleIKnowMarkAllAsRead
	{
		public PeopleIKnowMarkAllAsRead(MailboxSession mailboxSession, StoreId folderId, string senderSmtpAddress, bool suppressReadReceipts, ITracer tracer)
		{
			ServiceCommandBase.ThrowIfNull(mailboxSession, "mailboxSession", "PeopleIKnowMarkAllAsRead.PeopleIKnowMarkAllAsRead");
			ServiceCommandBase.ThrowIfNull(folderId, "folderId", "PeopleIKnowMarkAllAsRead.PeopleIKnowMarkAllAsRead");
			ServiceCommandBase.ThrowIfNullOrEmpty(senderSmtpAddress, "senderSmtpAddress", "PeopleIKnowMarkAllAsRead.PeopleIKnowMarkAllAsRead");
			ServiceCommandBase.ThrowIfNull(tracer, "tracer", "PeopleIKnowMarkAllAsRead.PeopleIKnowMarkAllAsRead");
			this.mailboxSession = mailboxSession;
			this.folderId = folderId;
			this.senderSmtpAddress = senderSmtpAddress;
			this.suppressReadReceipts = suppressReadReceipts;
			this.tracer = tracer;
		}

		public void Execute()
		{
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				PeopleIKnowQuery.GetItemQueryFilter(this.senderSmtpAddress),
				PeopleIKnowMarkAllAsRead.UnreadMessageFilter
			});
			using (Folder folder = Folder.Bind(this.mailboxSession, this.folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, PeopleIKnowQuery.GetItemQuerySortBy(PeopleIKnowMarkAllAsRead.EmptySortBy), PeopleIKnowMarkAllAsRead.ItemQueryProperties))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(100);
					while (propertyBags != null && propertyBags.Length > 0)
					{
						this.tracer.TraceDebug<int>((long)this.GetHashCode(), "PeopleIKnowMarkAllAsRead.Execute. Unread messages count: {0}", propertyBags.Length);
						ICollection<StoreId> source = from bag in propertyBags
						select bag.GetValueOrDefault<StoreId>(ItemSchema.Id, null);
						folder.MarkAsRead(this.suppressReadReceipts, source.ToArray<StoreId>());
						propertyBags = queryResult.GetPropertyBags(100);
					}
				}
			}
		}

		internal const int ChunkSize = 100;

		private static readonly ComparisonFilter UnreadMessageFilter = new ComparisonFilter(ComparisonOperator.Equal, MessageItemSchema.IsRead, false);

		public static readonly PropertyDefinition[] ItemQueryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly SortBy[] EmptySortBy = new SortBy[0];

		private readonly MailboxSession mailboxSession;

		private readonly StoreId folderId;

		private readonly string senderSmtpAddress;

		private readonly bool suppressReadReceipts;

		private readonly ITracer tracer;
	}
}
