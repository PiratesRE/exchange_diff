using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MailboxDatabaseConnectionManager : DisposeTrackableBase, IMailboxDatabaseConnectionManager, IDisposable
	{
		public MailboxDatabaseConnectionManager(Guid mdbGuid, ThrottlingObjectPool<PooledEvent> eventPool)
		{
			this.mdbGuid = mdbGuid;
			this.eventPool = eventPool;
		}

		public bool AddConnection(long smtpSessionId, IPAddress remoteIPAddress)
		{
			int minValue = int.MinValue;
			List<KeyValuePair<string, double>> list = null;
			int num = -1;
			return this.GetMdbHealthAndAddConnection(smtpSessionId, remoteIPAddress, out minValue, out list, out num);
		}

		public bool GetMdbHealthAndAddConnection(long smtpSessionId, IPAddress remoteIPAddress, out int mdbHealthMeasure, out List<KeyValuePair<string, double>> healthMonitorList, out int currentConnectionLimit)
		{
			mdbHealthMeasure = -1;
			healthMonitorList = null;
			currentConnectionLimit = -1;
			if (remoteIPAddress == null)
			{
				throw new ArgumentNullException("remoteIPAddress");
			}
			lock (this.syncObject)
			{
				if (this.IsPending(remoteIPAddress))
				{
					return false;
				}
				currentConnectionLimit = DeliveryThrottling.Instance.GetMDBThreadLimitAndHealth(this.mdbGuid, out mdbHealthMeasure, out healthMonitorList);
				if (!this.sessionEvents.TryAdd(smtpSessionId, this.eventPool.Acquire()))
				{
					throw new InvalidOperationException("Unable to initialize synchronization events for the session.");
				}
				MailboxDatabaseConnectionManager.ConnectionInfo orAdd = this.connections.GetOrAdd(smtpSessionId, new MailboxDatabaseConnectionManager.ConnectionInfo(smtpSessionId, remoteIPAddress, ExDateTime.UtcNow, false));
				if (this.connections.Keys.Count <= currentConnectionLimit && this.pendingConnections.Count == 0)
				{
					orAdd.Active = true;
					this.SetSessionEvent(smtpSessionId);
				}
				else
				{
					this.pendingConnections.Enqueue(smtpSessionId);
					if (this.connections.Keys.Count <= currentConnectionLimit)
					{
						this.ActivateNextConnection();
					}
				}
			}
			return true;
		}

		public bool RemoveConnection(long smtpSessionId, IPAddress remoteIPAddress)
		{
			if (remoteIPAddress == null)
			{
				throw new ArgumentNullException("remoteIPAddress");
			}
			MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
			if (!this.connections.TryGetValue(smtpSessionId, out connectionInfo))
			{
				return false;
			}
			if (!remoteIPAddress.Equals(connectionInfo.RemoteIPAddress))
			{
				return false;
			}
			bool active = connectionInfo.Active;
			connectionInfo.Active = false;
			bool result = false;
			lock (this.syncObject)
			{
				result = this.RemoveConnectionTrackingItems(smtpSessionId);
				if (active)
				{
					this.ActivateNextConnection();
				}
			}
			return result;
		}

		public bool TryAcquire(long smtpSessionId, IPAddress remoteIPAddress, TimeSpan timeout, out IMailboxDatabaseConnectionInfo mdbConnectionInfo)
		{
			bool result;
			try
			{
				mdbConnectionInfo = this.Acquire(smtpSessionId, remoteIPAddress, timeout);
				result = true;
			}
			catch (InvalidOperationException)
			{
				mdbConnectionInfo = null;
				result = false;
			}
			return result;
		}

		public IMailboxDatabaseConnectionInfo Acquire(long smtpSessionId, IPAddress remoteIPAddress, TimeSpan timeout)
		{
			if (remoteIPAddress == null)
			{
				throw new ArgumentNullException("remoteIPAddress");
			}
			if (this.connections.Count == 0)
			{
				throw new InvalidOperationException("The specified mailbox database has no current connections.");
			}
			if (!this.connections.ContainsKey(smtpSessionId))
			{
				throw new InvalidOperationException("The specified session is not currently connected to this mailbox database.");
			}
			this.UpdateLastActivityTime(smtpSessionId);
			if (!this.WaitUsingEvent(smtpSessionId, timeout))
			{
				return null;
			}
			return new MailboxDatabaseConnectionInfo(this.mdbGuid, smtpSessionId, remoteIPAddress);
		}

		public bool Release(ref IMailboxDatabaseConnectionInfo mdbConnectionInfo)
		{
			if (mdbConnectionInfo == null)
			{
				throw new ArgumentNullException("mdbConnectionInfo");
			}
			if (this.connections.Count == 0)
			{
				return false;
			}
			long smtpSessionId = mdbConnectionInfo.SmtpSessionId;
			mdbConnectionInfo = null;
			this.UpdateLastActivityTime(smtpSessionId);
			lock (this.syncObject)
			{
				this.DeactivateConnection(smtpSessionId);
				this.ActivateNextConnection();
			}
			return true;
		}

		public void ProcessStaleConnections(ExDateTime current, TimeSpan connectionMaxAge)
		{
			List<MailboxDatabaseConnectionManager.ConnectionInfo> list = new List<MailboxDatabaseConnectionManager.ConnectionInfo>();
			foreach (long key in this.connections.Keys)
			{
				MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
				if (this.connections.TryGetValue(key, out connectionInfo) && ExDateTime.TimeDiff(current, connectionInfo.LastActivityTime).TotalMilliseconds > connectionMaxAge.TotalMilliseconds)
				{
					list.Add(connectionInfo);
				}
			}
			foreach (MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo2 in list)
			{
				this.RemoveConnection(connectionInfo2.SmtpSessionId, connectionInfo2.RemoteIPAddress);
			}
		}

		public void UpdateLastActivityTime(long smtpSessionId)
		{
			MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
			if (this.connections.TryGetValue(smtpSessionId, out connectionInfo))
			{
				connectionInfo.LastActivityTime = ExDateTime.UtcNow;
			}
		}

		public XElement GetDiagnosticInfo(XElement parentElement)
		{
			XElement xelement = new XElement("Database");
			xelement.Add(new XElement("id", this.mdbGuid));
			int num = 0;
			DeliveryThrottling.Instance.TryGetDatabaseHealth(this.mdbGuid, out num);
			xelement.Add(new XElement("databaseHealthMeasure", num));
			XElement xelement2 = new XElement("activeConnectionCount");
			XElement xelement3 = new XElement("pendingConnectionCount");
			xelement.Add(xelement2);
			xelement.Add(xelement3);
			XElement xelement4 = new XElement("Sessions");
			xelement.Add(xelement4);
			int num2 = 0;
			foreach (long num3 in this.connections.Keys)
			{
				XElement xelement5 = new XElement("Session");
				xelement4.Add(xelement5);
				xelement5.Add(new XElement("id", num3));
				MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
				if (this.connections.TryGetValue(num3, out connectionInfo))
				{
					if (connectionInfo.Active)
					{
						num2++;
					}
					xelement5.Add(new XElement("state", connectionInfo.Active ? "Active" : "Pending"));
				}
			}
			xelement2.SetValue(num2);
			xelement3.SetValue(this.pendingConnections.Count);
			parentElement.Add(xelement);
			return parentElement;
		}

		public int GetThreadCount()
		{
			int num = 0;
			foreach (long key in this.connections.Keys)
			{
				MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
				if (this.connections.TryGetValue(key, out connectionInfo) && connectionInfo.Active)
				{
					num++;
				}
			}
			return num;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxDatabaseConnectionManager>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				foreach (PooledEvent pooledEvent in this.sessionEvents.Values)
				{
					pooledEvent.Dispose();
				}
				this.sessionEvents.Clear();
				this.sessionEvents = null;
			}
		}

		private bool RemoveConnectionTrackingItems(long smtpSessionId)
		{
			MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
			bool result = this.connections.TryRemove(smtpSessionId, out connectionInfo);
			PooledEvent value = null;
			bool flag = this.sessionEvents.TryRemove(smtpSessionId, out value);
			if (flag)
			{
				this.eventPool.Release(value);
			}
			return result;
		}

		private bool DeactivateConnection(long smtpSessionId)
		{
			if (!this.connections.ContainsKey(smtpSessionId))
			{
				return false;
			}
			this.ResetSessionEvent(smtpSessionId);
			this.connections[smtpSessionId].Active = false;
			this.pendingConnections.Enqueue(smtpSessionId);
			return true;
		}

		private bool ActivateNextConnection()
		{
			if (this.pendingConnections.Count == 0)
			{
				return false;
			}
			long key = 0L;
			MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
			while (connectionInfo == null && this.pendingConnections.TryDequeue(out key))
			{
				if (this.connections.TryGetValue(key, out connectionInfo))
				{
					connectionInfo.Active = true;
					this.SetSessionEvent(connectionInfo.SmtpSessionId);
				}
			}
			return true;
		}

		private bool IsPending(IPAddress remoteIPAddress)
		{
			lock (this.syncObject)
			{
				MailboxDatabaseConnectionManager.ConnectionInfo connectionInfo = null;
				foreach (long key in this.pendingConnections)
				{
					if (this.connections.TryGetValue(key, out connectionInfo) && !connectionInfo.Active && connectionInfo.RemoteIPAddress.Equals(remoteIPAddress))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsActive(long smtpSessionId)
		{
			return this.IsConnected(smtpSessionId) && this.connections[smtpSessionId].Active;
		}

		private bool IsConnected(long smtpSessionId)
		{
			return this.connections.ContainsKey(smtpSessionId);
		}

		private bool WaitUsingEvent(long smtpSessionId, TimeSpan timeout)
		{
			PooledEvent pooledEvent = null;
			if (!this.sessionEvents.TryGetValue(smtpSessionId, out pooledEvent))
			{
				throw new InvalidOperationException("The session event was not able to be retrieved when attempting to wait for a connection.");
			}
			return pooledEvent.WaitHandle.WaitOne(timeout);
		}

		private void ResetSessionEvent(long smtpSessionId)
		{
			PooledEvent pooledEvent = null;
			if (!this.sessionEvents.TryGetValue(smtpSessionId, out pooledEvent))
			{
				throw new InvalidOperationException("The session event was not able to be retrieved when attempting to wake a thread waiting for a connection.");
			}
			pooledEvent.WaitHandle.Reset();
		}

		private void SetSessionEvent(long smtpSessionId)
		{
			PooledEvent pooledEvent = null;
			if (!this.sessionEvents.TryGetValue(smtpSessionId, out pooledEvent))
			{
				throw new InvalidOperationException("The session event was not able to be retrieved when attempting to wake a thread waiting for a connection.");
			}
			pooledEvent.WaitHandle.Set();
		}

		private readonly Guid mdbGuid;

		private ConcurrentDictionary<long, MailboxDatabaseConnectionManager.ConnectionInfo> connections = new ConcurrentDictionary<long, MailboxDatabaseConnectionManager.ConnectionInfo>();

		private ConcurrentQueue<long> pendingConnections = new ConcurrentQueue<long>();

		private ConcurrentDictionary<long, PooledEvent> sessionEvents = new ConcurrentDictionary<long, PooledEvent>();

		private ThrottlingObjectPool<PooledEvent> eventPool;

		private object syncObject = new object();

		internal class ConnectionInfo
		{
			public ConnectionInfo(long sessionId, IPAddress remoteIPAddress, ExDateTime connectTime, bool active)
			{
				this.SmtpSessionId = sessionId;
				this.RemoteIPAddress = remoteIPAddress;
				this.LastActivityTime = connectTime;
				this.Active = active;
			}

			public long SmtpSessionId { get; private set; }

			public IPAddress RemoteIPAddress { get; private set; }

			public bool Active { get; set; }

			public ExDateTime LastActivityTime { get; set; }
		}
	}
}
