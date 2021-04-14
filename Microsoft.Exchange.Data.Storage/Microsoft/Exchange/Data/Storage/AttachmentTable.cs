using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentTable
	{
		public static QueryResult GetQueryResult(ICoreItem coreItem, PropertyDefinition[] columns)
		{
			Util.ThrowOnNullArgument(columns, "columns");
			MapiTable mapiTable = null;
			QueryResult queryResult = null;
			bool flag = false;
			QueryResult result;
			try
			{
				StoreSession session = coreItem.Session;
				object thisObject = null;
				bool flag2 = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					mapiTable = coreItem.MapiMessage.GetAttachmentTable();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetMapiTable, ex, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("AttachmentTable.GetQueryResult. Failed to get the MapiTable from the coreItem.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetMapiTable, ex2, session, thisObject, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("AttachmentTable.GetQueryResult. Failed to get the MapiTable from the coreItem.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (session != null)
						{
							session.EndMapiCall();
							if (flag2)
							{
								session.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				queryResult = new QueryResult(mapiTable, columns, null, coreItem.Session, true);
				flag = true;
				result = queryResult;
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(queryResult);
					Util.DisposeIfPresent(mapiTable);
				}
			}
			return result;
		}
	}
}
