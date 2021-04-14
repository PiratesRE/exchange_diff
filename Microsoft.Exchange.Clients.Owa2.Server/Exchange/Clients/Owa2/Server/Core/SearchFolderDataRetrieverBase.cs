using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SearchFolderDataRetrieverBase
	{
		protected static bool IsPropertyDefined(object[] row, int index)
		{
			return row[index] != null && !(row[index] is PropertyError);
		}

		protected static T GetItemProperty<T>(object[] row, int index)
		{
			return SearchFolderDataRetrieverBase.GetItemProperty<T>(row, index, default(T));
		}

		protected static T GetItemProperty<T>(object[] row, int index, T defaultValue)
		{
			if (!SearchFolderDataRetrieverBase.IsPropertyDefined(row, index))
			{
				return defaultValue;
			}
			object obj = row[index];
			if (!(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		protected static string GetDateTimeProperty(ExTimeZone timeZone, object[] row, int index)
		{
			ExDateTime itemProperty = SearchFolderDataRetrieverBase.GetItemProperty<ExDateTime>(row, index, ExDateTime.MinValue);
			if (ExDateTime.MinValue.Equals(itemProperty))
			{
				return null;
			}
			if (timeZone == null)
			{
				throw new ArgumentNullException("timeZone");
			}
			return ExDateTimeConverter.ToOffsetXsdDateTime(itemProperty, timeZone);
		}

		protected static ItemId StoreIdToEwsItemId(StoreId storeId, MailboxId mailboxId)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, mailboxId, null);
			return new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
		}

		protected static string GetEwsId(StoreId storeId, Guid mailboxGuid)
		{
			if (storeId == null)
			{
				return null;
			}
			return StoreId.StoreIdToEwsId(mailboxGuid, storeId);
		}
	}
}
