using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class ConnectionList : BaseObject
	{
		public ConnectionList()
		{
			this.idleConnectionsScavengerTimer = new MaintenanceJobTimer(new Action(this.ScavengeIdleConnections), () => Configuration.ServiceConfiguration.IdleConnectionCheckPeriod != TimeSpan.Zero, Configuration.ServiceConfiguration.MaintenanceJobTimerCheckPeriod, TimeSpan.Zero);
			this.logConnectionLatencyTimer = new MaintenanceJobTimer(new Action(this.LogConnectionLatency), () => Configuration.ServiceConfiguration.LogConnectionLatencyCheckPeriod != TimeSpan.Zero, Configuration.ServiceConfiguration.MaintenanceJobTimerCheckPeriod, TimeSpan.FromMilliseconds(Configuration.ServiceConfiguration.MaintenanceJobTimerCheckPeriod.TotalMilliseconds / 2.0));
		}

		public int MaximumConnections
		{
			get
			{
				return Configuration.ServiceConfiguration.MaximumConnections;
			}
		}

		public static int GetAsyncConnectionHandle(int connectionId)
		{
			return connectionId | 1073741824;
		}

		public static int GetSyncConnectionHandle(int asyncConnectionId)
		{
			return asyncConnectionId & 1073741823;
		}

		public void AddConnection(Connection connection)
		{
			if (this.connectionMap.Count >= this.MaximumConnections)
			{
				throw new OutOfMemoryException();
			}
			try
			{
				this.connectionListLock.EnterWriteLock();
				this.connectionMap.Add(Connection.GetConnectionId(connection), connection);
			}
			finally
			{
				try
				{
					this.connectionListLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ConnectionCount.Increment();
		}

		public void RemoveAndDisposeConnection(int connectionId, DisconnectReason disconnectReason)
		{
			string reason;
			switch (disconnectReason)
			{
			case DisconnectReason.ClientDisconnect:
				reason = "client disconnected";
				break;
			case DisconnectReason.ServerDropped:
				reason = "server dropped connection";
				break;
			case DisconnectReason.NetworkRundown:
				reason = "network rundown";
				break;
			default:
				throw new InvalidOperationException(string.Format("Invalid DisconnectReason; disconnectReason={0}", disconnectReason));
			}
			Connection connection;
			try
			{
				this.connectionListLock.EnterWriteLock();
				if (this.connectionMap.TryGetValue(connectionId, out connection))
				{
					this.connectionMap.Remove(connectionId);
				}
			}
			finally
			{
				try
				{
					this.connectionListLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (connection != null)
			{
				using (connection)
				{
					ExTraceGlobals.ConnectRpcTracer.TraceDebug<Connection>(Activity.TraceId, "RemoveAndDisposeConnection. Connection = {0}.", connection);
					connection.CompleteAction(reason, false, RpcErrorCode.None);
					RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ConnectionCount.Decrement();
				}
			}
			ProtocolLog.LogDisconnect(disconnectReason);
		}

		public bool TryGetConnection(int connectionId, out Connection connection)
		{
			connection = null;
			bool result;
			try
			{
				this.connectionListLock.EnterReadLock();
				result = this.connectionMap.TryGetValue(connectionId, out connection);
			}
			finally
			{
				try
				{
					this.connectionListLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		public Connection GetConnection(int connectionId)
		{
			Connection result;
			if (!this.TryGetConnection(connectionId, out result))
			{
				throw new ServerUnavailableException("contextHandle no longer valid - the connection must have been dropped during the previous RPC. Simulate a server failure.", null);
			}
			return result;
		}

		public int NextConnectionId()
		{
			int result;
			for (;;)
			{
				uint num = (uint)Interlocked.Increment(ref this.lastConnectionId);
				int num2 = (int)(1U + num % 268435456U);
				try
				{
					this.connectionListLock.EnterReadLock();
					if (this.connectionMap.ContainsKey(num2))
					{
						continue;
					}
					result = num2;
				}
				finally
				{
					try
					{
						this.connectionListLock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				break;
			}
			return result;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ConnectionList>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.idleConnectionsScavengerTimer);
			Util.DisposeIfPresent(this.logConnectionLatencyTimer);
			try
			{
				this.connectionListLock.EnterWriteLock();
				using (Dictionary<int, Connection>.ValueCollection.Enumerator enumerator = this.connectionMap.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Connection connection = enumerator.Current;
						if (connection.TryIncrementUsageCount())
						{
							connection.MarkForRemoval(delegate
							{
								connection.CompleteAction("shutdown", false, RpcErrorCode.None);
								connection.Dispose();
							});
							connection.DecrementUsageCount();
						}
					}
				}
				this.connectionMap.Clear();
				RpcClientAccessPerformanceCountersWrapper.RcaPerformanceCounters.ConnectionCount.RawValue = 0L;
			}
			finally
			{
				try
				{
					this.connectionListLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			base.InternalDispose();
		}

		private void ForEachValidConnection(Predicate<Connection> shouldInclude, Action<Connection> executeDelegate)
		{
			List<int> list = new List<int>();
			try
			{
				this.connectionListLock.EnterReadLock();
				foreach (KeyValuePair<int, Connection> keyValuePair in this.connectionMap)
				{
					if (shouldInclude(keyValuePair.Value))
					{
						list.Add(keyValuePair.Key);
					}
				}
			}
			finally
			{
				try
				{
					this.connectionListLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			foreach (int connectionId in list)
			{
				Connection connection;
				if (this.TryGetConnection(connectionId, out connection) && connection.TryIncrementUsageCount())
				{
					try
					{
						executeDelegate(connection);
					}
					finally
					{
						connection.DecrementUsageCount();
					}
				}
			}
		}

		private void ScavengeIdleConnections()
		{
			TimeSpan timePeriod = Configuration.ServiceConfiguration.IdleConnectionCheckPeriod;
			ExDateTime utcNow = ExDateTime.UtcNow;
			this.ForEachValidConnection((Connection x) => x.LastAccessTime + timePeriod < utcNow, delegate(Connection connection)
			{
				using (Activity.Guard guard = new Activity.Guard())
				{
					guard.AssociateWithCurrentThread(connection.Activity, false);
					connection.CompleteAction("idle", false, RpcErrorCode.None);
				}
			});
		}

		private void LogConnectionLatency()
		{
			TimeSpan timePeriod = Configuration.ServiceConfiguration.LogConnectionLatencyCheckPeriod;
			ExDateTime utcNow = ExDateTime.UtcNow;
			this.ForEachValidConnection((Connection x) => x.LastLogTime + timePeriod < utcNow, delegate(Connection connection)
			{
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				ReferencedActivityScope referencedActivityScope = null;
				try
				{
					referencedActivityScope = connection.GetReferencedActivityScope();
					ActivityContext.SetThreadScope(referencedActivityScope.ActivityScope);
					using (Activity.Guard guard = new Activity.Guard())
					{
						guard.AssociateWithCurrentThread(connection.Activity, false);
						connection.UpdateBudgetBalance();
						ProtocolLog.LogConnectionRpcProcessingTime();
						connection.LastLogTime = utcNow;
						connection.StartNewActivityScope();
					}
				}
				finally
				{
					ActivityContext.SetThreadScope(currentActivityScope);
					if (referencedActivityScope != null)
					{
						referencedActivityScope.Release();
					}
				}
			});
		}

		private const int MaxConnectionId = 268435456;

		private const int ConnectionIdKeyMask = 1073741823;

		private const uint AsynchronousConnectionIdFlag = 1073741824U;

		private readonly ReaderWriterLockSlim connectionListLock = new ReaderWriterLockSlim();

		private readonly Dictionary<int, Connection> connectionMap = new Dictionary<int, Connection>();

		private readonly MaintenanceJobTimer idleConnectionsScavengerTimer;

		private readonly MaintenanceJobTimer logConnectionLatencyTimer;

		private int lastConnectionId;
	}
}
