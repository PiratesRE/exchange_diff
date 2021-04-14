using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal static class MailboxTableQuery
	{
		internal static IDisposable SetGetPropValuesFromMailboxTableTestHook(Func<string, Guid, PropTag[], PropValue[][]> delegateFunction)
		{
			return MailboxTableQuery.getPropValuesFromMailboxTableHook.SetTestHook(delegateFunction);
		}

		internal static PropValue GetMailboxProperty(PropValue[] propValueArray, PropTag property)
		{
			foreach (PropValue result in propValueArray)
			{
				uint num = (uint)(result.PropTag & (PropTag)4294901760U);
				uint num2 = (uint)(property & (PropTag)4294901760U);
				if (num == num2)
				{
					return result;
				}
			}
			throw new ArgumentException(string.Format("Property {0} not found in the provided property set.", property));
		}

		internal static PropValue[][] GetMailboxes(string clientId, DatabaseInfo databaseInfo, PropTag[] properties)
		{
			ExTraceGlobals.DatabaseInfoTracer.TraceDebug<DatabaseInfo>(0L, "{0}: Get list of mailboxes from mailbox table.", databaseInfo);
			PropValue[][] array = MailboxTableQuery.getPropValuesFromMailboxTableHook.Value(clientId, databaseInfo.Guid, properties);
			ExTraceGlobals.DatabaseInfoTracer.TraceDebug<DatabaseInfo, int>(0L, "{0}: {1} mailboxes retrieved.", databaseInfo, array.Length);
			return array;
		}

		private static PropValue[][] GetPropValuesFromMailboxTable(string clientId, Guid databaseGuid, PropTag[] properties)
		{
			PropValue[][] mailboxTable;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create(clientId, null, null, null, null))
			{
				mailboxTable = exRpcAdmin.GetMailboxTable(databaseGuid, properties);
			}
			return mailboxTable;
		}

		private static Hookable<Func<string, Guid, PropTag[], PropValue[][]>> getPropValuesFromMailboxTableHook = Hookable<Func<string, Guid, PropTag[], PropValue[][]>>.Create(true, new Func<string, Guid, PropTag[], PropValue[][]>(MailboxTableQuery.GetPropValuesFromMailboxTable));

		internal static readonly PropTag[] RequiredMailboxTableProperties = new PropTag[]
		{
			PropTag.UserGuid,
			PropTag.DisplayName,
			PropTag.DateDiscoveredAbsentInDS,
			PropTag.MailboxMiscFlags,
			PropTag.MailboxType,
			PropTag.PersistableTenantPartitionHint,
			PropTag.MailboxTypeDetail,
			PropTag.LastLogonTime
		};
	}
}
