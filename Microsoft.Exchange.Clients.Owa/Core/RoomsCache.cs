using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class RoomsCache : RecipientCache
	{
		private RoomsCache(UserContext userContext, UserConfiguration configuration) : base(userContext, 15, configuration)
		{
		}

		public static void FinishSession(UserContext userContext)
		{
			if (userContext.IsRoomsCacheSessionStarted)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.RoomsCache", userContext))
				{
					RoomsCache roomsCache = RoomsCache.TryGetCache(userContext);
					if (roomsCache != null)
					{
						roomsCache.FinishSession(new RoomsCache(userContext, recipientCacheTransaction.Configuration), recipientCacheTransaction.Configuration);
					}
				}
			}
		}

		private static RoomsCache GetCache(UserContext userContext, bool useInMemCache)
		{
			if (userContext.RoomsCache == null || !useInMemCache)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.RoomsCache", userContext))
				{
					userContext.RoomsCache = RoomsCache.Load(userContext, recipientCacheTransaction.Configuration);
					if (!userContext.IsRoomsCacheSessionStarted)
					{
						userContext.RoomsCache.StartCacheSession();
						userContext.RoomsCache.Commit(recipientCacheTransaction.Configuration);
						userContext.IsRoomsCacheSessionStarted = true;
					}
				}
			}
			return userContext.RoomsCache;
		}

		public static RoomsCache TryGetCache(UserContext userContext)
		{
			return RoomsCache.TryGetCache(userContext, true);
		}

		public static RoomsCache TryGetCache(UserContext userContext, bool useInMemCache)
		{
			try
			{
				return RoomsCache.GetCache(userContext, useInMemCache);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceError<string, string>(0L, "Failed to get rooms cache from server. Error: {0}. Stack: {1}", ex.Message, ex.StackTrace);
			}
			return null;
		}

		public override void Commit(bool mergeBeforeCommit)
		{
			if (base.IsDirty)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.RoomsCache", base.UserContext))
				{
					RecipientCache backEndRecipientCache = null;
					if (mergeBeforeCommit)
					{
						backEndRecipientCache = new RoomsCache(base.UserContext, recipientCacheTransaction.Configuration);
					}
					this.Commit(backEndRecipientCache, recipientCacheTransaction.Configuration);
				}
			}
		}

		private static RoomsCache Load(UserContext userContext, UserConfiguration configuration)
		{
			return new RoomsCache(userContext, configuration);
		}

		public const string ConfigurationName = "OWA.RoomsCache";

		private const short Size = 15;
	}
}
