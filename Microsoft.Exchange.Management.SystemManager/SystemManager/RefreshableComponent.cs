using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	[DefaultEvent("RefreshingChanged")]
	public class RefreshableComponent : Component, IRefreshableNotification, IRefreshable, ISupportInitializeNotification, ISupportInitialize
	{
		public ICloneable RefreshArgument
		{
			get
			{
				return this.refreshArgument;
			}
			set
			{
				this.refreshArgument = value;
			}
		}

		protected virtual ICloneable DefaultRefreshArgument
		{
			get
			{
				return null;
			}
		}

		private bool ShouldSerializeRefreshArgument()
		{
			return this.RefreshArgument != this.DefaultRefreshArgument;
		}

		private void ResetRefreshArgument()
		{
			this.RefreshArgument = this.DefaultRefreshArgument;
		}

		protected object CloneRefreshArgument()
		{
			if (this.RefreshArgument == null)
			{
				return null;
			}
			return this.RefreshArgument.Clone();
		}

		[Browsable(false)]
		public bool Refreshing
		{
			get
			{
				return this.refreshing;
			}
			private set
			{
				if (this.Refreshing != value)
				{
					if (value)
					{
						this.Refreshed = true;
					}
					ExTraceGlobals.ProgramFlowTracer.TraceFunction<RefreshableComponent, bool>((long)this.GetHashCode(), "*--RefreshableComponent.Refreshing: {0}: {1}", this, value);
					this.refreshing = value;
					this.OnRefreshingChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnRefreshingChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[RefreshableComponent.EventRefreshingChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler RefreshingChanged
		{
			add
			{
				base.Events.AddHandler(RefreshableComponent.EventRefreshingChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(RefreshableComponent.EventRefreshingChanged, value);
			}
		}

		public bool Refreshed
		{
			get
			{
				return this.refreshed;
			}
			private set
			{
				this.refreshed = value;
			}
		}

		protected virtual void OnRefreshStarting(CancelEventArgs e)
		{
			CancelEventHandler cancelEventHandler = (CancelEventHandler)base.Events[RefreshableComponent.EventRefreshStarting];
			if (cancelEventHandler != null)
			{
				cancelEventHandler(this, e);
			}
		}

		public event CancelEventHandler RefreshStarting
		{
			add
			{
				base.Events.AddHandler(RefreshableComponent.EventRefreshStarting, value);
			}
			remove
			{
				base.Events.RemoveHandler(RefreshableComponent.EventRefreshStarting, value);
			}
		}

		public void Refresh(IProgress progress)
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			this.OnRefreshStarting(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				RefreshRequestEventArgs refreshRequest = this.CreateFullRefreshRequest(progress);
				this.RefreshCore(progress, refreshRequest);
				return;
			}
			progress.ReportProgress(100, 100, string.Empty);
		}

		protected virtual RefreshRequestEventArgs CreateFullRefreshRequest(IProgress progress)
		{
			return new RefreshRequestEventArgs(true, progress, this.CloneRefreshArgument());
		}

		protected void RefreshCore(IProgress progress, RefreshRequestEventArgs refreshRequest)
		{
			ExTraceGlobals.ProgramFlowTracer.TracePerformance<RefreshableComponent, string>(0L, "Time: {1}. Start Refresh in UI thread. -->RefreshableComponent.RefreshCore: {0}. ", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
			if (!this.IsInitialized || this.IsInCriticalState)
			{
				progress.ReportProgress(100, 100, string.Empty);
				return;
			}
			this.EnqueueRequest(progress, refreshRequest);
		}

		private void EnqueueRequest(IProgress progress, RefreshRequestEventArgs refreshRequest)
		{
			int num = 0;
			bool flag = false;
			foreach (RefreshRequestEventArgs refreshRequestEventArgs in this.refreshRequestQueue)
			{
				if (refreshRequest.Priority < refreshRequestEventArgs.Priority)
				{
					if (flag)
					{
						break;
					}
				}
				else if (refreshRequest.Priority == refreshRequestEventArgs.Priority && !refreshRequestEventArgs.Cancel)
				{
					PartialOrder partialOrder = this.ComparePartialOrder(refreshRequest, refreshRequestEventArgs);
					if (partialOrder == null || partialOrder == 1)
					{
						this.CancelRefresh(refreshRequestEventArgs);
					}
					else if (partialOrder == -1 && flag)
					{
						this.CancelRefresh(refreshRequest);
						break;
					}
				}
				num++;
				flag = true;
			}
			this.refreshRequestQueue.Insert(num, refreshRequest);
			if (this.refreshRequestQueue.Count == 1)
			{
				this.ProcessRequest(refreshRequest);
			}
		}

		protected virtual PartialOrder ComparePartialOrder(RefreshRequestEventArgs leftValue, RefreshRequestEventArgs rightValue)
		{
			PartialOrder result = int.MinValue;
			if (leftValue.IsFullRefresh)
			{
				result = (rightValue.IsFullRefresh ? 0 : 1);
			}
			else if (rightValue.IsFullRefresh)
			{
				result = -1;
			}
			else
			{
				PartialRefreshRequestEventArgs partialRefreshRequestEventArgs = leftValue as PartialRefreshRequestEventArgs;
				PartialRefreshRequestEventArgs partialRefreshRequestEventArgs2 = rightValue as PartialRefreshRequestEventArgs;
				if (!partialRefreshRequestEventArgs.Identities.IsEmptyCollection() && !partialRefreshRequestEventArgs2.Identities.IsEmptyCollection())
				{
					if (partialRefreshRequestEventArgs.Identities.Length > partialRefreshRequestEventArgs2.Identities.Length)
					{
						if (RefreshableComponent.IsSubsetOf(partialRefreshRequestEventArgs.Identities, partialRefreshRequestEventArgs2.Identities))
						{
							result = 1;
						}
					}
					else if (RefreshableComponent.IsSubsetOf(partialRefreshRequestEventArgs2.Identities, partialRefreshRequestEventArgs.Identities))
					{
						result = -1;
					}
				}
			}
			return result;
		}

		private static bool IsSubsetOf(object[] objectsSet, object[] objectsSubset)
		{
			foreach (object value in objectsSubset)
			{
				if (Array.IndexOf<object>(objectsSet, value) < 0)
				{
					return false;
				}
			}
			return true;
		}

		private void ProcessRequest(RefreshRequestEventArgs refreshRequest)
		{
			BackgroundWorker backgroundWorker = refreshRequest.BackgroundWorker;
			backgroundWorker.WorkerReportsProgress = true;
			backgroundWorker.WorkerSupportsCancellation = true;
			backgroundWorker.DoWork += this.worker_DoWork;
			backgroundWorker.RunWorkerCompleted += this.worker_RunWorkerCompleted;
			backgroundWorker.ProgressChanged += this.worker_ProgressChanged;
			SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
			backgroundWorker.RunWorkerAsync(refreshRequest);
			this.Refreshing = true;
			this.OnRefreshStarted(EventArgs.Empty);
		}

		protected virtual void OnRefreshStarted(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[RefreshableComponent.EventRefreshStarted];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler RefreshStarted
		{
			add
			{
				base.Events.AddHandler(RefreshableComponent.EventRefreshStarted, value);
			}
			remove
			{
				base.Events.RemoveHandler(RefreshableComponent.EventRefreshStarted, value);
			}
		}

		public void CancelRefresh()
		{
			foreach (RefreshRequestEventArgs refreshRequest in this.refreshRequestQueue)
			{
				this.CancelRefresh(refreshRequest);
			}
		}

		private void CancelRefresh(RefreshRequestEventArgs refreshRequest)
		{
			if (refreshRequest.CancellationPending || refreshRequest.Cancel)
			{
				return;
			}
			if (refreshRequest.BackgroundWorker.IsBusy)
			{
				refreshRequest.BackgroundWorker.CancelAsync();
				return;
			}
			refreshRequest.Cancel = true;
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TracePerformance<RefreshableComponent, string, string>(0L, "Time:{1}. Start Refresh {2} in worker thread. -->RefreshableComponent.worker_DoWork: {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), (this is DataTableLoader) ? (this as DataTableLoader).Table.TableName : string.Empty);
			RefreshRequestEventArgs refreshRequestEventArgs = (RefreshRequestEventArgs)e.Argument;
			try
			{
				this.OnDoRefreshWork(refreshRequestEventArgs);
			}
			finally
			{
				refreshRequestEventArgs.ShellProgress.ReportProgress(100, 100, "");
			}
			e.Result = refreshRequestEventArgs.Result;
			e.Cancel = refreshRequestEventArgs.CancellationPending;
			ExTraceGlobals.ProgramFlowTracer.TracePerformance<RefreshableComponent, string, string>(0L, "Time:{1}. End Refresh {2} in worker thread. <--RefreshableComponent.worker_DoWork: {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), (this is DataTableLoader) ? (this as DataTableLoader).Table.TableName : string.Empty);
		}

		protected virtual void OnDoRefreshWork(RefreshRequestEventArgs e)
		{
			RefreshRequestEventHandler refreshRequestEventHandler = (RefreshRequestEventHandler)base.Events[RefreshableComponent.EventDoRefreshWork];
			if (refreshRequestEventHandler != null)
			{
				refreshRequestEventHandler(this, e);
			}
		}

		public event RefreshRequestEventHandler DoRefreshWork
		{
			add
			{
				base.Events.AddHandler(RefreshableComponent.EventDoRefreshWork, value);
			}
			remove
			{
				base.Events.RemoveHandler(RefreshableComponent.EventDoRefreshWork, value);
			}
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			RefreshProgressChangedEventArgs refreshProgressChangedEventArgs = (RefreshProgressChangedEventArgs)e.UserState;
			this.OnProgressChanged(refreshProgressChangedEventArgs);
			if (refreshProgressChangedEventArgs.IsFirstProgressReport)
			{
				ExTraceGlobals.ProgramFlowTracer.TracePerformance<RefreshableComponent, string, string>(0L, "Time:{1}. {2} First batch data arrived in UI thread. {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), (this is DataTableLoader) ? (this as DataTableLoader).Table.TableName : string.Empty);
				this.OnFirstBatchDataArrived(EventArgs.Empty);
			}
		}

		public event EventHandler FirstBatchDataArrived;

		private void OnFirstBatchDataArrived(EventArgs e)
		{
			if (this.FirstBatchDataArrived != null)
			{
				this.FirstBatchDataArrived(this, e);
			}
		}

		protected virtual void OnProgressChanged(RefreshProgressChangedEventArgs e)
		{
			RefreshProgressChangedEventHandler refreshProgressChangedEventHandler = (RefreshProgressChangedEventHandler)base.Events[RefreshableComponent.EventProgressChanged];
			if (refreshProgressChangedEventHandler != null)
			{
				refreshProgressChangedEventHandler(this, e);
			}
		}

		public event RefreshProgressChangedEventHandler ProgressChanged
		{
			add
			{
				base.Events.AddHandler(RefreshableComponent.EventProgressChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(RefreshableComponent.EventProgressChanged, value);
			}
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			RefreshRequestEventArgs refreshRequestEventArgs = this.refreshRequestQueue[0];
			if (!refreshRequestEventArgs.Cancel)
			{
				this.DoPostRefreshAction(refreshRequestEventArgs);
			}
			this.refreshRequestQueue.RemoveAt(0);
			List<RefreshRequestEventArgs> list = new List<RefreshRequestEventArgs>();
			while (this.refreshRequestQueue.Count > 0 && this.refreshRequestQueue[0].Cancel)
			{
				list.Add(this.refreshRequestQueue[0]);
				this.refreshRequestQueue.RemoveAt(0);
			}
			if (this.refreshRequestQueue.Count > 0)
			{
				this.ProcessRequest(this.refreshRequestQueue[0]);
			}
			else
			{
				this.Refreshing = false;
			}
			this.isInCriticalState = true;
			try
			{
				if (e.Error != null && this.UIService != null)
				{
					this.UIService.ShowError(e.Error);
				}
			}
			finally
			{
				this.isInCriticalState = false;
			}
			this.OnRefreshCompleted(e);
			foreach (RefreshRequestEventArgs refreshRequestEventArgs2 in list)
			{
				refreshRequestEventArgs2.ShellProgress.ReportProgress(100, 100, "");
				this.OnRefreshCompleted(new RunWorkerCompletedEventArgs(null, null, true));
			}
			ExTraceGlobals.ProgramFlowTracer.TracePerformance<RefreshableComponent, string>(0L, "Time:{1}. End Refresh in UI thread. <--RefreshableComponent.worker_RunWorkerCompleted: {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
		}

		protected virtual void DoPostRefreshAction(RefreshRequestEventArgs refreshRequest)
		{
		}

		protected IUIService UIService
		{
			get
			{
				return (IUIService)this.GetService(typeof(IUIService));
			}
		}

		private bool IsInCriticalState
		{
			get
			{
				return this.isInCriticalState;
			}
		}

		protected virtual void OnRefreshCompleted(RunWorkerCompletedEventArgs e)
		{
			RunWorkerCompletedEventHandler runWorkerCompletedEventHandler = (RunWorkerCompletedEventHandler)base.Events[RefreshableComponent.EventRefreshCompleted];
			if (runWorkerCompletedEventHandler != null)
			{
				runWorkerCompletedEventHandler(this, e);
			}
		}

		public event RunWorkerCompletedEventHandler RefreshCompleted
		{
			add
			{
				base.Events.AddHandler(RefreshableComponent.EventRefreshCompleted, value);
			}
			remove
			{
				base.Events.RemoveHandler(RefreshableComponent.EventRefreshCompleted, value);
			}
		}

		public bool IsInitialized
		{
			get
			{
				return !this.initializing;
			}
		}

		public void BeginInit()
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<RefreshableComponent>((long)this.GetHashCode(), "*--RefreshableComponent.BeginInit: {0}", this);
			this.initializing = true;
		}

		public void EndInit()
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<RefreshableComponent>((long)this.GetHashCode(), "*--RefreshableComponent.EndInit: {0}", this);
			this.initializing = false;
			this.OnInitialized(EventArgs.Empty);
		}

		protected virtual void OnInitialized(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[RefreshableComponent.EventInitialized];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler Initialized
		{
			add
			{
				base.Events.AddHandler(RefreshableComponent.EventInitialized, value);
			}
			remove
			{
				base.Events.RemoveHandler(RefreshableComponent.EventInitialized, value);
			}
		}

		private ICloneable refreshArgument;

		private bool refreshing;

		private bool initializing;

		private bool isInCriticalState;

		private static readonly object EventRefreshingChanged = new object();

		private bool refreshed;

		private static readonly object EventRefreshStarting = new object();

		private List<RefreshRequestEventArgs> refreshRequestQueue = new List<RefreshRequestEventArgs>();

		private static readonly object EventRefreshStarted = new object();

		private static readonly object EventDoRefreshWork = new object();

		private static readonly object EventProgressChanged = new object();

		private static readonly object EventRefreshCompleted = new object();

		private static readonly object EventInitialized = new object();
	}
}
