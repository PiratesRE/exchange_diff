using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PrimingDispatchDriver : DisposeTrackableBase, IDispatchDriver
	{
		public PrimingDispatchDriver(SyncLogSession syncLogSession, TimeSpan primingDispatchTime)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.syncLogSession = syncLogSession;
			this.workThreadsEvent = new ManualResetEvent(true);
			this.primerTimerWorker = new Timer(new TimerCallback(this.PrimingWorkerCallback), null, primingDispatchTime, primingDispatchTime);
		}

		public event EventHandler<EventArgs> PrimingEvent
		{
			add
			{
				this.InternalPrimingEvent += value;
			}
			remove
			{
				this.InternalPrimingEvent -= value;
			}
		}

		private event EventHandler<EventArgs> InternalPrimingEvent;

		public void AddDiagnosticInfoTo(XElement componentElement)
		{
			componentElement.Add(new XElement("lastPrimerStartTime", (this.lastPrimerStartTime != null) ? this.lastPrimerStartTime.Value.ToString("o") : string.Empty));
			TimeSpan? timeSpan = null;
			if (this.lastPrimerStartTime != null)
			{
				timeSpan = new TimeSpan?(ExDateTime.UtcNow - this.lastPrimerStartTime.Value);
			}
			componentElement.Add(new XElement("timeSinceLastPrimerStart", (timeSpan != null) ? timeSpan.Value.ToString() : string.Empty));
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.primerTimerWorker != null)
			{
				this.workThreadsEvent.WaitOne();
				this.primerTimerWorker.Dispose();
				this.primerTimerWorker = null;
				this.workThreadsEvent.Close();
				this.workThreadsEvent = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PrimingDispatchDriver>(this);
		}

		private void PrimingWorkerCallback(object state)
		{
			this.syncLogSession.LogDebugging((TSLID)431UL, "PrimingWorkerCallback", new object[0]);
			if (Monitor.TryEnter(this.primerLock))
			{
				this.workThreadsEvent.Reset();
				try
				{
					this.lastPrimerStartTime = new ExDateTime?(ExDateTime.UtcNow);
					if (this.InternalPrimingEvent != null)
					{
						this.InternalPrimingEvent(this, null);
					}
					return;
				}
				finally
				{
					this.workThreadsEvent.Set();
					Monitor.Exit(this.primerLock);
				}
			}
			this.syncLogSession.LogDebugging((TSLID)432UL, "Primer is still running, unable to run again this interval", new object[0]);
		}

		private Timer primerTimerWorker;

		private object primerLock = new object();

		private ManualResetEvent workThreadsEvent;

		private SyncLogSession syncLogSession;

		private ExDateTime? lastPrimerStartTime = null;
	}
}
