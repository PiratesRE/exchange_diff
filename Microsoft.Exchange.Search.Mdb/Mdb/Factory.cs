using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Performance;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class Factory
	{
		protected Factory()
		{
		}

		internal static Hookable<Factory> Instance
		{
			[DebuggerStepThrough]
			get
			{
				return Factory.instance;
			}
		}

		internal static Factory Current
		{
			[DebuggerStepThrough]
			get
			{
				return Factory.instance.Value;
			}
		}

		internal virtual IMdbWatcher CreateMdbWatcher()
		{
			return new MdbWatcher();
		}

		internal virtual IFeeder CreateNotificationsFeeder(MdbPerfCountersInstance mdbFeedingPerfCounters, MdbInfo mdbInfo, ISearchServiceConfig config, ISubmitDocument indexFeeder, IWatermarkStorage watermarkStorage, IIndexStatusStore indexStatusStore)
		{
			return new NotificationsFeeder(mdbFeedingPerfCounters, mdbInfo, config, indexFeeder, watermarkStorage, indexStatusStore);
		}

		internal virtual IFeeder CreateCrawlerFeeder(MdbPerfCountersInstance mdbFeedingPerfCounters, MdbInfo mdbInfo, ISearchServiceConfig config, ISubmitDocument indexFeeder, IWatermarkStorage watermarkStorage, IFailedItemStorage failedItemStorage, IIndexStatusStore indexStatusStore)
		{
			CrawlerMailboxIterator mailboxIterator = Factory.Current.CreateCrawlerMailboxIterator(mdbInfo);
			ICrawlerItemIterator<int> itemIterator = Factory.Current.CreateCrawlerItemIterator(config.MaxRowCount);
			return new CrawlerFeeder(mdbFeedingPerfCounters, mdbInfo, config, mailboxIterator, itemIterator, watermarkStorage, failedItemStorage, indexFeeder, indexStatusStore);
		}

		internal virtual IFeeder CreateRetryFeeder(MdbPerfCountersInstance mdbFeedingPerfCounters, MdbInfo mdbInfo, ISearchServiceConfig config, ISubmitDocument indexFeeder, IFailedItemStorage failedItemStorage, IWatermarkStorage watermarkStorage, IIndexStatusStore indexStatusStore)
		{
			return new RetryFeeder(mdbFeedingPerfCounters, mdbInfo, indexFeeder, config, failedItemStorage, watermarkStorage, indexStatusStore);
		}

		internal virtual INotificationsEventSource CreateNotificationsEventSource(MdbInfo mdbInfo)
		{
			return new NotificationsEventSource(mdbInfo);
		}

		internal virtual CrawlerMailboxIterator CreateCrawlerMailboxIterator(MdbInfo mdbInfo)
		{
			return new CrawlerMailboxIterator(mdbInfo);
		}

		internal virtual ICrawlerItemIterator<int> CreateCrawlerItemIterator(int maxRowCount)
		{
			return new CrawlerDocIdViewIterator(maxRowCount);
		}

		internal virtual IFeederRateThrottlingManager CreateFeederRateThrottlingManager(ISearchServiceConfig config, MdbInfo mdbInfo, FeederRateThrottlingManager.ThrottlingRateExecutionType execType)
		{
			return new FeederRateThrottlingManager(config, mdbInfo, execType);
		}

		internal virtual IFeederDelayThrottlingManager CreateFeederDelayThrottlingManager(ISearchServiceConfig config)
		{
			return new FeederDelayThrottlingManager(config);
		}

		internal virtual ISearchServiceConfig CreateSearchServiceConfig()
		{
			return SearchConfig.Instance;
		}

		private static readonly Hookable<Factory> instance = Hookable<Factory>.Create(true, new Factory());
	}
}
