using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal abstract class LazyLookupTimeoutCacheWithDiagnostics<K, T> : LazyLookupTimeoutCache<K, T>
	{
		protected LazyLookupTimeoutCacheWithDiagnostics(int buckets, int maxBucketSize, bool shouldCallbackOnDispose, TimeSpan slidingLiveTime, TimeSpan absoluteLiveTime) : base(buckets, maxBucketSize, shouldCallbackOnDispose, slidingLiveTime, absoluteLiveTime)
		{
		}

		protected LazyLookupTimeoutCacheWithDiagnostics(int buckets, int maxBucketSize, bool shouldCallbackOnDispose, TimeSpan absoluteLiveTime) : base(buckets, maxBucketSize, shouldCallbackOnDispose, absoluteLiveTime)
		{
		}

		protected abstract T Create(K key, ref bool shouldAdd);

		protected sealed override T CreateOnCacheMiss(K key, ref bool shouldAdd)
		{
			T result = default(T);
			lock (this)
			{
				if (this.reprievedItems.TryGetValue(key, out result))
				{
					this.LogToCommonDiagnostics("ReprieveListUsed", key, null);
					shouldAdd = false;
					return result;
				}
			}
			this.LogToCommonDiagnostics("CacheMissStart", key, null);
			T result2 = this.Create(key, ref shouldAdd);
			this.LogToCommonDiagnostics("CachMissDone", key, shouldAdd ? null : new KeyValuePair<string, object>?(new KeyValuePair<string, object>("Cached", shouldAdd.ToString())));
			return result2;
		}

		protected override void HandleRemove(K key, T value, RemoveReason reason)
		{
			this.LogToCommonDiagnostics("KeyRemoved", key, new KeyValuePair<string, object>?(new KeyValuePair<string, object>("Reason", Names<RemoveReason>.Map[(int)reason])));
			bool flag = false;
			if (reason == RemoveReason.Expired)
			{
				lock (this)
				{
					if (!this.reprievedItems.ContainsKey(key))
					{
						this.reprievedItems.Add(key, value);
						flag = true;
					}
				}
			}
			if (flag)
			{
				using (ActivityContext.SuppressThreadScope())
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.BackgroundLookup), key);
				}
			}
			base.HandleRemove(key, value, reason);
		}

		private void BackgroundLookup(object backGroundLookupArgObj)
		{
			Exception ex = null;
			K k = (K)((object)backGroundLookupArgObj);
			this.LogToCommonDiagnostics("AsyncCacheLookupStart", k, null);
			bool flag = false;
			T value = default(T);
			try
			{
				value = this.Create(k, ref flag);
			}
			catch (TrackingTransientException ex2)
			{
				ex = ex2;
			}
			catch (TrackingFatalException ex3)
			{
				ex = ex3;
			}
			catch (TransientException ex4)
			{
				ex = ex4;
			}
			catch (DataSourceOperationException ex5)
			{
				ex = ex5;
			}
			catch (DataValidationException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<K, Exception>(this.GetHashCode(), "Exception in background lookup thread, Key={0}, Exception={1}", k, ex);
				KeyValuePair<string, object> value2 = new KeyValuePair<string, object>("Exception", ex.ToString());
				this.LogToCommonDiagnostics("AsyncCacheLookupException", k, new KeyValuePair<string, object>?(value2));
				lock (this)
				{
					this.reprievedItems.Remove(k);
				}
				return;
			}
			lock (this)
			{
				this.TryPerformAdd(k, value);
				this.reprievedItems.Remove(k);
			}
			this.LogToCommonDiagnostics("AsyncCacheLookupDone", k, flag ? null : new KeyValuePair<string, object>?(new KeyValuePair<string, object>("Cached", flag.ToString())));
		}

		private void LogToCommonDiagnostics(string eventToAdd, K key, KeyValuePair<string, object>? additionalPair)
		{
			if (ServerCache.Instance.WriteToStatsLogs && ServerCache.Instance.HostId == HostId.ECPApplicationPool)
			{
				KeyValuePair<string, object>[] eventData;
				if (additionalPair != null)
				{
					eventData = new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Event", eventToAdd),
						new KeyValuePair<string, object>("Type", base.GetType().Name),
						new KeyValuePair<string, object>("Key", key.ToString()),
						additionalPair.Value
					};
				}
				else
				{
					eventData = new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Event", eventToAdd),
						new KeyValuePair<string, object>("Type", base.GetType().Name),
						new KeyValuePair<string, object>("Key", key.ToString())
					};
				}
				CommonDiagnosticsLog.Instance.LogEvent(CommonDiagnosticsLog.Source.DeliveryReportsCache, eventData);
			}
		}

		internal Dictionary<K, T> reprievedItems = new Dictionary<K, T>(64);
	}
}
