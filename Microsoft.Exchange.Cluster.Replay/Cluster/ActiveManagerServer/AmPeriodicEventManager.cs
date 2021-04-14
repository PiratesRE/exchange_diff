using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmPeriodicEventManager
	{
		internal AmPeriodicEventManager()
		{
		}

		internal bool IsExiting { get; private set; }

		internal static bool IsPeriodicEvent(AmEvtBase evt)
		{
			return evt is AmEvtPeriodicDbStateAnalyze || evt is AmEvtPeriodicCheckMismountedDatabase;
		}

		internal void EnqueuePeriodicEventIfRequired()
		{
			if (!RegistryParameters.AmPeriodicDatabaseAnalyzerEnabled)
			{
				return;
			}
			if (this.IsExiting)
			{
				return;
			}
			lock (this.m_locker)
			{
				if (this.m_checkDbWatch == null)
				{
					this.m_checkDbWatch = new Stopwatch();
					this.m_checkDbWatch.Reset();
					this.m_checkDbWatch.Start();
				}
			}
			if (this.m_checkDbWatch.ElapsedMilliseconds >= (long)RegistryParameters.AmPeriodicDatabaseAnalyzerIntervalInMSec)
			{
				AmConfig config = AmSystemManager.Instance.Config;
				if (config.IsDecidingAuthority)
				{
					AmEvtPeriodicDbStateAnalyze amEvtPeriodicDbStateAnalyze = new AmEvtPeriodicDbStateAnalyze();
					amEvtPeriodicDbStateAnalyze.Notify();
				}
				else if (config.IsSAM)
				{
					AmEvtPeriodicCheckMismountedDatabase amEvtPeriodicCheckMismountedDatabase = new AmEvtPeriodicCheckMismountedDatabase();
					amEvtPeriodicCheckMismountedDatabase.Notify();
				}
				this.m_checkDbWatch.Reset();
				this.m_checkDbWatch.Start();
			}
		}

		internal bool EnqueueDeferredSystemEvent(AmEvtBase evt, int after)
		{
			bool result;
			lock (this.m_locker)
			{
				if (this.m_isInUse || this.IsExiting)
				{
					result = false;
				}
				else
				{
					this.m_isInUse = true;
					if (this.m_deferredTimer == null)
					{
						this.m_deferredTimer = new Timer(new TimerCallback(this.DeferredActionCallback), null, -1, -1);
					}
					this.m_deferredEvt = evt;
					this.m_deferredTimer.Change(after, -1);
					result = true;
				}
			}
			return result;
		}

		internal void DeferredActionCallback(object unused)
		{
			lock (this.m_locker)
			{
				this.m_deferredTimer.Change(-1, -1);
				if (!this.IsExiting)
				{
					this.m_deferredEvt.Notify();
				}
				this.m_isInUse = false;
			}
		}

		internal void Stop()
		{
			lock (this.m_locker)
			{
				this.IsExiting = true;
				if (this.m_deferredTimer != null)
				{
					this.m_deferredTimer.Change(-1, -1);
				}
				if (this.m_checkDbWatch != null)
				{
					this.m_checkDbWatch.Reset();
				}
			}
		}

		private object m_locker = new object();

		private Timer m_deferredTimer;

		private AmEvtBase m_deferredEvt;

		private bool m_isInUse;

		private Stopwatch m_checkDbWatch;
	}
}
