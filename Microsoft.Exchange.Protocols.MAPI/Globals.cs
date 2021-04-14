using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public static class Globals
	{
		public static void Initialize()
		{
			Globals.spoolerLockSlot = MailboxState.AllocateComponentDataSlot(false);
			TimedEventDispatcher.RegisterHandler(MapiTimedEvents.EventSource, new MapiTimedEventHandler());
			MapiStreamLock.Initialize();
			MailboxLogonList.Initialize();
			ActiveObjectLimits.Initialize();
			MapiSessionPerServiceCounter.Initialize();
			MapiSessionPerUserCounter.Initialize();
			SecurityContextManager.Initialize();
			MapiLogon.Initialize();
			MapiFolder.Initialize();
		}

		public static void Terminate()
		{
			TimedEventDispatcher.UnregisterHandler(MapiTimedEvents.EventSource);
			ActiveObjectLimits.Terminate();
			SecurityContextManager.Terminate();
		}

		public static void DatabaseMounting(Context context, StoreDatabase database)
		{
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
		}

		internal static HashSet<ExchangeId> GetSpoolerLockList(Mailbox mailbox)
		{
			return mailbox.SharedState.GetComponentData(Globals.spoolerLockSlot) as HashSet<ExchangeId>;
		}

		internal static void SetSpoolerLockList(Mailbox mailbox, HashSet<ExchangeId> list)
		{
			mailbox.SharedState.SetComponentData(Globals.spoolerLockSlot, list);
		}

		internal const uint HsotNone = 4294967295U;

		internal const int MapiUnicode = -2147483648;

		private static int spoolerLockSlot;
	}
}
