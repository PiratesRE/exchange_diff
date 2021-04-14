using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MailboxAdminHelper
	{
		public static List<Guid> GetOnlineDatabase(string serverFqdn, List<Guid> guidDatabases)
		{
			List<Guid> list = new List<Guid>();
			MdbStatus[] array = null;
			StoreSession storeSession = null;
			object thisObject = null;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=MSExchangeRPC", serverFqdn, null, null, null))
					{
						array = exRpcAdmin.ListMdbStatus(guidDatabases.ToArray());
					}
				}
				catch (MapiExceptionNoAccess)
				{
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetProperties, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Cannot list mailbox database status.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetProperties, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Cannot list mailbox database status.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if ((array[i].Status & MdbStatusFlags.Online) == MdbStatusFlags.Online)
					{
						list.Add(guidDatabases[i]);
					}
				}
			}
			return list;
		}
	}
}
