using System;
using System.Threading;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class TraceCollector<TAggregator, TContainer, TKey, TParameters> where TAggregator : TraceDataAggregator<TParameters> where TContainer : TraceContainer<TAggregator, TKey, TParameters>, new() where TParameters : ITraceParameters
	{
		protected TraceCollector(StoreDatabase database, LoggerType loggerType)
		{
			this.database = database;
			this.logger = LoggerManager.GetLogger(loggerType);
			this.container = Activator.CreateInstance<TContainer>();
		}

		internal TContainer Data
		{
			get
			{
				return this.container;
			}
		}

		public void Add(TKey key, TParameters parameters)
		{
			if (this.logger != null && this.logger.IsLoggingEnabled)
			{
				this.container.Set(key, parameters);
				if (this.container.HasDataToLog && Interlocked.Exchange(ref this.governor, 1) == 0)
				{
					TContainer tcontainer = Interlocked.Exchange<TContainer>(ref this.container, Activator.CreateInstance<TContainer>());
					if (this.logger.IsLoggingEnabled)
					{
						tcontainer.Commit(this.database, this.logger);
					}
					Interlocked.Exchange(ref this.governor, 0);
				}
			}
		}

		private readonly StoreDatabase database;

		private readonly IBinaryLogger logger;

		private TContainer container;

		private int governor;
	}
}
