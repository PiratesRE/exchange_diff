using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal partial class BackgroundWorkerProgressDialog : ProgressDialog
	{
		public BackgroundWorkerProgressDialog()
		{
			this.backgroundWorker = new BackgroundWorker();
			this.backgroundWorker.DoWork += this.backgroundWorker_DoWork;
			this.backgroundWorker.RunWorkerCompleted += this.backgroundWorker_RunWorkerCompleted;
			base.UseMarquee = true;
		}

		public event DoWorkEventHandler DoWork;

		public event RunWorkerCompletedEventHandler RunWorkerCompleted;

		public bool IsBusy
		{
			get
			{
				return this.backgroundWorker.IsBusy;
			}
		}

		public void ReportProgress(int percentProgress, string statusText)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new BackgroundWorkerProgressDialog.ReportProgressDelegate(this.ReportProgress), new object[]
				{
					percentProgress,
					statusText
				});
				return;
			}
			base.UseMarquee = false;
			base.Value = percentProgress;
			base.StatusText = statusText;
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			base.UseMarquee = false;
			base.Maximum = 100;
			base.Value = 100;
			this.runWorkerCompletedEventArgs = e;
			if (this.RunWorkerCompleted != null)
			{
				this.RunWorkerCompleted(this, e);
			}
			base.Close();
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (this.DoWork != null)
			{
				this.DoWork(this, e);
			}
		}

		public bool ShowErrors(string errorMessage, string warningMessage, WorkUnitCollection workUnits, IUIService uiService)
		{
			Exception error = this.runWorkerCompletedEventArgs.Error;
			if (error != null)
			{
				uiService.ShowError(error);
				return true;
			}
			IList<WorkUnit> errors = workUnits.FindByErrorOrWarning();
			return UIService.ShowError(errorMessage, warningMessage, errors, uiService);
		}

		public object AsyncResults
		{
			get
			{
				if (this.runWorkerCompletedEventArgs != null && this.runWorkerCompletedEventArgs.Error == null)
				{
					return this.runWorkerCompletedEventArgs.Result;
				}
				return null;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = this.backgroundWorker.IsBusy;
			base.OnClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
			this.backgroundWorker.RunWorkerAsync();
		}

		private RunWorkerCompletedEventArgs runWorkerCompletedEventArgs;

		private delegate void ReportProgressDelegate(int percentProgress, string statusText);
	}
}
