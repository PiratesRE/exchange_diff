using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.FailFast;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal static class ConnectedUserManager
	{
		internal static bool ShouldFailFastUserInCache(string userToken, string cacheKey, FailFastUserCacheValue cacheValue, BlockedReason blockedReason)
		{
			Logger.EnterFunction(ExTraceGlobals.FailFastModuleTracer, "ConnectedUserManager.ShouldFailFastUserInCache");
			if (blockedReason == BlockedReason.BySelf)
			{
				if (cacheValue.HitCount >= 3)
				{
					Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "User {0} is considered fail-fast because blocked reason is Self. cacheValue: {1}", new object[]
					{
						userToken,
						cacheValue
					});
					return true;
				}
				return false;
			}
			else
			{
				bool flag;
				if (!ConnectedUserManager.connectedUserCache.TryGetValue(userToken, out flag))
				{
					Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "User {0} is considered fail-fast because he is not in connected user cache.", new object[]
					{
						userToken
					});
					return true;
				}
				Logger.ExitFunction(ExTraceGlobals.FailFastModuleTracer, "ConnectedUserManager.ShouldFailFastUserInCache");
				return false;
			}
		}

		internal static void RemoveUser(string userToken)
		{
			if (ConnectedUserManager.connectedUserCache.Remove(userToken))
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "User {0} is removed from connected user cache.", new object[]
				{
					userToken
				});
			}
		}

		internal static void AddUser(string userToken)
		{
			ConnectedUserManager.connectedUserCache.InsertSliding(userToken, true, ConnectedUserManager.DefaultTimeOutForConnectedUser, new RemoveItemDelegate<string, bool>(ConnectedUserManager.OnUserRemovedFromCache));
			Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "User {0} is iserted into connected user cache.", new object[]
			{
				userToken
			});
			FailFastModule.RemotePowershellPerfCounter.ConnectedUserCacheSize.RawValue = (long)ConnectedUserManager.connectedUserCache.Count;
		}

		internal static void RefreshUser(string userToken)
		{
			bool flag;
			if (ConnectedUserManager.connectedUserCache.TryGetValue(userToken, out flag))
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "User {0} is refreshed in connected user cache.", new object[]
				{
					userToken
				});
			}
		}

		private static void OnUserRemovedFromCache(string userToken, bool value, RemoveReason reason)
		{
			FailFastModule.RemotePowershellPerfCounter.ConnectedUserCacheSize.RawValue = (long)ConnectedUserManager.connectedUserCache.Count;
			Logger.TraceDebug(ExTraceGlobals.FailFastModuleTracer, "User {0} is removed from connected user cache. Reason: {1}", new object[]
			{
				userToken,
				reason
			});
		}

		private const int ThresholdToConvertToBadUser = 3;

		private static readonly TimeoutCache<string, bool> connectedUserCache = new TimeoutCache<string, bool>(20, 5000, false);

		private static readonly TimeSpan DefaultTimeOutForConnectedUser = TimeSpan.FromMinutes(5.0);
	}
}
