using System;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class AutoCompleteCache : RecipientCache
	{
		private AutoCompleteCache(UserContext userContext, UserConfiguration configuration) : base(userContext, 100, configuration)
		{
		}

		private static AutoCompleteCache Load(UserContext userContext, UserConfiguration configuration)
		{
			return new AutoCompleteCache(userContext, configuration);
		}

		public void RemoveResolvedRecipients(ResolvedRecipientDetail[] resolvedRecipientDetails)
		{
			foreach (ResolvedRecipientDetail resolvedRecipientDetail in resolvedRecipientDetails)
			{
				base.RemoveEntry(resolvedRecipientDetail.RoutingAddress, resolvedRecipientDetail.ItemId);
			}
		}

		public static void FinishSession(UserContext userContext)
		{
			using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.AutocompleteCache", userContext))
			{
				AutoCompleteCache autoCompleteCache = AutoCompleteCache.TryGetCache(userContext);
				if (autoCompleteCache != null)
				{
					autoCompleteCache.FinishSession(new AutoCompleteCache(userContext, recipientCacheTransaction.Configuration), recipientCacheTransaction.Configuration);
				}
			}
		}

		private static AutoCompleteCache GetCache(UserContext userContext, bool useInMemCache)
		{
			if (userContext.AutoCompleteCache == null || !useInMemCache)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.AutocompleteCache", userContext))
				{
					userContext.AutoCompleteCache = AutoCompleteCache.Load(userContext, recipientCacheTransaction.Configuration);
					if (!userContext.IsAutoCompleteSessionStarted)
					{
						userContext.AutoCompleteCache.StartCacheSession();
						userContext.IsAutoCompleteSessionStarted = true;
					}
				}
			}
			return userContext.AutoCompleteCache;
		}

		public static AutoCompleteCache TryGetCache(UserContext userContext)
		{
			return AutoCompleteCache.TryGetCache(userContext, true);
		}

		public static AutoCompleteCache TryGetCache(UserContext userContext, bool useInMemCache)
		{
			try
			{
				return AutoCompleteCache.GetCache(userContext, useInMemCache);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceError<string, string>(0L, "Failed to get autocomplete cache from server. Error: {0}. Stack: {1}", ex.Message, ex.StackTrace);
			}
			return null;
		}

		public static void UpdateAutoCompleteCacheFromRecipientInfoList(RecipientInfoAC[] entries, UserContext userContext)
		{
			if (entries == null)
			{
				throw new ArgumentNullException("entries");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			AutoCompleteCache autoCompleteCache = AutoCompleteCache.TryGetCache(userContext);
			if (autoCompleteCache != null)
			{
				for (int i = 0; i < entries.Length; i++)
				{
					RecipientInfoCacheEntry recipientInfoCacheEntry = AutoCompleteCacheEntry.ParseClientEntry(entries[i]);
					if (recipientInfoCacheEntry != null)
					{
						autoCompleteCache.AddEntry(recipientInfoCacheEntry);
					}
				}
			}
		}

		public override void AddEntry(RecipientInfoCacheEntry newEntry)
		{
			base.AddEntry(newEntry);
			if (0 < (newEntry.RecipientFlags & 2))
			{
				RecipientCache recipientCache = RoomsCache.TryGetCache(base.UserContext);
				if (recipientCache != null)
				{
					recipientCache.AddEntry(newEntry);
				}
			}
		}

		public override void Commit(bool mergeBeforeCommit)
		{
			if (base.IsDirty)
			{
				using (RecipientCacheTransaction recipientCacheTransaction = new RecipientCacheTransaction("OWA.AutocompleteCache", base.UserContext))
				{
					AutoCompleteCache backEndRecipientCache = null;
					if (mergeBeforeCommit)
					{
						backEndRecipientCache = new AutoCompleteCache(base.UserContext, recipientCacheTransaction.Configuration);
					}
					this.Commit(backEndRecipientCache, recipientCacheTransaction.Configuration);
				}
			}
			if (base.UserContext.RoomsCache != null && base.UserContext.RoomsCache.IsDirty)
			{
				base.UserContext.RoomsCache.Commit(true);
			}
		}

		public const string ConfigurationName = "OWA.AutocompleteCache";

		public const short CacheSize = 100;
	}
}
