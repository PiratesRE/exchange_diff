using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.MailboxTransport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class SubmissionConnectionPool
	{
		private SubmissionConnectionPool()
		{
			this.connections = new Dictionary<string, SubmissionConnection>();
			this.connectionsSyncObject = new object();
			TraceHelper.SubmissionConnectionPoolTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "SubmissionConnectionPool: Instance created.");
		}

		public static SubmissionConnectionWrapper GetConnection(string server, string database)
		{
			SubmissionConnection connection = SubmissionConnectionPool.instance.InternalGetConnection(server, database);
			return new SubmissionConnectionWrapper(connection);
		}

		public static void ExpireOldConnections()
		{
			SubmissionConnectionPool.instance.InternalExpireOldConnections();
		}

		public bool CanStopConnection(SubmissionConnection connection)
		{
			bool result;
			lock (this.connectionsSyncObject)
			{
				if (connection.IsInUse)
				{
					TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.CanStopConnection: Thread {0}, No removing in-use connection: {1}.", Thread.CurrentThread.ManagedThreadId, connection.ToString());
					result = false;
				}
				else if (connection.HasReachedSubmissionLimit)
				{
					TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.CanStopConnection: Thread {0}, Connection not in use, and reached limit, OK to stop: {1}.", Thread.CurrentThread.ManagedThreadId, connection.ToString());
					result = true;
				}
				else if (connection.Failures > 0)
				{
					if (this.connections.ContainsKey(connection.Key))
					{
						this.connections.Remove(connection.Key);
						TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.CanStopConnection: Thread {0}, Removed connection: {1}.", Thread.CurrentThread.ManagedThreadId, connection.ToString());
						result = true;
					}
					else
					{
						TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.CanStopConnection: Thread {0}, Connection was already removed: {1}.", Thread.CurrentThread.ManagedThreadId, connection.ToString());
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		internal static void TestReset()
		{
			SubmissionConnectionPool.instance.InternalTestReset();
		}

		private void InternalTestReset()
		{
			lock (this.connectionsSyncObject)
			{
				this.connections.Clear();
			}
		}

		private SubmissionConnection InternalGetConnection(string server, string database)
		{
			string connectionKey = this.GetConnectionKey(server, database);
			TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.InternalGetConnection: Thread {0}, Server {1}, Database {2}).", Thread.CurrentThread.ManagedThreadId, server, database);
			bool flag = false;
			SubmissionConnection submissionConnection;
			lock (this.connectionsSyncObject)
			{
				if (this.connections.TryGetValue(connectionKey, out submissionConnection))
				{
					submissionConnection.SubmissionStarted();
					TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.InternalGetConnection: Thread {0}, using existing connection: {1}.", Thread.CurrentThread.ManagedThreadId, submissionConnection.ToString());
					if (submissionConnection.HasReachedSubmissionLimit)
					{
						TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, 0L, "SubmissionConnectionPool.InternalGetConnection: Thread {0}, connection has reached its message limit and will be removed from the pool: {1}.", Thread.CurrentThread.ManagedThreadId, submissionConnection.ToString());
						this.connections.Remove(connectionKey);
					}
				}
				else
				{
					submissionConnection = new SubmissionConnection(connectionKey, this, server, database);
					submissionConnection.SubmissionStarted();
					flag = true;
					this.connections[connectionKey] = submissionConnection;
					TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.InternalGetConnection: Thread {0}, created new connection {1}.", Thread.CurrentThread.ManagedThreadId, submissionConnection.ToString());
				}
			}
			if (flag)
			{
				submissionConnection.StartConnection();
			}
			return submissionConnection;
		}

		private void InternalExpireOldConnections()
		{
			List<SubmissionConnection> list = null;
			lock (this.connectionsSyncObject)
			{
				foreach (string key in this.connections.Keys)
				{
					SubmissionConnection submissionConnection = this.connections[key];
					if (submissionConnection.TimeoutElapsed && !submissionConnection.IsInUse)
					{
						if (list == null)
						{
							list = new List<SubmissionConnection>();
						}
						TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.ExpireOldConnections: Thread {0}, Adding to expired list: {1}.", Thread.CurrentThread.ManagedThreadId, submissionConnection.ToString());
						list.Add(submissionConnection);
					}
				}
				if (list != null)
				{
					foreach (SubmissionConnection submissionConnection2 in list)
					{
						TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.ExpireOldConnections: Thread {0}, Removing connection: {1}.", Thread.CurrentThread.ManagedThreadId, submissionConnection2.ToString());
						this.connections.Remove(submissionConnection2.Key);
					}
				}
			}
			if (list != null)
			{
				foreach (SubmissionConnection submissionConnection3 in list)
				{
					TraceHelper.SubmissionConnectionPoolTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnectionPool.ExpireOldConnections: Thread {0}, Invoking TimeOutExpired for: {1}.", Thread.CurrentThread.ManagedThreadId, submissionConnection3.ToString());
					submissionConnection3.TimeoutExpired();
				}
			}
		}

		private string GetConnectionKey(string server, string database)
		{
			return string.Format("Server: {0}, Database: {1}", server, database);
		}

		private static readonly SubmissionConnectionPool instance = new SubmissionConnectionPool();

		private readonly object connectionsSyncObject;

		private Dictionary<string, SubmissionConnection> connections;
	}
}
