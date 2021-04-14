using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class ACSTokenCache : IDisposable
	{
		internal ACSTokenCache()
		{
			this.dictionary = new ConcurrentDictionary<string, TimeoutCache<string, ACSTokenBuildRequest>>();
			this.cancellationTokenSource = new CancellationTokenSource();
			this.getTokenRequestQueue = new BlockingCollection<ACSTokenBuildRequest>();
			this.getTokenAsyncTask = Task.Factory.StartNew(delegate()
			{
				ExTraceGlobals.OAuthTracer.TraceDebug((long)this.GetHashCode(), "[ACSTokenCache:GetTokenAsyncTask] GetToken async task started");
				foreach (ACSTokenBuildRequest acstokenBuildRequest in this.getTokenRequestQueue.GetConsumingEnumerable(this.cancellationTokenSource.Token))
				{
					try
					{
						acstokenBuildRequest.RefreshTokenIfNeed();
						OAuthCommon.PerfCounters.NumberOfPendingAuthServerRequests.RawValue = (long)this.getTokenRequestQueue.Count;
					}
					catch (Exception ex)
					{
						ExTraceGlobals.OAuthTracer.TraceError<Exception>((long)this.GetHashCode(), "[ACSTokenCache:GetTokenAsyncTask] hit exception: {0}", ex);
						ExWatson.SendReport(ex, ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash | ReportOptions.DoNotFreezeThreads, string.Empty);
					}
				}
			}, this.cancellationTokenSource.Token);
		}

		public static ACSTokenCache Instance
		{
			get
			{
				if (ACSTokenCache.instance == null)
				{
					lock (ACSTokenCache.lockObj)
					{
						if (ACSTokenCache.instance == null)
						{
							ACSTokenCache.instance = new ACSTokenCache();
						}
					}
				}
				return ACSTokenCache.instance;
			}
		}

		public TokenResult GetActorToken(ACSTokenBuildRequest tokenBuildRequest, IOutboundTracer tracer, Guid? clientRequestId)
		{
			TimeoutCache<string, ACSTokenBuildRequest> orAdd = this.dictionary.GetOrAdd(tokenBuildRequest.SelfKey, (string dummyKey) => new TimeoutCache<string, ACSTokenBuildRequest>(2, ACSTokenCache.cacheSize.Value, false));
			int num = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (KeyValuePair<string, TimeoutCache<string, ACSTokenBuildRequest>> keyValuePair in this.dictionary)
			{
				num += keyValuePair.Value.Count;
				tracer.LogInformation(this.GetHashCode(), "[ACSTokenCache:GetActorToken] Each key and its counts are {0}, {1}", new object[]
				{
					keyValuePair.Key,
					keyValuePair.Value.Count
				});
			}
			tracer.LogInformation(this.GetHashCode(), "[ACSTokenCache:GetActorToken] cache size is {0}", new object[]
			{
				num
			});
			OAuthCommon.PerfCounters.AuthServerTokenCacheSize.RawValue = (long)num;
			ACSTokenBuildRequest acstokenBuildRequest;
			TimeSpan remainingTokenLifeTime;
			if (orAdd.TryGetValue(tokenBuildRequest.PartnerKey, out acstokenBuildRequest) && (remainingTokenLifeTime = acstokenBuildRequest.TokenResult.RemainingTokenLifeTime) > TimeSpan.FromMinutes(5.0))
			{
				tracer.LogInformation(this.GetHashCode(), "[ACSTokenCache:GetActorToken] found the cached ACSTokenBuildRequest, which has the token with the remaining life time of '{0}'", new object[]
				{
					remainingTokenLifeTime
				});
				if (remainingTokenLifeTime < ACSTokenLifeTime.Instance.RemaingLifetimeLimitToRefreshACSToken)
				{
					tracer.LogInformation(this.GetHashCode(), "[ACSTokenCache:GetActorToken] try to get a new token from authserver asynchronously, currently {0} requests in the queue", new object[]
					{
						this.getTokenRequestQueue.Count
					});
					acstokenBuildRequest.Tracer = tracer;
					acstokenBuildRequest.ClientRequestId = clientRequestId;
					this.getTokenRequestQueue.Add(acstokenBuildRequest);
					OAuthCommon.PerfCounters.NumberOfPendingAuthServerRequests.RawValue = (long)this.getTokenRequestQueue.Count;
				}
				stopwatch.Stop();
				OutboundProtocolLog.BeginAppend("GetCachedACSToken", "ok", stopwatch.ElapsedMilliseconds, tokenBuildRequest.Caller, clientRequestId, tokenBuildRequest.ACSTokenIssuingEndpoint, tokenBuildRequest.TenantId, tokenBuildRequest.Resource, null, null, string.Format("CacheSize:{0}", num), remainingTokenLifeTime, acstokenBuildRequest.TokenResult);
				return acstokenBuildRequest.TokenResult;
			}
			tracer.LogInformation(this.GetHashCode(), "[ACSTokenCache:GetActorToken] try to get a new ACS token synchronously", new object[0]);
			tokenBuildRequest.Tracer = tracer;
			tokenBuildRequest.ClientRequestId = clientRequestId;
			tokenBuildRequest.BuildToken(true);
			orAdd.InsertSliding(tokenBuildRequest.PartnerKey, tokenBuildRequest, ACSTokenLifeTime.Instance.ACSTokenCacheSlidingExpiration, null);
			return tokenBuildRequest.TokenResult;
		}

		public void Dispose()
		{
			if (this.cancellationTokenSource != null)
			{
				this.cancellationTokenSource.Cancel();
				this.cancellationTokenSource.Dispose();
				this.cancellationTokenSource = null;
			}
			if (this.getTokenAsyncTask != null)
			{
				try
				{
					this.getTokenAsyncTask.Wait();
				}
				catch (AggregateException)
				{
				}
				this.getTokenAsyncTask.Dispose();
				this.getTokenAsyncTask = null;
			}
			if (this.getTokenRequestQueue != null)
			{
				this.getTokenRequestQueue.Dispose();
				this.getTokenRequestQueue = null;
			}
		}

		public void ClearCache()
		{
			this.dictionary = new ConcurrentDictionary<string, TimeoutCache<string, ACSTokenBuildRequest>>();
			this.cancellationTokenSource = new CancellationTokenSource();
			this.getTokenRequestQueue = new BlockingCollection<ACSTokenBuildRequest>();
		}

		private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("ACSTokenCacheSize", AuthCommon.IsMultiTenancyEnabled ? 4000 : 10, ExTraceGlobals.OAuthTracer);

		private static readonly object lockObj = new object();

		private static ACSTokenCache instance = null;

		private ConcurrentDictionary<string, TimeoutCache<string, ACSTokenBuildRequest>> dictionary;

		private CancellationTokenSource cancellationTokenSource;

		private BlockingCollection<ACSTokenBuildRequest> getTokenRequestQueue;

		private Task getTokenAsyncTask;
	}
}
