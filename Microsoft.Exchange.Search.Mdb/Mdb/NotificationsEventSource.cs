using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class NotificationsEventSource : INotificationsEventSource, IDisposeTrackable, IDisposable
	{
		internal NotificationsEventSource(MdbInfo mdbInfo)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("NotificationsEventSource", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbNotificationsFeederTracer, (long)this.GetHashCode());
			this.mdbInfo = mdbInfo;
			this.componentGuid = ComponentInstance.Globals.Search.Id;
			this.disposeTracker = this.GetDisposeTracker();
		}

		private MapiEventManager EventManager
		{
			get
			{
				if (this.eventManager == null)
				{
					this.RefreshEventManager();
				}
				return this.eventManager;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationsEventSource>(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public MapiEvent[] ReadEvents(long startCounter, int eventCountWanted, ReadEventsFlags flags, out long endCounter)
		{
			long outEndCounter = 0L;
			MapiEvent[] result;
			lock (this.lockObject)
			{
				this.CheckDisposed();
				try
				{
					MapiEvent[] array = MapiUtil.TranslateMapiExceptionsWithReturnValue<MapiEvent[]>(this.diagnosticsSession, Strings.FailedToReadNotifications(this.mdbInfo.Guid), () => this.EventManager.ReadEvents(startCounter, eventCountWanted, 0, null, flags, false, out outEndCounter));
					endCounter = outEndCounter;
					result = array;
				}
				catch (ComponentException arg)
				{
					this.diagnosticsSession.TraceError<ComponentException>("Failed to read events. Exception = {0}", arg);
					this.CleanupEventManager();
					throw;
				}
			}
			return result;
		}

		public MapiEvent ReadLastEvent()
		{
			MapiEvent result;
			lock (this.lockObject)
			{
				this.CheckDisposed();
				try
				{
					result = MapiUtil.TranslateMapiExceptionsWithReturnValue<MapiEvent>(this.diagnosticsSession, Strings.FailedToReadNotifications(this.mdbInfo.Guid), () => this.EventManager.ReadLastEvent(false));
				}
				catch (ComponentException arg)
				{
					this.diagnosticsSession.TraceError<ComponentException>("Failed to read last event. Exception = {0}", arg);
					this.CleanupEventManager();
					throw;
				}
			}
			return result;
		}

		public long ReadFirstEventCounter()
		{
			this.diagnosticsSession.TraceDebug("Get the first event in the system", new object[0]);
			long num;
			MapiEvent[] array = this.ReadEvents(0L, 1, ReadEventsFlags.IncludeMoveDestinationEvents, out num);
			num = ((array.Length > 0) ? array[0].EventCounter : num);
			this.diagnosticsSession.TraceDebug<long>("Event {0} was found.", num);
			return num;
		}

		public long GetNetworkLatency(int samples)
		{
			if (samples <= 0)
			{
				throw new ArgumentOutOfRangeException("samples");
			}
			this.diagnosticsSession.TraceDebug<MdbInfo>("Calculating network latency for {0}", this.mdbInfo);
			this.ReadLastEvent();
			Stopwatch stopwatch = new Stopwatch();
			long num = 0L;
			for (int i = 0; i < samples; i++)
			{
				if (i != 0)
				{
					Thread.Sleep(TimeSpan.FromSeconds(1.0));
				}
				stopwatch.Restart();
				this.ReadLastEvent();
				stopwatch.Stop();
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				long timeInStore = this.GetTimeInStore();
				num += elapsedMilliseconds - timeInStore;
				this.diagnosticsSession.TraceDebug<int, long, long>("Sample {0}, elapsed: {1} ms, time in store: {2} ms", i + 1, elapsedMilliseconds, timeInStore);
			}
			long num2 = num / (long)samples;
			this.diagnosticsSession.TraceDebug<MdbInfo, long>("Network latency for {0}: {1} ms", this.mdbInfo, num2);
			return num2;
		}

		private void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.lockObject)
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
					this.disposed = true;
					this.CleanupEventManager();
				}
			}
		}

		private long GetTimeInStore()
		{
			long result;
			lock (this.lockObject)
			{
				this.CheckDisposed();
				try
				{
					result = MapiUtil.TranslateMapiExceptionsWithReturnValue<long>(this.diagnosticsSession, Strings.FailedToReadNotifications(this.mdbInfo.Guid), () => (long)this.exRpcAdmin.GetStorePerRPCStats().timeInServer.TotalMilliseconds);
				}
				catch (ComponentException arg)
				{
					this.diagnosticsSession.TraceError<ComponentException>("Failed to calculate network latency. Exception = {0}", arg);
					this.CleanupEventManager();
					throw;
				}
			}
			return result;
		}

		private void CleanupEventManager()
		{
			this.diagnosticsSession.TraceDebug("Clean up event manager", new object[0]);
			this.eventManager = null;
			if (this.exRpcAdmin != null)
			{
				this.exRpcAdmin.Dispose();
				this.exRpcAdmin = null;
			}
		}

		private void RefreshEventManager()
		{
			this.diagnosticsSession.TraceDebug<MdbInfo>("Refresh a new event manager for server {0}", this.mdbInfo);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ISearchServiceConfig searchConfig = Factory.Current.CreateSearchServiceConfig();
				this.exRpcAdmin = MapiUtil.TranslateMapiExceptionsWithReturnValue<ExRpcAdmin>(this.diagnosticsSession, Strings.FailedToOpenAdminRpcConnection, () => ExRpcAdmin.Create("Client=CI", (searchConfig.ReadFromPassiveEnabled && !this.mdbInfo.IsLagCopy) ? LocalServer.GetServer().Fqdn : this.mdbInfo.OwningServer, null, null, null));
				disposeGuard.Add<ExRpcAdmin>(this.exRpcAdmin);
				this.eventManager = MapiUtil.TranslateMapiExceptionsWithReturnValue<MapiEventManager>(this.diagnosticsSession, Strings.FailedCreateEventManager(this.mdbInfo.Guid), () => MapiEventManager.Create(this.exRpcAdmin, this.componentGuid, this.mdbInfo.Guid));
				disposeGuard.Success();
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly MdbInfo mdbInfo;

		private readonly Guid componentGuid;

		private readonly object lockObject = new object();

		private IDiagnosticsSession diagnosticsSession;

		private ExRpcAdmin exRpcAdmin;

		private MapiEventManager eventManager;

		private volatile bool disposed;

		private DisposeTracker disposeTracker;
	}
}
