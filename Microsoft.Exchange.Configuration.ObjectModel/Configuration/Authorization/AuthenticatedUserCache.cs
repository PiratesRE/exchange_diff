using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class AuthenticatedUserCache : TimeoutCache<string, AuthZPluginUserToken>
	{
		private AuthenticatedUserCache() : base(20, 5000, false)
		{
		}

		internal static AuthenticatedUserCache Instance
		{
			get
			{
				return AuthenticatedUserCache.instance;
			}
		}

		internal static string CreateKeyForPsws(SecurityIdentifier userSid, Microsoft.Exchange.Configuration.Core.AuthenticationType authenticationType, PartitionId partitionId)
		{
			ExAssert.RetailAssert(userSid != null, "The user sid is invalid (null).");
			return string.Format("{0}:{1}:{2}", authenticationType, partitionId, userSid.Value);
		}

		internal void AddUserToCache(string key, AuthZPluginUserToken userToken)
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, string>(0L, "Add user token {0} with key {1} to cache.", userToken.UserName, key.ToString());
			base.InsertAbsolute(key, userToken, AuthenticatedUserCache.DefaultTimeOut, new RemoveItemDelegate<string, AuthZPluginUserToken>(this.OnUserRemovedFromCache));
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.AuthenticatedUserCacheSize.RawValue = (long)base.Count;
			}
		}

		private void OnUserRemovedFromCache(string key, AuthZPluginUserToken userToken, RemoveReason reason)
		{
			ExTraceGlobals.PublicPluginAPITracer.TraceDebug<string, string, RemoveReason>(0L, "User token {0} with key {1} removed from cache. Reason: {2}.", userToken.UserName, key.ToString(), reason);
			RemotePowershellPerformanceCountersInstance remotePowershellPerfCounter = ExchangeAuthorizationPlugin.RemotePowershellPerfCounter;
			if (remotePowershellPerfCounter != null)
			{
				remotePowershellPerfCounter.AuthenticatedUserCacheSize.RawValue = (long)base.Count;
			}
		}

		private const string CacheKeyStringFormat = "{0}:{1}";

		private const string UserNameFormatForCertDetails = "ISS:{0};SUB:{1}";

		private const string PswsCacheKeyStringFormat = "{0}:{1}:{2}";

		private static readonly AuthenticatedUserCache instance = new AuthenticatedUserCache();

		private static readonly TimeSpan DefaultTimeOut = new TimeSpan(0, 5, 0);
	}
}
