using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class SendFromCache : RecipientCache
	{
		private SendFromCache(UserContext userContext, UserConfiguration configuration) : base(userContext, 10, configuration)
		{
		}

		public static void FinishSession(UserContext userContext)
		{
			if (userContext.IsSendFromCacheSessionStarted)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.SendFromCache", userContext))
				{
					SendFromCache sendFromCache = SendFromCache.TryGetCache(userContext);
					if (sendFromCache != null)
					{
						sendFromCache.FinishSession(new SendFromCache(userContext, recipientCacheTransaction.Configuration), recipientCacheTransaction.Configuration);
					}
				}
			}
		}

		public static SendFromCache TryGetCache(UserContext userContext)
		{
			return SendFromCache.TryGetCache(userContext, true);
		}

		public static SendFromCache TryGetCache(UserContext userContext, bool useInMemCache)
		{
			try
			{
				return SendFromCache.GetCache(userContext, useInMemCache);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceError<string, string>(0L, "Failed to get send from cache from server. Error: {0}. Stack: {1}", ex.Message, ex.StackTrace);
			}
			return null;
		}

		private static SendFromCache GetCache(UserContext userContext, bool useInMemCache)
		{
			if (userContext.SendFromCache == null || !useInMemCache)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.SendFromCache", userContext))
				{
					userContext.SendFromCache = SendFromCache.Load(userContext, recipientCacheTransaction.Configuration);
					if (!userContext.IsSendFromCacheSessionStarted)
					{
						userContext.SendFromCache.StartCacheSession();
						userContext.SendFromCache.Commit(recipientCacheTransaction.Configuration);
						userContext.IsSendFromCacheSessionStarted = true;
					}
				}
			}
			return userContext.SendFromCache;
		}

		public void RenderToJavascript(TextWriter writer)
		{
			for (int i = 0; i < base.CacheLength; i++)
			{
				if (i > 0)
				{
					writer.Write(",");
				}
				AutoCompleteCacheEntry.RenderEntryJavascript(writer, base.CacheEntries[i]);
			}
		}

		private static SendFromCache Load(UserContext userContext, UserConfiguration configuration)
		{
			return new SendFromCache(userContext, configuration);
		}

		public override void Commit(bool mergeBeforeCommit)
		{
			if (base.IsDirty)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.SendFromCache", base.UserContext))
				{
					RecipientCache backEndRecipientCache = null;
					if (mergeBeforeCommit)
					{
						backEndRecipientCache = new SendFromCache(base.UserContext, recipientCacheTransaction.Configuration);
					}
					this.Commit(backEndRecipientCache, recipientCacheTransaction.Configuration);
				}
			}
		}

		public const string ConfigurationName = "OWA.SendFromCache";

		private const short Size = 10;
	}
}
