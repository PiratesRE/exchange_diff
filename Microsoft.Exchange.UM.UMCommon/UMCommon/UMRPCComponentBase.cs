using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class UMRPCComponentBase : IUMAsyncComponent
	{
		public AutoResetEvent StoppedEvent
		{
			get
			{
				return this.controlEvent;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.initialized;
			}
		}

		public string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		public static void HandleException(Exception ex)
		{
			if (ex is LocalizedException)
			{
				return;
			}
			if (GrayException.IsGrayException(ex))
			{
				ExWatson.SendReport(ex);
				return;
			}
			CrashProcess.Instance.CrashThisProcess(ex);
		}

		public void StartNow(StartupStage stage)
		{
			if (stage == StartupStage.WPActivation)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "{0} starting in stage {1}", new object[]
				{
					this.Name,
					stage
				});
				this.activeRequestCount = 0;
				this.shutdownInProgress = false;
				this.RegisterServer();
				this.initialized = true;
			}
		}

		public void StopAsync()
		{
			this.controlEvent.Reset();
			lock (this.lockObj)
			{
				this.shutdownInProgress = true;
				if (this.activeRequestCount <= 0)
				{
					this.controlEvent.Set();
				}
			}
		}

		public void CleanupAfterStopped()
		{
			this.controlEvent.Close();
		}

		internal bool GuardBeforeExecution()
		{
			bool result;
			lock (this.lockObj)
			{
				if (this.shutdownInProgress)
				{
					result = false;
				}
				else
				{
					this.activeRequestCount++;
					result = true;
				}
			}
			return result;
		}

		internal void GuardAfterExecution()
		{
			lock (this.lockObj)
			{
				this.activeRequestCount--;
				if (this.shutdownInProgress && this.activeRequestCount <= 0)
				{
					this.controlEvent.Set();
				}
			}
		}

		internal abstract void RegisterServer();

		private AutoResetEvent controlEvent = new AutoResetEvent(false);

		private object lockObj = new object();

		private bool shutdownInProgress;

		private bool initialized;

		private int activeRequestCount;
	}
}
