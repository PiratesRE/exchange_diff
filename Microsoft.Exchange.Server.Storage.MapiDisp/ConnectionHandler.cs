using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	public class ConnectionHandler : DisposableBase, IConnectionHandler, IDisposable
	{
		private ConnectionHandler(MapiSession mapiSession)
		{
			this.ropHandler = new RopHandler(mapiSession);
			this.notificationHandler = new NotificationHandler(mapiSession);
		}

		public static ConnectionHandler Create(MapiSession mapiSession)
		{
			return new ConnectionHandler(mapiSession);
		}

		IRopHandler IConnectionHandler.RopHandler
		{
			get
			{
				return this.ropHandler;
			}
		}

		INotificationHandler IConnectionHandler.NotificationHandler
		{
			get
			{
				return this.notificationHandler;
			}
		}

		void IConnectionHandler.BeginRopProcessing(AuxiliaryData auxiliaryData)
		{
			RopHandler ropHandler = (RopHandler)this.ropHandler;
			MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)ropHandler.MapiContext.Diagnostics;
			JET_THREADSTATS threadStats;
			Factory.GetDatabaseThreadStats(out threadStats);
			mapiExecutionDiagnostics.MapiExMonLogger.BeginRopProcessing(threadStats);
		}

		void IConnectionHandler.EndRopProcessing(AuxiliaryData auxiliaryData)
		{
			RopHandler ropHandler = (RopHandler)this.ropHandler;
			MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)ropHandler.MapiContext.Diagnostics;
			mapiExecutionDiagnostics.MapiExMonLogger.EndRopProcessing();
		}

		void IConnectionHandler.LogInputRops(IEnumerable<RopId> rops)
		{
			RopHandler ropHandler = (RopHandler)this.ropHandler;
			MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)ropHandler.MapiContext.Diagnostics;
			mapiExecutionDiagnostics.MapiExMonLogger.LogInputRops(rops);
		}

		void IConnectionHandler.LogPrepareForRop(RopId ropId)
		{
			RopHandler ropHandler = (RopHandler)this.ropHandler;
			MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)ropHandler.MapiContext.Diagnostics;
			mapiExecutionDiagnostics.OnRopBegin(ropId);
		}

		void IConnectionHandler.LogCompletedRop(RopId ropId, ErrorCode errorCode)
		{
			RopHandler ropHandler = (RopHandler)this.ropHandler;
			MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)ropHandler.MapiContext.Diagnostics;
			mapiExecutionDiagnostics.OnRopEnd(ropId, errorCode);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ConnectionHandler>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.ropHandler != null)
			{
				this.ropHandler.Dispose();
				this.ropHandler = null;
			}
		}

		private IRopHandler ropHandler;

		private INotificationHandler notificationHandler;
	}
}
