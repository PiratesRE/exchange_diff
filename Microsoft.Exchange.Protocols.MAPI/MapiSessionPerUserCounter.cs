using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class MapiSessionPerUserCounter
	{
		private MapiSessionPerUserCounter()
		{
		}

		internal static void Initialize()
		{
			if (MapiSessionPerUserCounter.lockObject == null)
			{
				MapiSessionPerUserCounter.userSessionCounters = new Dictionary<SecurityIdentifier, MapiSessionPerUserCounter.UserSessionCounter>();
				MapiSessionPerUserCounter.lockObject = new object();
			}
		}

		internal static IMapiObjectCounter GetObjectCounter(string userDN, SecurityIdentifier sid, ClientType clientType)
		{
			return new MapiSessionPerUserAndClientTypeCounter(userDN, sid, clientType);
		}

		internal static IList<SecurityIdentifier> GetUsersSnapshot()
		{
			IList<SecurityIdentifier> result;
			using (LockManager.Lock(MapiSessionPerUserCounter.lockObject))
			{
				result = new List<SecurityIdentifier>(MapiSessionPerUserCounter.userSessionCounters.Keys);
			}
			return result;
		}

		internal static long GetCount(SecurityIdentifier userSid, ClientType clientType)
		{
			using (LockManager.Lock(MapiSessionPerUserCounter.lockObject))
			{
				MapiSessionPerUserCounter.UserSessionCounter userSessionCounter;
				if (MapiSessionPerUserCounter.userSessionCounters.TryGetValue(userSid, out userSessionCounter))
				{
					return userSessionCounter.GetCount(clientType);
				}
			}
			return 0L;
		}

		internal static void IncrementCount(SecurityIdentifier userSid, ClientType clientType)
		{
			using (LockManager.Lock(MapiSessionPerUserCounter.lockObject))
			{
				MapiSessionPerUserCounter.UserSessionCounter userSessionCounter;
				if (MapiSessionPerUserCounter.userSessionCounters.TryGetValue(userSid, out userSessionCounter))
				{
					userSessionCounter.IncrementCount(clientType);
				}
				else
				{
					userSessionCounter = new MapiSessionPerUserCounter.UserSessionCounter();
					userSessionCounter.IncrementCount(clientType);
					MapiSessionPerUserCounter.userSessionCounters.Add(userSid, userSessionCounter);
				}
			}
		}

		internal static void DecrementCount(SecurityIdentifier userSid, ClientType clientType)
		{
			using (LockManager.Lock(MapiSessionPerUserCounter.lockObject))
			{
				MapiSessionPerUserCounter.UserSessionCounter userSessionCounter;
				if (MapiSessionPerUserCounter.userSessionCounters.TryGetValue(userSid, out userSessionCounter))
				{
					userSessionCounter.DecrementCount(clientType);
					if (userSessionCounter.IsEmpty())
					{
						MapiSessionPerUserCounter.userSessionCounters.Remove(userSid);
					}
				}
			}
		}

		internal static bool IsClientOverQuota(SecurityIdentifier userSid, ClientType clientType, long effectiveLimitation, bool mustBeStrictlyUnderQuota, out bool needLogEvent)
		{
			needLogEvent = false;
			using (LockManager.Lock(MapiSessionPerUserCounter.lockObject))
			{
				MapiSessionPerUserCounter.UserSessionCounter userSessionCounter;
				if (MapiSessionPerUserCounter.userSessionCounters.TryGetValue(userSid, out userSessionCounter))
				{
					return userSessionCounter.IsClientOverQuota(clientType, effectiveLimitation, mustBeStrictlyUnderQuota, out needLogEvent);
				}
			}
			return false;
		}

		private static Dictionary<SecurityIdentifier, MapiSessionPerUserCounter.UserSessionCounter> userSessionCounters;

		private static object lockObject;

		private class UserSessionCounter
		{
			internal UserSessionCounter()
			{
				this.lastReportTime = DateTime.UtcNow.AddMonths(-1);
			}

			internal bool IsEmpty()
			{
				for (int i = 0; i < this.counters.Length; i++)
				{
					if (this.counters[i] != 0L)
					{
						return false;
					}
				}
				return true;
			}

			internal long GetCount(ClientType clientType)
			{
				return this.counters[(int)this.CountTypeFromClientType(clientType)];
			}

			internal void IncrementCount(ClientType clientType)
			{
				this.counters[(int)this.CountTypeFromClientType(clientType)] += 1L;
			}

			internal void DecrementCount(ClientType clientType)
			{
				this.counters[(int)this.CountTypeFromClientType(clientType)] -= 1L;
			}

			internal bool IsClientOverQuota(ClientType clientType, long effectiveLimitation, bool mustBeStrictlyUnderQuota, out bool needLogEvent)
			{
				needLogEvent = false;
				long num = this.counters[(int)this.CountTypeFromClientType(clientType)];
				bool flag = mustBeStrictlyUnderQuota ? (num >= effectiveLimitation) : (num > effectiveLimitation);
				if (flag)
				{
					if (DateTime.UtcNow - this.lastReportTime > MapiSessionPerUserCounter.UserSessionCounter.eventLogInterval)
					{
						needLogEvent = true;
						this.lastReportTime = DateTime.UtcNow;
					}
					DiagnosticContext.TraceDwordAndString((LID)57936U, (uint)(effectiveLimitation & (long)((ulong)-1)), clientType.ToString());
					DiagnosticContext.TraceDword((LID)33360U, (uint)num);
				}
				return flag;
			}

			private MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType CountTypeFromClientType(ClientType clientType)
			{
				switch (clientType)
				{
				case ClientType.Administrator:
					return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.Administrator;
				case ClientType.User:
				case ClientType.Transport:
				case ClientType.EventBasedAssistants:
				case ClientType.RpcHttp:
				case ClientType.ContentIndexing:
				case ClientType.Monitoring:
					break;
				case ClientType.AirSync:
					return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.AirSync;
				case ClientType.OWA:
					return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.OWA;
				case ClientType.Imap:
					return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.Imap;
				case ClientType.AvailabilityService:
				case ClientType.Management:
				case ClientType.WebServices:
					return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.WebServices;
				case ClientType.ELC:
					return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.ELC;
				case ClientType.UnifiedMessaging:
					return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.UnifiedMessaging;
				default:
					if (clientType == ClientType.Pop)
					{
						return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.Pop;
					}
					break;
				}
				return MapiSessionPerUserCounter.UserSessionCounter.MapiSessionPerUserCountType.MoMT;
			}

			private static TimeSpan eventLogInterval = TimeSpan.FromMinutes(30.0);

			private DateTime lastReportTime;

			private long[] counters = new long[9];

			private enum MapiSessionPerUserCountType
			{
				MoMT,
				AirSync,
				OWA,
				Pop,
				Imap,
				UnifiedMessaging,
				WebServices,
				ELC,
				Administrator,
				MaxValue
			}
		}
	}
}
