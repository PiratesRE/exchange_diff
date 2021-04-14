using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Transport.Sync.Worker.Health
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ThrottlingInfo
	{
		internal ThrottlingInfo()
		{
			this.Cache = new Dictionary<SyncResourceMonitorType, Dictionary<ResourceLoadState, TimeSpan>>();
			foreach (object obj in Enum.GetValues(typeof(SyncResourceMonitorType)))
			{
				SyncResourceMonitorType key = (SyncResourceMonitorType)obj;
				Dictionary<ResourceLoadState, TimeSpan> dictionary = new Dictionary<ResourceLoadState, TimeSpan>();
				foreach (object obj2 in Enum.GetValues(typeof(ResourceLoadState)))
				{
					ResourceLoadState key2 = (ResourceLoadState)obj2;
					dictionary.Add(key2, TimeSpan.Zero);
				}
				this.Cache.Add(key, dictionary);
			}
		}

		internal Dictionary<SyncResourceMonitorType, Dictionary<ResourceLoadState, TimeSpan>> Cache { get; private set; }

		internal TimeSpan BackOffTime
		{
			get
			{
				return this.GetBackoffTime();
			}
		}

		internal void Add(SyncResourceMonitorType monitor, ResourceLoadState health, int backoff)
		{
			Dictionary<ResourceLoadState, TimeSpan> dictionary;
			TimeSpan timeSpan;
			if (this.Cache.TryGetValue(monitor, out dictionary) && dictionary.TryGetValue(health, out timeSpan))
			{
				timeSpan = dictionary[health];
				timeSpan += TimeSpan.FromMilliseconds((double)backoff);
				this.Cache[monitor][health] = timeSpan;
			}
		}

		private TimeSpan GetBackoffTime()
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			if (this.Cache != null && this.Cache.Count != 0)
			{
				foreach (KeyValuePair<SyncResourceMonitorType, Dictionary<ResourceLoadState, TimeSpan>> keyValuePair in this.Cache)
				{
					foreach (KeyValuePair<ResourceLoadState, TimeSpan> keyValuePair2 in keyValuePair.Value)
					{
						timeSpan += keyValuePair2.Value;
					}
				}
			}
			return timeSpan;
		}
	}
}
