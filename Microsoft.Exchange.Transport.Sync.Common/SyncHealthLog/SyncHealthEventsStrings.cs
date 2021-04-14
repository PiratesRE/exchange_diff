using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncHealthEventsStrings
	{
		public static Dictionary<SyncHealthEventsStrings.SyncHealthEvents, string> StringMap
		{
			get
			{
				if (SyncHealthEventsStrings.stringMap == null)
				{
					lock (SyncHealthEventsStrings.initLock)
					{
						if (SyncHealthEventsStrings.stringMap == null)
						{
							Type typeFromHandle = typeof(SyncHealthEventsStrings.SyncHealthEvents);
							Array values = Enum.GetValues(typeFromHandle);
							SyncHealthEventsStrings.stringMap = new Dictionary<SyncHealthEventsStrings.SyncHealthEvents, string>(values.Length);
							for (int i = 0; i < values.Length; i++)
							{
								SyncHealthEventsStrings.SyncHealthEvents syncHealthEvents = (SyncHealthEventsStrings.SyncHealthEvents)values.GetValue(i);
								string name = Enum.GetName(typeFromHandle, syncHealthEvents);
								SyncHealthEventsStrings.stringMap.Add(syncHealthEvents, name);
							}
						}
					}
				}
				return SyncHealthEventsStrings.stringMap;
			}
		}

		private static object initLock = new object();

		private static Dictionary<SyncHealthEventsStrings.SyncHealthEvents, string> stringMap;

		internal enum SyncHealthEvents
		{
			Sync,
			PolicyInducedSubscriptionDeletion,
			SubscriptionDispatch,
			SubscriptionCompletion,
			SubscriptionCreation,
			SubscriptionDeletion,
			RemoteServerHealth,
			DatabaseDiscovery,
			WorkTypeBudgets,
			MailboxNotification
		}
	}
}
