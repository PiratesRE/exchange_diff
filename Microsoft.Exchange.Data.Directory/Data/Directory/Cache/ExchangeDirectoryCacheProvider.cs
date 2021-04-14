using System;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Diagnostics;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.DirectoryCache;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal class ExchangeDirectoryCacheProvider : IDirectoryCacheProvider
	{
		public ExchangeDirectoryCacheProvider()
		{
			ExchangeDirectoryCacheProvider.InitializeProxyPoolIfRequired();
		}

		public TObject Get<TObject>(DirectoryCacheRequest cacheRequest) where TObject : ADRawEntry, new()
		{
			ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
			CacheUtils.PopulateAndCheckObjectType<TObject>(cacheRequest);
			SimpleADObject sADO = null;
			int retryCount = 0;
			long apiTime = 0L;
			Stopwatch stopwatch = Stopwatch.StartNew();
			ExchangeDirectoryCacheProvider.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IDirectoryCacheClient> proxy)
			{
				Stopwatch stopwatch3 = Stopwatch.StartNew();
				try
				{
					retryCount++;
					this.IsNewProxyObject = string.IsNullOrEmpty(proxy.Tag);
					GetObjectContext @object = proxy.Client.GetObject(cacheRequest);
					sADO = @object.Object;
					this.ResultState = @object.ResultState;
					CachePerformanceTracker.AddPerfData(Operation.WCFBeginOperation, @object.BeginOperationLatency);
					CachePerformanceTracker.AddPerfData(Operation.WCFEndOperation, @object.EndOperationLatency);
					proxy.Tag = DateTime.UtcNow.ToString();
				}
				finally
				{
					stopwatch3.Stop();
					CachePerformanceTracker.AddPerfData(Operation.WCFGetOperation, stopwatch3.ElapsedMilliseconds);
					apiTime = stopwatch3.ElapsedMilliseconds;
				}
			}, string.Format("Get Object {0}", typeof(TObject).FullName), 3);
			stopwatch.Stop();
			CachePerformanceTracker.AddPerfData(Operation.WCFProxyObjectCreation, stopwatch.ElapsedMilliseconds - apiTime);
			this.RetryCount = retryCount;
			TObject result = default(TObject);
			if (sADO != null)
			{
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				CachePerformanceTracker.AddPerfData(Operation.DataSize, (long)sADO.Data.Length);
				sADO.Initialize(false);
				stopwatch2.Stop();
				CachePerformanceTracker.AddPerfData(Operation.ObjectInitialization, stopwatch2.ElapsedMilliseconds);
				stopwatch2.Restart();
				if (!typeof(TObject).Equals(typeof(ADRawEntry)))
				{
					result = SimpleADObject.CreateFrom<TObject>(sADO, null, cacheRequest.ADPropertiesRequested);
				}
				else
				{
					result = (TObject)((object)SimpleADObject.CreateFrom(sADO, cacheRequest.ADPropertiesRequested));
				}
				stopwatch2.Stop();
				CachePerformanceTracker.AddPerfData(Operation.ObjectCreation, stopwatch2.ElapsedMilliseconds);
			}
			return result;
		}

		public void Put(AddDirectoryCacheRequest cacheRequest)
		{
			ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
			int retryCount = 0;
			long apiTime = 0L;
			Stopwatch stopwatch = Stopwatch.StartNew();
			ExchangeDirectoryCacheProvider.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IDirectoryCacheClient> proxy)
			{
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				try
				{
					retryCount++;
					this.IsNewProxyObject = string.IsNullOrEmpty(proxy.Tag);
					CacheResponseContext cacheResponseContext = proxy.Client.PutObject(cacheRequest);
					proxy.Tag = DateTime.UtcNow.ToString();
					CachePerformanceTracker.AddPerfData(Operation.WCFBeginOperation, cacheResponseContext.BeginOperationLatency);
					CachePerformanceTracker.AddPerfData(Operation.WCFEndOperation, cacheResponseContext.EndOperationLatency);
				}
				finally
				{
					stopwatch2.Stop();
					CachePerformanceTracker.AddPerfData(Operation.WCFPutOperation, stopwatch2.ElapsedMilliseconds);
					apiTime = stopwatch2.ElapsedMilliseconds;
				}
			}, string.Format("Adding {0} object to cache", cacheRequest.ObjectType), 3);
			stopwatch.Stop();
			CachePerformanceTracker.AddPerfData(Operation.WCFProxyObjectCreation, stopwatch.ElapsedMilliseconds - apiTime);
			this.RetryCount = retryCount;
		}

		public void Remove(RemoveDirectoryCacheRequest cacheRequest)
		{
			ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
			int retryCount = 0;
			long apiTime = 0L;
			Stopwatch stopwatch = Stopwatch.StartNew();
			ExchangeDirectoryCacheProvider.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IDirectoryCacheClient> proxy)
			{
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				try
				{
					retryCount++;
					this.IsNewProxyObject = string.IsNullOrEmpty(proxy.Tag);
					CacheResponseContext cacheResponseContext = proxy.Client.RemoveObject(cacheRequest);
					proxy.Tag = DateTime.UtcNow.ToString();
					CachePerformanceTracker.AddPerfData(Operation.WCFBeginOperation, cacheResponseContext.BeginOperationLatency);
					CachePerformanceTracker.AddPerfData(Operation.WCFEndOperation, cacheResponseContext.EndOperationLatency);
				}
				finally
				{
					stopwatch2.Stop();
					CachePerformanceTracker.AddPerfData(Operation.WCFRemoveOperation, stopwatch2.ElapsedMilliseconds);
					apiTime = stopwatch2.ElapsedMilliseconds;
				}
			}, string.Format("Removing {0} object from cache", cacheRequest.ObjectType), 3);
			stopwatch.Stop();
			CachePerformanceTracker.AddPerfData(Operation.WCFProxyObjectCreation, stopwatch.ElapsedMilliseconds - apiTime);
			this.RetryCount = retryCount;
		}

		public ADCacheResultState ResultState { get; private set; }

		public bool IsNewProxyObject { get; private set; }

		public int RetryCount { get; private set; }

		public void TestOnlyResetAllCaches()
		{
			if (!Globals.IsDatacenter)
			{
				return;
			}
			ExchangeDirectoryCacheProvider.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IDirectoryCacheClient> proxy)
			{
				proxy.Client.Diagnostic(new DiagnosticsCacheRequest());
			}, string.Format("Removing object from cache", new object[0]), 3);
		}

		private static void InitializeProxyPoolIfRequired()
		{
			if (ExchangeDirectoryCacheProvider.proxyPool == null)
			{
				lock (ExchangeDirectoryCacheProvider.lockObject)
				{
					if (ExchangeDirectoryCacheProvider.proxyPool == null)
					{
						NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
						netNamedPipeBinding.OpenTimeout = new TimeSpan(0, 0, 15);
						netNamedPipeBinding.ReceiveTimeout = new TimeSpan(0, 0, 15);
						netNamedPipeBinding.SendTimeout = new TimeSpan(0, 0, 15);
						netNamedPipeBinding.CloseTimeout = new TimeSpan(0, 0, 15);
						netNamedPipeBinding.MaxReceivedMessageSize = 10485760L;
						netNamedPipeBinding.MaxBufferSize = 10485760;
						ExchangeDirectoryCacheProvider.proxyPool = DirectoryServiceProxyPool<IDirectoryCacheClient>.CreateDirectoryServiceProxyPool(string.Format("ADCache {0}", Globals.ProcessAppName), new EndpointAddress("net.pipe://localhost/DirectoryCache/service.svc"), ExTraceGlobals.WCFClientEndpointTracer, 100, netNamedPipeBinding, (Exception x, string y) => new ADTransientException(new LocalizedString(x.Message), x), (Exception x, string y) => new PermanentGlobalException(new LocalizedString(x.Message), x), DirectoryEventLogConstants.Tuple_CannotContactADCacheService, false);
					}
				}
			}
		}

		private static DirectoryServiceProxyPool<IDirectoryCacheClient> proxyPool = null;

		private static readonly object lockObject = new object();
	}
}
