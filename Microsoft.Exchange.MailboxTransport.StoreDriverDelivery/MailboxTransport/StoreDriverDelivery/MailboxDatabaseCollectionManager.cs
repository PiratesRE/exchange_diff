using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MailboxDatabaseCollectionManager : DisposeTrackableBase, IMailboxDatabaseCollectionManager, IDisposable
	{
		public MailboxDatabaseCollectionManager() : this(TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(15.0))
		{
		}

		public MailboxDatabaseCollectionManager(TimeSpan staleConnectionProcessingInterval, TimeSpan connectionMaxIdle)
		{
			this.staleConnectionsProcessor = new GuardedTimer(new TimerCallback(this.ProcessStaleConnections), null, staleConnectionProcessingInterval, staleConnectionProcessingInterval);
			this.connectionMaxIdle = connectionMaxIdle;
		}

		public IMailboxDatabaseConnectionManager GetConnectionManager(Guid mdbGuid)
		{
			return this.databases.GetOrAdd(mdbGuid, new Func<Guid, MailboxDatabaseConnectionManager>(this.CreateConnectionManagerForKey));
		}

		public XElement GetDiagnosticInfo(XElement parentElement)
		{
			parentElement.Add(new XElement("MaxMailboxDeliveryPerMdbConnections", DeliveryConfiguration.Instance.Throttling.MaxMailboxDeliveryPerMdbConnections));
			foreach (MailboxDatabaseConnectionManager mailboxDatabaseConnectionManager in this.databases.Values)
			{
				mailboxDatabaseConnectionManager.GetDiagnosticInfo(parentElement);
			}
			return parentElement;
		}

		public void UpdateMdbThreadCounters()
		{
			foreach (KeyValuePair<Guid, MailboxDatabaseConnectionManager> keyValuePair in this.databases)
			{
				StoreDriverDatabasePerfCounters.AddDeliveryThreadSample(keyValuePair.Key.ToString(), (long)keyValuePair.Value.GetThreadCount());
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxDatabaseCollectionManager>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.staleConnectionsProcessor.Dispose(true);
				this.staleConnectionsProcessor = null;
				foreach (IMailboxDatabaseConnectionManager mailboxDatabaseConnectionManager in this.databases.Values)
				{
					mailboxDatabaseConnectionManager.Dispose();
				}
				this.databases.Clear();
				this.databases = null;
				this.eventPool.Dispose();
				this.eventPool = null;
			}
		}

		private void ProcessStaleConnections(object state)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			foreach (MailboxDatabaseConnectionManager mailboxDatabaseConnectionManager in this.databases.Values)
			{
				mailboxDatabaseConnectionManager.ProcessStaleConnections(utcNow, this.connectionMaxIdle);
			}
		}

		private MailboxDatabaseConnectionManager CreateConnectionManagerForKey(Guid mdbGuid)
		{
			return new MailboxDatabaseConnectionManager(mdbGuid, this.eventPool);
		}

		private readonly TimeSpan connectionMaxIdle;

		private GuardedTimer staleConnectionsProcessor;

		private ConcurrentDictionary<Guid, MailboxDatabaseConnectionManager> databases = new ConcurrentDictionary<Guid, MailboxDatabaseConnectionManager>();

		private ThrottlingObjectPool<PooledEvent> eventPool = new ThrottlingObjectPool<PooledEvent>(DeliveryConfiguration.Instance.Throttling.MailboxServerThreadLimit);
	}
}
