using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public sealed class OmexWebServiceUrlsCache
	{
		internal static OmexWebServiceUrlsCache Singleton
		{
			get
			{
				return OmexWebServiceUrlsCache.singleton;
			}
		}

		public int MaxInitializeCompletionCallbacks
		{
			get
			{
				return this.maxInitializeCompletionCallbacks;
			}
			set
			{
				this.maxInitializeCompletionCallbacks = value;
			}
		}

		public bool IsInitialized
		{
			get
			{
				long num = (this.CacheLifeTimeFromHeaderInSeconds > 0L) ? this.CacheLifeTimeFromHeaderInSeconds : 86400L;
				return this.isInitialized && DateTime.UtcNow.Subtract(this.lastUpdatedTime).TotalSeconds < (double)num;
			}
			set
			{
				this.isInitialized = value;
				if (this.isInitialized)
				{
					this.lastUpdatedTime = DateTime.UtcNow;
				}
			}
		}

		public long CacheLifeTimeFromHeaderInSeconds { get; set; }

		public string AppStateUrl { get; set; }

		public string DownloadUrl { get; set; }

		public string KillbitUrl { get; set; }

		public string ConfigServiceUrl
		{
			get
			{
				if (this.configServiceUrl == null)
				{
					this.configServiceUrl = ExtensionData.ConfigServiceUrl;
				}
				return this.configServiceUrl;
			}
		}

		internal void Initialize(string configServiceUrl, OmexWebServiceUrlsCache.InitializeCompletionCallback initializeCompletionCallback)
		{
			if (configServiceUrl == null)
			{
				throw new ArgumentNullException("configServiceUrl");
			}
			if (configServiceUrl.Length == 0)
			{
				throw new ArgumentException("configServiceUrl is empty");
			}
			if (initializeCompletionCallback == null)
			{
				throw new ArgumentNullException("initializeCompletionCallback");
			}
			OmexWebServiceUrlsCache.Tracer.TraceDebug<string>(0L, "OmexWebServicesUrlsCache.Initialize: Setting configServiceUrl: {0}", configServiceUrl);
			this.configServiceUrl = configServiceUrl;
			this.Initialize(initializeCompletionCallback);
		}

		internal void Initialize(OmexWebServiceUrlsCache.InitializeCompletionCallback initializeCompletionCallback)
		{
			if (initializeCompletionCallback == null)
			{
				throw new ArgumentNullException("initializeCompletionCallback");
			}
			bool? flag = null;
			bool flag2 = false;
			lock (this.lockObject)
			{
				if (this.IsInitialized)
				{
					flag = new bool?(true);
				}
				else if (this.initializeCompletionCallbacks.Count + 1 > this.maxInitializeCompletionCallbacks)
				{
					OmexWebServiceUrlsCache.Tracer.TraceError(0L, "OmexWebServicesUrlsCache.Initialize: too many completion callbacks");
					flag = new bool?(false);
				}
				else if (this.ConfigServiceUrl == null)
				{
					OmexWebServiceUrlsCache.Tracer.TraceError(0L, "OmexWebServicesUrlsCache.Initialize: Config service url is null");
					flag = new bool?(false);
				}
				else
				{
					this.initializeCompletionCallbacks.Add(initializeCompletionCallback);
					if (!this.isInitializing)
					{
						flag2 = true;
						this.isInitializing = true;
					}
				}
			}
			if (flag2)
			{
				GetConfig getConfig = new GetConfig(this);
				getConfig.Execute(new GetConfig.SuccessCallback(this.CompleteInitialization), new BaseAsyncCommand.FailureCallback(this.GetConfigFailureCallback));
			}
			if (flag != null)
			{
				initializeCompletionCallback(flag.Value);
			}
		}

		internal void CompleteInitialization(List<ConfigResponseUrl> configResponses)
		{
			OmexWebServiceUrlsCache.Tracer.TraceDebug(0L, "OmexWebServicesUrlsCache.CompleteInitialization: initialization completed");
			foreach (ConfigResponseUrl configResponseUrl in configResponses)
			{
				string serviceName;
				if ((serviceName = configResponseUrl.ServiceName) != null)
				{
					if (serviceName == "AppInfoQuery15")
					{
						this.KillbitUrl = configResponseUrl.Url;
						continue;
					}
					if (serviceName == "AppInstallInfoQuery15")
					{
						this.DownloadUrl = configResponseUrl.Url;
						continue;
					}
					if (serviceName == "AppStateQuery15")
					{
						this.AppStateUrl = configResponseUrl.Url;
						continue;
					}
				}
				throw new NotSupportedException("Service name: " + configResponseUrl.ServiceName);
			}
			this.ExecuteCompletionCallbacks(true);
		}

		private void GetConfigFailureCallback(Exception exception)
		{
			OmexWebServiceUrlsCache.Tracer.TraceError<Exception>(0L, "OmexWebServicesUrlsCache.GetConfigFailureCallback: exception: {0}", exception);
			this.ExecuteCompletionCallbacks(false);
		}

		private void ExecuteCompletionCallbacks(bool initializeSucceeded)
		{
			OmexWebServiceUrlsCache.Tracer.TraceDebug<bool>(0L, "OmexWebServicesUrlsCache.ExecuteCompletionCallbacks: executing callbacks, initializeSucceeded {0}", initializeSucceeded);
			List<OmexWebServiceUrlsCache.InitializeCompletionCallback> list;
			lock (this.lockObject)
			{
				list = this.initializeCompletionCallbacks;
				this.initializeCompletionCallbacks = new List<OmexWebServiceUrlsCache.InitializeCompletionCallback>();
				this.isInitializing = false;
				this.IsInitialized = initializeSucceeded;
			}
			foreach (OmexWebServiceUrlsCache.InitializeCompletionCallback initializeCompletionCallback in list)
			{
				initializeCompletionCallback(initializeSucceeded);
			}
		}

		internal void TestInitialize(OmexWebServiceUrlsCache.InitializeCompletionCallback initializeCompletionCallback)
		{
			this.isInitializing = true;
			this.Initialize("http:\\dummyUrl", initializeCompletionCallback);
		}

		private const long maxCacheLifeTimeInSeconds = 86400L;

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private object lockObject = new object();

		private static OmexWebServiceUrlsCache singleton = new OmexWebServiceUrlsCache();

		private List<OmexWebServiceUrlsCache.InitializeCompletionCallback> initializeCompletionCallbacks = new List<OmexWebServiceUrlsCache.InitializeCompletionCallback>();

		private int maxInitializeCompletionCallbacks = 1000;

		private bool isInitializing;

		private bool isInitialized;

		private DateTime lastUpdatedTime;

		private string configServiceUrl;

		internal delegate void InitializeCompletionCallback(bool isInitialized);
	}
}
