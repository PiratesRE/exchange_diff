using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal static class QueryStringUtilities
	{
		public static StoreObjectId CreateStoreObjectId(MailboxSession mailboxSession, HttpRequest httpRequest, string idParameter, bool required)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(httpRequest, idParameter, required);
			if (string.IsNullOrEmpty(queryStringParameter))
			{
				return null;
			}
			return Utilities.CreateStoreObjectId(mailboxSession, queryStringParameter);
		}

		public static StoreObjectId CreateItemStoreObjectId(MailboxSession mailboxSession, HttpRequest httpRequest, bool required)
		{
			return QueryStringUtilities.CreateStoreObjectId(mailboxSession, httpRequest, "id", required);
		}

		public static StoreObjectId CreateItemStoreObjectId(MailboxSession mailboxSession, HttpRequest httpRequest)
		{
			return QueryStringUtilities.CreateStoreObjectId(mailboxSession, httpRequest, "id", true);
		}

		public static StoreObjectId CreateFolderStoreObjectId(MailboxSession mailboxSession, HttpRequest httpRequest, bool required)
		{
			return QueryStringUtilities.CreateStoreObjectId(mailboxSession, httpRequest, "id", required);
		}

		public static StoreObjectId CreateFolderStoreObjectId(MailboxSession mailboxSession, HttpRequest httpRequest)
		{
			return QueryStringUtilities.CreateStoreObjectId(mailboxSession, httpRequest, "id", true);
		}

		public const string ItemMainIdParameter = "id";

		public const string FolderMainIdParameter = "id";

		public const string OwnerItemIdParameter = "oId";

		public const string OwnerItemChangeKeyParameter = "oCk";

		public const string OwnerItemTypeParameter = "oT";

		public const string OwnerItemStateParameter = "oS";
	}
}
