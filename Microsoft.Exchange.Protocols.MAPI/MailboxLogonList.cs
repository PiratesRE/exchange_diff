using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class MailboxLogonList : LinkedList<MapiLogon>, IComponentData
	{
		internal MailboxLogonList()
		{
		}

		internal static void Initialize()
		{
			if (MailboxLogonList.mapilongListSlot == -1)
			{
				MailboxLogonList.mapilongListSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		internal static LinkedListNode<MapiLogon> AddLogon(MapiLogon logon)
		{
			MailboxLogonList cacheForMailbox = MailboxLogonList.GetCacheForMailbox(logon.MapiMailbox.SharedState);
			LinkedListNode<MapiLogon> result;
			using (LockManager.Lock(cacheForMailbox))
			{
				result = cacheForMailbox.AddLast(logon);
			}
			return result;
		}

		internal static void RemoveLogon(MapiLogon logon)
		{
			MailboxLogonList cacheForMailbox = MailboxLogonList.GetCacheForMailbox(logon.MapiMailbox.SharedState);
			using (LockManager.Lock(cacheForMailbox))
			{
				cacheForMailbox.Remove(logon.NodeOfMailboxStateLogonList);
			}
		}

		internal static IList<MapiSession> GetSessionListOfMailbox(Context context, Guid mailboxGuid)
		{
			List<MapiSession> result = null;
			bool flag;
			MailboxState mailboxState;
			if (MailboxStateCache.TryGetByGuidLocked(context, mailboxGuid, MailboxCreation.DontAllow, false, false, (MailboxState state) => Context.GetMailboxLockTimeout(state, MapiContext.MailboxLockTimeout), context.Diagnostics, out flag, out mailboxState))
			{
				try
				{
					MailboxLogonList mailboxLogonList = (MailboxLogonList)mailboxState.GetComponentData(MailboxLogonList.mapilongListSlot);
					if (mailboxLogonList != null)
					{
						result = mailboxLogonList.Select(delegate(MapiLogon logon)
						{
							if (!logon.IsValid || logon.IsDisposed)
							{
								return null;
							}
							return logon.Session;
						}).ToList<MapiSession>();
					}
				}
				finally
				{
					mailboxState.ReleaseMailboxLock(false);
				}
				return result;
			}
			if (flag)
			{
				throw new StoreException((LID)38748U, ErrorCodeValue.Timeout);
			}
			throw new StoreException((LID)40400U, ErrorCodeValue.NotFound);
		}

		bool IComponentData.DoCleanup(Context context)
		{
			bool result;
			using (LockManager.Lock(this, context.Diagnostics))
			{
				result = (base.Count == 0);
			}
			return result;
		}

		private static MailboxLogonList GetCacheForMailbox(MailboxState mailboxState)
		{
			MailboxLogonList mailboxLogonList = (MailboxLogonList)mailboxState.GetComponentData(MailboxLogonList.mapilongListSlot);
			if (mailboxLogonList == null)
			{
				mailboxLogonList = new MailboxLogonList();
				MailboxLogonList mailboxLogonList2 = (MailboxLogonList)mailboxState.CompareExchangeComponentData(MailboxLogonList.mapilongListSlot, null, mailboxLogonList);
				if (mailboxLogonList2 != null)
				{
					mailboxLogonList = mailboxLogonList2;
				}
			}
			return mailboxLogonList;
		}

		private static int mapilongListSlot = -1;
	}
}
