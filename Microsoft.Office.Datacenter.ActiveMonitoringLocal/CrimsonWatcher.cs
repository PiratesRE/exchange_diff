using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal abstract class CrimsonWatcher<T> : CrimsonOperation<T> where T : IPersistence, new()
	{
		internal CrimsonWatcher(EventBookmark bookmark, bool isReadExistingEvents) : this(bookmark, isReadExistingEvents, null)
		{
		}

		internal CrimsonWatcher(EventBookmark bookmark, bool isReadExistingEvents, string channelName) : base(bookmark, channelName)
		{
			this.isReadExistingEvents = isReadExistingEvents;
		}

		internal void InitializeIfRequired()
		{
			if (!base.IsInitialized)
			{
				EventLogQuery queryObject = base.GetQueryObject();
				EventBookmark bookMark = base.BookMark;
				this.watcher = new EventLogWatcher(queryObject, bookMark, this.isReadExistingEvents);
				this.watcher.EventRecordWritten += this.EventArrivedHandler;
				base.IsInitialized = true;
			}
		}

		internal override void Cleanup()
		{
			this.Stop();
			lock (this.locker)
			{
				if (this.watcher != null)
				{
					this.watcher.Dispose();
					this.watcher = null;
				}
			}
		}

		internal void Start(bool isSyncMode)
		{
			if (this.stopRequested)
			{
				return;
			}
			this.InitializeIfRequired();
			if (isSyncMode)
			{
				this.watcher.Enabled = true;
				return;
			}
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				lock (this.locker)
				{
					if (!this.stopRequested)
					{
						this.watcher.Enabled = true;
					}
				}
			});
		}

		internal void Start()
		{
			this.Start(false);
		}

		internal void Stop()
		{
			this.stopRequested = true;
			this.watcher.Enabled = false;
		}

		protected void EventArrivedHandler(object sender, EventRecordWrittenEventArgs arg)
		{
			try
			{
				if (!this.stopRequested)
				{
					if (arg != null && arg.EventRecord != null)
					{
						using (EventRecord eventRecord = arg.EventRecord)
						{
							T o = base.EventToObject(eventRecord);
							this.ResultArrivedHandler(o);
						}
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.DataAccessTracer, TracingContext.Default, string.Format("EventArrivedHandler excepiton: {0}", ex.ToString()), null, "EventArrivedHandler", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\CrimsonWatcher.cs", 187);
			}
		}

		protected abstract void ResultArrivedHandler(T o);

		private readonly bool isReadExistingEvents;

		private EventLogWatcher watcher;

		private object locker = new object();

		private bool stopRequested;
	}
}
