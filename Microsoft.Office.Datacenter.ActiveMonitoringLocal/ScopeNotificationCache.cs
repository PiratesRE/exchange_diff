using System;
using System.Collections.Concurrent;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal sealed class ScopeNotificationCache
	{
		private ScopeNotificationCache()
		{
		}

		internal static ScopeNotificationCache Instance
		{
			get
			{
				if (ScopeNotificationCache.instance == null)
				{
					lock (ScopeNotificationCache.locker)
					{
						if (ScopeNotificationCache.instance == null)
						{
							ScopeNotificationCache.instance = new ScopeNotificationCache();
						}
					}
				}
				return ScopeNotificationCache.instance;
			}
		}

		internal ConcurrentDictionary<string, ScopeNotificationUploadState> ScopeNotificationUploadStates
		{
			get
			{
				if (this.scopeNotificationUploadStates == null)
				{
					this.scopeNotificationUploadStates = new ConcurrentDictionary<string, ScopeNotificationUploadState>(StringComparer.InvariantCultureIgnoreCase);
				}
				return this.scopeNotificationUploadStates;
			}
		}

		internal void AddScopeNotificationRawData(ScopeNotificationRawData data)
		{
			this.ScopeNotificationUploadStates.AddOrUpdate(data.NotificationName, new ScopeNotificationUploadState
			{
				Data = data
			}, delegate(string key, ScopeNotificationUploadState existingValue)
			{
				existingValue.Data = data;
				return existingValue;
			});
		}

		private ConcurrentDictionary<string, ScopeNotificationUploadState> scopeNotificationUploadStates;

		private static ScopeNotificationCache instance = null;

		private static object locker = new object();
	}
}
