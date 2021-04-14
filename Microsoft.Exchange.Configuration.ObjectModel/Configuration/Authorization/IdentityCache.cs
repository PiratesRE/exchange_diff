using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal sealed class IdentityCache<T> where T : class
	{
		private IdentityCache()
		{
		}

		public static IdentityCache<T> Current
		{
			get
			{
				return IdentityCache<T>.instance;
			}
		}

		private TimeoutCache<string, T> CachedData
		{
			get
			{
				if (this.cachedData == null)
				{
					lock (this.lockObject)
					{
						this.cachedData = (this.cachedData ?? new TimeoutCache<string, T>(20, 5000, false));
					}
				}
				return this.cachedData;
			}
		}

		public bool Add(IIdentity identity, T data)
		{
			if (identity == null || data == null)
			{
				return false;
			}
			if (!identity.IsAuthenticated)
			{
				return false;
			}
			string text = IdentityCache<T>.CreateKey(identity);
			if (text == null)
			{
				return false;
			}
			this.CachedData.InsertAbsolute(text, data, AppSettings.Current.SidsCacheTimeoutInHours, new RemoveItemDelegate<string, T>(IdentityCache<T>.OnKeyToRemoveBudgetsCacheValueRemoved));
			return true;
		}

		public bool TryGetValue(IIdentity identity, out T data)
		{
			data = default(T);
			if (identity == null)
			{
				return false;
			}
			string text = IdentityCache<T>.CreateKey(identity);
			return text != null && this.CachedData.TryGetValue(text, out data);
		}

		private static string CreateKey(IIdentity identity)
		{
			try
			{
				return identity.AuthenticationType + "|" + identity.GetSafeName(true);
			}
			catch
			{
			}
			return null;
		}

		private static void OnKeyToRemoveBudgetsCacheValueRemoved(string key, T value, RemoveReason reason)
		{
			if (reason != RemoveReason.Removed)
			{
				ExTraceGlobals.RunspaceConfigTracer.TraceDebug<string, RemoveReason>(0L, "{0} is removed from budgets dictionary after timeout. Remove reason = {1}", key, reason);
			}
		}

		private static readonly IdentityCache<T> instance = new IdentityCache<T>();

		private object lockObject = new object();

		private TimeoutCache<string, T> cachedData;
	}
}
