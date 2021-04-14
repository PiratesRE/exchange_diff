using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class GlsAsyncResult : AsyncResult
	{
		internal GlsAsyncResult(AsyncCallback callback, object asyncState, LocatorService serviceProxy, IAsyncResult internalAsyncResult) : base(callback, asyncState)
		{
			if (serviceProxy != null)
			{
				this.pooledProxy = new GlsAsyncResult.GlsServiceProxy(serviceProxy);
			}
			this.internalAsyncResult = internalAsyncResult;
			this.creationTime = DateTime.UtcNow;
			this.isOfflineGls = false;
		}

		internal void CheckExceptionAndEnd()
		{
			try
			{
				if (this.asyncException != null)
				{
					throw AsyncExceptionWrapperHelper.GetAsyncWrapper(this.asyncException);
				}
			}
			finally
			{
				base.End();
			}
		}

		internal LocatorService ServiceProxy
		{
			get
			{
				if (this.pooledProxy == null)
				{
					return null;
				}
				return this.pooledProxy.Client;
			}
		}

		internal IPooledServiceProxy<LocatorService> PooledProxy
		{
			get
			{
				return this.pooledProxy;
			}
			set
			{
				this.pooledProxy = value;
			}
		}

		internal DateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		internal IAsyncResult InternalAsyncResult
		{
			get
			{
				return this.internalAsyncResult;
			}
			set
			{
				if (this.internalAsyncResult != null && this.internalAsyncResult != value)
				{
					throw new ArgumentException("InternalAsyncResult");
				}
				this.internalAsyncResult = value;
			}
		}

		internal GlsLoggerContext LoggerContext
		{
			get
			{
				return this.loggerContext;
			}
			set
			{
				this.loggerContext = value;
			}
		}

		internal Exception AsyncException
		{
			get
			{
				return this.asyncException;
			}
			set
			{
				if (this.asyncException != null)
				{
					throw new ArgumentException("AsyncException");
				}
				this.asyncException = value;
			}
		}

		internal bool IsOfflineGls
		{
			get
			{
				return this.isOfflineGls;
			}
			set
			{
				this.isOfflineGls = value;
			}
		}

		private readonly DateTime creationTime;

		private IPooledServiceProxy<LocatorService> pooledProxy;

		private IAsyncResult internalAsyncResult;

		private GlsLoggerContext loggerContext;

		private Exception asyncException;

		private bool isOfflineGls;

		private class GlsServiceProxy : IPooledServiceProxy<LocatorService>
		{
			public GlsServiceProxy(LocatorService client)
			{
				this.client = client;
			}

			public LocatorService Client
			{
				get
				{
					return this.client;
				}
			}

			public string Tag { get; set; }

			private LocatorService client;
		}
	}
}
