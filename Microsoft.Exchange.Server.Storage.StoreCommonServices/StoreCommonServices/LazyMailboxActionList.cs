using System;
using System.Threading;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal class LazyMailboxActionList : IComponentData
	{
		private LazyMailboxActionList()
		{
			this.actions = new Action<Context, Mailbox>[LazyMailboxActionList.nextAvailableSlot];
		}

		internal static int LazyActionSlotForTest
		{
			get
			{
				return LazyMailboxActionList.lazyActionSlotForTest;
			}
		}

		internal static LazyMailboxActionList GetCachedForMailbox(MailboxState mailboxState)
		{
			return (LazyMailboxActionList)mailboxState.GetComponentData(LazyMailboxActionList.mailboxStateSlot);
		}

		internal static void Initialize()
		{
			if (LazyMailboxActionList.mailboxStateSlot == -1)
			{
				LazyMailboxActionList.mailboxStateSlot = MailboxState.AllocateComponentDataSlot(true);
			}
		}

		internal static void InitializeActionSlotForTest()
		{
			if (LazyMailboxActionList.lazyActionSlotForTest == -1)
			{
				LazyMailboxActionList.lazyActionSlotForTest = LazyMailboxActionList.AllocateSlot();
			}
		}

		public static int AllocateSlot()
		{
			return Interlocked.Increment(ref LazyMailboxActionList.nextAvailableSlot) - 1;
		}

		internal static void SetMailboxAction(int slot, MailboxState mailboxState, Action<Context, Mailbox> mailboxActionDelegate)
		{
			LazyMailboxActionList.SetMailboxAction(slot, mailboxState, mailboxActionDelegate, false);
		}

		internal static void AppendMailboxAction(int slot, MailboxState mailboxState, Action<Context, Mailbox> mailboxActionDelegate)
		{
			LazyMailboxActionList.SetMailboxAction(slot, mailboxState, mailboxActionDelegate, true);
		}

		private static void SetMailboxAction(int slot, MailboxState mailboxState, Action<Context, Mailbox> mailboxActionDelegate, bool appendToSlotList)
		{
			LazyMailboxActionList lazyMailboxActionList = (LazyMailboxActionList)mailboxState.GetComponentData(LazyMailboxActionList.mailboxStateSlot);
			if (lazyMailboxActionList == null)
			{
				lazyMailboxActionList = new LazyMailboxActionList();
				LazyMailboxActionList lazyMailboxActionList2 = (LazyMailboxActionList)mailboxState.CompareExchangeComponentData(LazyMailboxActionList.mailboxStateSlot, null, lazyMailboxActionList);
				if (lazyMailboxActionList2 != null)
				{
					lazyMailboxActionList = lazyMailboxActionList2;
				}
			}
			using (LockManager.Lock(lazyMailboxActionList.actions, LockManager.LockType.LeafMonitorLock))
			{
				if (lazyMailboxActionList.actions[slot] != null && appendToSlotList)
				{
					Action<Context, Mailbox> existingAction = lazyMailboxActionList.actions[slot];
					lazyMailboxActionList.actions[slot] = delegate(Context context, Mailbox mailbox)
					{
						existingAction(context, mailbox);
						mailboxActionDelegate(context, mailbox);
					};
				}
				else
				{
					lazyMailboxActionList.actions[slot] = mailboxActionDelegate;
				}
			}
		}

		internal static void ExecuteMailboxActions(Context context, Mailbox storeMailbox)
		{
			LazyMailboxActionList lazyMailboxActionList = (LazyMailboxActionList)storeMailbox.SharedState.GetComponentData(LazyMailboxActionList.mailboxStateSlot);
			if (lazyMailboxActionList != null)
			{
				for (int i = 0; i < lazyMailboxActionList.actions.Length; i++)
				{
					if (lazyMailboxActionList.actions[i] != null)
					{
						try
						{
							lazyMailboxActionList.actions[i](context, storeMailbox);
						}
						finally
						{
							lazyMailboxActionList.actions[i] = null;
						}
					}
				}
			}
		}

		bool IComponentData.DoCleanup(Context context)
		{
			return true;
		}

		private static int mailboxStateSlot = -1;

		private static int lazyActionSlotForTest = -1;

		private static int nextAvailableSlot;

		private Action<Context, Mailbox>[] actions;
	}
}
