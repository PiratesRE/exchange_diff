using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GlobalLocatorCache;

namespace Microsoft.Exchange.Net.Mserve
{
	internal class MserveCacheServiceProvider : IDisposeTrackable, IDisposable
	{
		internal MserveCacheServiceProvider() : this("localhost")
		{
		}

		internal MserveCacheServiceProvider(string machineName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("machineName", machineName);
			ExTraceGlobals.ClientTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating new MserveCacheService provider instance to server {0}", machineName);
			this.machineName = machineName;
			this.InitializeServiceProxyPool();
			this.disposeTracker = this.GetDisposeTracker();
		}

		private void InitializeServiceProxyPool()
		{
			ExTraceGlobals.ClientTracer.TraceDebug((long)this.GetHashCode(), "MserveCacheServiceProvider - Initializing Service proxy pool");
			NetNamedPipeBinding defaultBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			EndpointAddress endpointAddress = MserveCacheServiceProvider.CreateAndConfigureMserveCacheServiceEndpoint(this.machineName);
			this.serviceProxyPool = MserveCacheServiceProxyPool<IMserveCacheService>.CreateMserveCacheServiceProxyPool("MserveCacheServiceNetPipeEndpoint", endpointAddress, ExTraceGlobals.ClientTracer, 2, defaultBinding, new GetWrappedExceptionDelegate(MserveCacheServiceProvider.GetTransientWrappedException), new GetWrappedExceptionDelegate(MserveCacheServiceProvider.GetPermanentWrappedException), CommonEventLogConstants.Tuple_CannotContactMserveCacheService, true);
		}

		private static Exception GetTransientWrappedException(Exception wcfException, string targetInfo)
		{
			if (wcfException is TimeoutException)
			{
				return new MserveCacheServiceTransientException(NetServerException.MserveCacheTimeoutError(wcfException.Message), wcfException);
			}
			if (wcfException is EndpointNotFoundException)
			{
				return new MserveCacheServiceTransientException(NetServerException.MserveCacheEndpointNotFound(targetInfo, wcfException.ToString()));
			}
			return new MserveCacheServiceTransientException(NetServerException.TransientMserveCacheError(wcfException.Message), wcfException);
		}

		private static Exception GetPermanentWrappedException(Exception wcfException, string targetInfo)
		{
			return new MserveCacheServicePermanentException(NetServerException.PermanentMserveCacheError(wcfException.Message), wcfException);
		}

		internal static EndpointAddress CreateAndConfigureMserveCacheServiceEndpoint(string machineName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("machineName", machineName);
			string uri = string.Format(MserveCacheServiceProvider.serviceEndpointFormat, machineName);
			EndpointAddress result;
			try
			{
				EndpointAddress endpointAddress = new EndpointAddress(uri);
				result = endpointAddress;
			}
			catch (UriFormatException arg)
			{
				ExTraceGlobals.ClientTracer.TraceError<string, UriFormatException>(0L, "MserveCacheServiceProvider.MserveCacheServiceProvider() - Invalid Server Name {0}.  Exception: {1}", machineName, arg);
				throw;
			}
			return result;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MserveCacheServiceProvider>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			ExTraceGlobals.ClientTracer.TraceDebug(0L, "Disposing of MserveCacheServiceProvider instance");
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.serviceProxyPool.Dispose();
			GC.SuppressFinalize(this);
		}

		internal static MserveCacheServiceProvider GetInstance()
		{
			MserveCacheServiceProvider mserveCacheServiceProvider = MserveCacheServiceProvider.staticInstance;
			if (mserveCacheServiceProvider == null)
			{
				lock (MserveCacheServiceProvider.instanceLockRoot)
				{
					if (MserveCacheServiceProvider.staticInstance == null)
					{
						MserveCacheServiceProvider.staticInstance = new MserveCacheServiceProvider();
						MserveCacheServiceProvider.staticInstance.SuppressDisposeTracker();
						mserveCacheServiceProvider = MserveCacheServiceProvider.staticInstance;
					}
				}
			}
			return mserveCacheServiceProvider;
		}

		internal static ExEventLog EventLog
		{
			get
			{
				lock (MserveCacheServiceProvider.eventLock)
				{
					if (MserveCacheServiceProvider.eventlog == null)
					{
						MserveCacheServiceProvider.eventlog = new ExEventLog(ExTraceGlobals.ClientTracer.Category, "MSExchange Common");
					}
				}
				return MserveCacheServiceProvider.eventlog;
			}
		}

		public string ReadMserveData(string requestName)
		{
			ExTraceGlobals.ClientTracer.TraceFunction(0L, "Enter MserveCacheServiceProvider.ReadMserveData().");
			string partnerId = string.Empty;
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMserveCacheService> proxy)
			{
				partnerId = proxy.Client.ReadMserveData(requestName);
			}, string.Format("ReadMserveData for {0}", requestName), 3);
			string.IsNullOrEmpty(partnerId);
			ExTraceGlobals.ClientTracer.TraceFunction(0L, "Exit MserveCacheServiceProvider.ReadMserveData().");
			return partnerId;
		}

		public int GetChunkSize()
		{
			ExTraceGlobals.ClientTracer.TraceFunction(0L, "Enter MserveCacheServiceProvider.GetChunkSize().");
			int chunkSize = 0;
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMserveCacheService> proxy)
			{
				chunkSize = proxy.Client.GetChunkSize();
			}, "GetChunkSize", 3);
			if (chunkSize != 0)
			{
				ExTraceGlobals.ClientTracer.TraceDebug<int>(0L, "Returning Chunk Size = {0}", chunkSize);
			}
			ExTraceGlobals.ClientTracer.TraceFunction(0L, "Exit MserveCacheServiceProvider.GetChunkSize().");
			return chunkSize;
		}

		private const int TotalRetries = 3;

		private const string MserveCacheServiceNetPipeEndpoint = "MserveCacheServiceNetPipeEndpoint";

		private const int MaxNumberOfClientProxies = 2;

		private static readonly TimeSpan defaultSendTimeout = TimeSpan.FromMinutes(2.0);

		private static string serviceEndpointFormat = "net.pipe://{0}/MserveCacheService/service.svc";

		private static object instanceLockRoot = new object();

		private static object eventLock = new object();

		private static MserveCacheServiceProvider staticInstance;

		private static ExEventLog eventlog;

		private readonly string machineName;

		private DisposeTracker disposeTracker;

		private MserveCacheServiceProxyPool<IMserveCacheService> serviceProxyPool;
	}
}
