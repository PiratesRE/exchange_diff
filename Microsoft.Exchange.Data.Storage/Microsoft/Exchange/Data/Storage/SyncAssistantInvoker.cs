using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SyncAssistantInvoker
	{
		public static void SyncFolder(MailboxSession mailboxSession, StoreObjectId folderId)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(folderId, "folderId");
			using (AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn))
			{
				try
				{
					assistantsRpcClient.StartWithParams("CalendarSyncAssistant", mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, mailboxSession.MailboxOwner.MailboxInfo.GetDatabaseGuid(), folderId.ToHexEntryId());
				}
				catch (RpcException arg)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, StoreObjectId, RpcException>(0L, "{0}: SyncAssistantInvoker.SyncFolder for folder id {1} failed with exception {2}.", mailboxSession.MailboxOwner, folderId, arg);
				}
			}
		}

		public static bool MailboxServerSupportsSync(MailboxSession mailboxSession)
		{
			ServerVersion a = new ServerVersion(mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion);
			return ServerVersion.Compare(a, SyncAssistantInvoker.OnlyCasSyncVersion) > 0;
		}

		public const string CalendarSyncAssistant = "CalendarSyncAssistant";

		private static readonly ServerVersion OnlyCasSyncVersion = new ServerVersion(14, 1, 138, 0);
	}
}
