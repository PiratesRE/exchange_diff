using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	public static class Globals
	{
		public static void Initialize()
		{
			if (Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationCompletionPort == null)
			{
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationCompletionPort = NativeMethods.CreateIoCompletionPort(new SafeFileHandle(new IntPtr(-1), true), IoCompletionPort.InvalidHandle, new UIntPtr(0U), 0U);
				NotificationContext.AssignCompletionPort(Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationCompletionPort, 1U);
			}
			if (Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask == null)
			{
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask = new CompletionNotificationTask<object>(new CompletionNotificationTask<object>.CompletionNotificationCallback(MapiRpc.PumpNotificationsTask), null, Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationCompletionPort, (uint)Microsoft.Exchange.Server.Storage.MapiDisp.Globals.NotificationPumpingInterval.TotalMilliseconds, false);
			}
			MailboxCleanup.Initialize();
		}

		public static void Terminate()
		{
			PoolRpcServer.Terminate();
			MapiRpc.Terminate();
			if (Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask != null)
			{
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask.WaitForCompletion();
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask.Dispose();
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask = null;
			}
			if (Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationCompletionPort != null)
			{
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationCompletionPort.Dispose();
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationCompletionPort = null;
			}
		}

		public static void StartAllTasks()
		{
			Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask.Start();
		}

		public static void DisableTokenSingleInstancingForTest()
		{
			Microsoft.Exchange.Server.Storage.MapiDisp.Globals.IsTokenSingleInstancingEnabled = false;
		}

		public static void EnableTokenSingleInstancingForTest()
		{
			Microsoft.Exchange.Server.Storage.MapiDisp.Globals.IsTokenSingleInstancingEnabled = true;
		}

		public static void SignalStopToAllTasks()
		{
			NotificationContext.AssignCompletionPort(null, 0U);
			if (Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask != null)
			{
				Microsoft.Exchange.Server.Storage.MapiDisp.Globals.notificationPumpTask.Stop();
			}
		}

		public static void DatabaseMounting(Context context, StoreDatabase database)
		{
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
			if (!database.IsReadOnly)
			{
				MailboxCleanup.MountedEventHandler(context, database);
			}
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
			Microsoft.Exchange.Server.Storage.MapiDisp.Globals.ForEachSession(delegate(MapiSession session, Func<bool> shouldCallbackContinue)
			{
				if (!session.IsDisposed)
				{
					session.RequestClose();
				}
			});
		}

		public static void ForEachSession(Task<MapiSession>.Callback enumCallback)
		{
			Microsoft.Exchange.Server.Storage.MapiDisp.Globals.ForEachSession(enumCallback, () => true);
		}

		public static void ForEachSession(Task<MapiSession>.Callback enumCallback, Func<bool> shouldCallbackContinue)
		{
			if (MapiRpc.Instance != null)
			{
				IEnumerable<MapiSession> sessionListSnapshot = MapiRpc.Instance.GetSessionListSnapshot();
				foreach (MapiSession mapiSession in sessionListSnapshot)
				{
					mapiSession.LockSession(false);
					try
					{
						if (mapiSession.IsValid)
						{
							enumCallback(mapiSession, shouldCallbackContinue);
						}
					}
					finally
					{
						mapiSession.UnlockSession();
					}
				}
			}
		}

		public static void DeregisterAllSessionssOfMailbox(MapiContext context, Guid mdbGuid, Guid mailboxGuid)
		{
			IList<MapiSession> list = null;
			StoreDatabase storeDatabase = Storage.FindDatabase(mdbGuid);
			if (storeDatabase == null)
			{
				throw new StoreException((LID)48592U, ErrorCodeValue.NotFound);
			}
			using (context.AssociateWithDatabase(storeDatabase))
			{
				if (!storeDatabase.IsOnlineActive)
				{
					throw new StoreException((LID)64976U, ErrorCodeValue.MdbNotInitialized);
				}
				list = MailboxLogonList.GetSessionListOfMailbox(context, mailboxGuid);
			}
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				MapiSession mapiSession = list[i];
				if (mapiSession != null)
				{
					mapiSession.LockSession(false);
					try
					{
						if (mapiSession.IsValid && !mapiSession.IsDisposed)
						{
							mapiSession.RequestClose();
						}
					}
					finally
					{
						mapiSession.UnlockSession();
					}
				}
			}
		}

		private static readonly TimeSpan NotificationPumpingInterval = TimeSpan.FromSeconds(10.0);

		private static CompletionNotificationTask<object> notificationPumpTask;

		private static IoCompletionPort notificationCompletionPort;

		internal static bool IsTokenSingleInstancingEnabled = true;
	}
}
