using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SendersInMailFolder
	{
		public SendersInMailFolder(Folder folder)
		{
			ArgumentValidator.ThrowIfNull("folder", folder);
			this.folder = folder;
		}

		public QueryResult GetQueryResultGroupedBySender(PropertyDefinition[] properties)
		{
			return this.folder.GroupedItemQuery(null, ItemQueryType.None, SendersInMailFolder.GroupAndSortBySmtpAddress, 0, null, properties);
		}

		public IEnumerable<IStorePropertyBag> GetSendersFromMailFolder(PropertyDefinition[] properties)
		{
			QueryResult queryResult = this.GetQueryResultGroupedBySender(properties);
			for (;;)
			{
				IStorePropertyBag[] results = queryResult.GetPropertyBags(10000);
				if (results == null || results.Length == 0)
				{
					break;
				}
				foreach (IStorePropertyBag result in results)
				{
					yield return result;
				}
			}
			yield break;
			yield break;
		}

		private readonly Folder folder;

		private static readonly GroupByAndOrder[] GroupAndSortBySmtpAddress = new GroupByAndOrder[]
		{
			new GroupByAndOrder(MessageItemSchema.SenderSmtpAddress, new GroupSort(MessageItemSchema.SenderSmtpAddress, SortOrder.Ascending, Aggregate.Min))
		};
	}
}
