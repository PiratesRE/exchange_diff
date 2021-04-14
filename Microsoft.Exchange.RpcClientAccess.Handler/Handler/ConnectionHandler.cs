using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConnectionHandler : BaseObject, IConnectionHandler, IDisposable
	{
		private ConnectionHandler(IConnection connectionHandlerConnection)
		{
			this.connection = connectionHandlerConnection;
			this.ropHandler = new RopHandler(this);
			this.notificationHandler = new NotificationHandler(this);
		}

		public IConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		public IRopHandler RopHandler
		{
			get
			{
				return this.ropHandler;
			}
		}

		public INotificationHandler NotificationHandler
		{
			get
			{
				return this.notificationHandler;
			}
		}

		public static ConnectionHandler Create(IConnection connectionHandlerConnection)
		{
			return new ConnectionHandler(connectionHandlerConnection);
		}

		public void BeginRopProcessing(AuxiliaryData auxiliaryData)
		{
			bool flag = false;
			lock (this.logons)
			{
				flag = (this.logons.Count > 0);
			}
			if (flag)
			{
				this.lastTimeConnectionHadLogons = DateTime.UtcNow;
			}
		}

		public void EndRopProcessing(AuxiliaryData auxiliaryData)
		{
			bool flag = false;
			lock (this.logons)
			{
				flag = (this.logons.Count > 0);
			}
			if (!flag && Configuration.ServiceConfiguration.EnableSmartConnectionTearDown && DateTime.UtcNow - this.lastTimeConnectionHadLogons > ConnectionHandler.MaximumLifetimeForAConnectionWithoutLogons)
			{
				throw new SessionDeadException("Connection doesn't have any open logons, but has client activity. This may be masking synchronization stalls. Dropping a connection.");
			}
			if (this.notificationHandler.HasPendingNotifications())
			{
				this.notificationHandler.InvokeCallback();
			}
		}

		public void LogInputRops(IEnumerable<RopId> rops)
		{
			ProtocolLog.LogInputRops(rops);
		}

		public void LogPrepareForRop(RopId ropId)
		{
		}

		public void LogCompletedRop(RopId ropId, ErrorCode errorCode)
		{
			ProtocolLog.LogOutputRop(ropId, errorCode);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.ropHandler);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ConnectionHandler>(this);
		}

		internal bool ForAnyLogon(Func<Logon, bool> logonDelegate)
		{
			lock (this.logons)
			{
				foreach (Logon arg in this.logons)
				{
					if (logonDelegate(arg))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void AddLogon(Logon logon)
		{
			lock (this.logons)
			{
				this.logons.Add(logon);
			}
		}

		internal void RemoveLogon(Logon logon)
		{
			lock (this.logons)
			{
				this.logons.Remove(logon);
			}
		}

		private static TimeSpan MaximumLifetimeForAConnectionWithoutLogons
		{
			get
			{
				return TimeSpan.FromMilliseconds(2.0 * Configuration.ServiceConfiguration.RpcPollsMax.TotalMilliseconds);
			}
		}

		public static readonly HandlerFactory Factory = new HandlerFactory(ConnectionHandler.Create);

		private readonly IConnection connection;

		private readonly HashSet<Logon> logons = new HashSet<Logon>();

		private readonly RopHandler ropHandler;

		private readonly NotificationHandler notificationHandler;

		private DateTime lastTimeConnectionHadLogons = DateTime.UtcNow;
	}
}
