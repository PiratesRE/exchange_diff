using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.ResourceMonitoring;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal class MessagingDatabaseComponent : IMessagingDatabaseComponent, IStartableTransportComponent, ITransportComponent, IDiagnosable
	{
		public IMessagingDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public string CurrentState
		{
			get
			{
				return this.database.CurrentState;
			}
		}

		public IEnumerable<RoutedQueueBase> Queues
		{
			get
			{
				return this.queuesByNextHopSolutionKey.Values;
			}
		}

		public RoutedQueueBase GetQueue(NextHopSolutionKey queueNextHopSolutionKey)
		{
			RoutedQueueBase result;
			if (!this.TryGetQueue(queueNextHopSolutionKey, out result))
			{
				throw new KeyNotFoundException(string.Format("Unable to find queue with key={0}", queueNextHopSolutionKey));
			}
			return result;
		}

		public RoutedQueueBase GetOrAddQueue(NextHopSolutionKey queueNextHopSolutionKey)
		{
			RoutedQueueBase result;
			if (!this.TryGetQueue(queueNextHopSolutionKey, out result))
			{
				result = this.CreateQueue(queueNextHopSolutionKey, false);
			}
			return result;
		}

		public bool TryGetQueue(NextHopSolutionKey queueNextHopSolutionKey, out RoutedQueueBase queue)
		{
			return this.queuesByNextHopSolutionKey.TryGetValue(queueNextHopSolutionKey, out queue);
		}

		public RoutedQueueBase CreateQueue(NextHopSolutionKey key, bool suspendQueue)
		{
			long id = Interlocked.Increment(ref this.currentQueueId);
			RoutedQueueBase routedQueueBase = new RoutedQueueBase(id, key)
			{
				Suspended = suspendQueue
			};
			if (this.queuesByNextHopSolutionKey.TryAdd(key, routedQueueBase))
			{
				routedQueueBase.Commit();
			}
			else
			{
				routedQueueBase = this.queuesByNextHopSolutionKey[key];
			}
			return routedQueueBase;
		}

		public void Load()
		{
			this.database.Attach(this.config);
			this.meterableDataSource = MeterableJetDataSourceFactory.CreateMeterableDataSource(this.database.DataSource);
			this.perfCounters = DatabasePerfCounters.GetInstance("other");
			this.statisticsTimer = new GuardedTimer(new TimerCallback(this.UpdateStatistics), null, this.config.StatisticsUpdateInterval);
			this.ScanQueueTable();
		}

		public void Unload()
		{
			if (this.statisticsTimer != null)
			{
				this.statisticsTimer.Dispose(true);
			}
			this.meterableDataSource = null;
			this.database.Detach();
		}

		public string OnUnhandledException(Exception e)
		{
			if (this.Database.DataSource != null)
			{
				return "Messaging Database flush result: " + (this.Database.DataSource.TryForceFlush() ?? "success.");
			}
			return null;
		}

		public void SetLoadTimeDependencies(IMessagingDatabaseConfig config)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			this.config = config;
		}

		public IBootLoader CreateBootScanner()
		{
			return StorageFactory.CreateBootScanner();
		}

		public string GetDiagnosticComponentName()
		{
			return "MessagingDatabase";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement("MessagingDatabase");
			if (parameters.Argument.Equals("config", StringComparison.InvariantCultureIgnoreCase))
			{
				xelement.Add(TransportAppConfig.GetDiagnosticInfoForType(this.config));
			}
			else
			{
				xelement.Add(this.Database.GetDiagnosticInfo(parameters));
			}
			return xelement;
		}

		public void Start(bool initiallyPaused, ServiceState intendedState)
		{
			this.database.Start();
		}

		public void Stop()
		{
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		private void UpdateStatistics(object state)
		{
			if (this.perfCounters == null || this.meterableDataSource == null)
			{
				return;
			}
			this.perfCounters.TransportQueueDatabaseFileSize.RawValue = (long)ByteQuantifiedSize.FromBytes((ulong)this.meterableDataSource.GetDatabaseFileSize()).ToMB();
			this.perfCounters.TransportQueueDatabaseInternalFreeSpace.RawValue = (long)ByteQuantifiedSize.FromBytes((ulong)this.meterableDataSource.GetAvailableFreeSpace()).ToMB();
		}

		private void ScanQueueTable()
		{
			QueueTable queueTable = this.database.QueueTable;
			this.currentQueueId = 0L;
			using (DataTableCursor cursor = queueTable.GetCursor())
			{
				using (cursor.BeginTransaction())
				{
					bool flag = cursor.TryMoveFirst();
					while (flag)
					{
						RoutedQueueBase routedQueueBase = RoutedQueueBase.LoadFromRow(cursor);
						NextHopSolutionKey key = new NextHopSolutionKey(routedQueueBase.NextHopType.DeliveryType, routedQueueBase.NextHopDomain, routedQueueBase.NextHopConnector, routedQueueBase.NextHopTlsDomain);
						this.queuesByNextHopSolutionKey.TryAdd(key, routedQueueBase);
						if (routedQueueBase.Id > this.currentQueueId)
						{
							this.currentQueueId = routedQueueBase.Id;
						}
						flag = cursor.TryMoveNext(false);
					}
				}
			}
		}

		private readonly IMessagingDatabase database = StorageFactory.GetNewDatabaseInstance();

		private readonly ConcurrentDictionary<NextHopSolutionKey, RoutedQueueBase> queuesByNextHopSolutionKey = new ConcurrentDictionary<NextHopSolutionKey, RoutedQueueBase>();

		private IMessagingDatabaseConfig config;

		private long currentQueueId;

		private DatabasePerfCountersInstance perfCounters;

		private IMeterableJetDataSource meterableDataSource;

		private GuardedTimer statisticsTimer;
	}
}
