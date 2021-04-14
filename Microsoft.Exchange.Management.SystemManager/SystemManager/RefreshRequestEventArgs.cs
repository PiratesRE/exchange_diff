using System;
using System.ComponentModel;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class RefreshRequestEventArgs : DoWorkEventArgs
	{
		public RefreshRequestEventArgs(bool isFullRefresh, IProgress progress, object argument, RefreshRequestPriority priority) : base(argument)
		{
			if (progress == null)
			{
				throw new ArgumentNullException("progress");
			}
			this.isFullRefresh = isFullRefresh;
			this.priority = priority;
			this.progress = progress;
			this.worker = new BackgroundWorker();
		}

		public RefreshRequestEventArgs(bool isFullRefresh, IProgress progress, object argument) : this(isFullRefresh, progress, argument, 0)
		{
		}

		internal IProgress ShellProgress
		{
			get
			{
				return this.progress;
			}
		}

		public BackgroundWorker BackgroundWorker
		{
			get
			{
				return this.worker;
			}
		}

		public bool ReportedProgress
		{
			get
			{
				return this.reportedProgress;
			}
		}

		public bool IsFullRefresh
		{
			get
			{
				return this.isFullRefresh;
			}
		}

		public RefreshRequestPriority Priority
		{
			get
			{
				return this.priority;
			}
		}

		public bool CancellationPending
		{
			get
			{
				return this.worker.CancellationPending || this.ShellProgress.Canceled;
			}
		}

		public void ReportProgress(int workProcessed, int totalWork, string statusText, object progressState)
		{
			RefreshProgressChangedEventArgs refreshProgressChangedEventArgs = new RefreshProgressChangedEventArgs(this, workProcessed, totalWork, statusText, progressState);
			this.reportedProgress = true;
			this.ShellProgress.ReportProgress(workProcessed, totalWork, statusText);
			this.worker.ReportProgress(refreshProgressChangedEventArgs.ProgressPercentage, refreshProgressChangedEventArgs);
		}

		private bool reportedProgress;

		private IProgress progress;

		private BackgroundWorker worker;

		private bool isFullRefresh;

		private RefreshRequestPriority priority;
	}
}
