using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal partial class InvisibleForm : ExchangeForm
	{
		public InvisibleForm()
		{
			this.AutoSize = true;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.ShowInTaskbar = false;
			base.ShowIcon = false;
			base.MinimizeBox = false;
			base.MinimizeBox = false;
			base.ControlBox = false;
			base.Size = new Size(0, 0);
			base.Opacity = 0.0;
			this.backgroundWorker = new BackgroundWorker();
			this.backgroundWorker.RunWorkerCompleted += this.backgroundWorker_RunWorkerCompleted;
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.runWorkerCompletedEventArgs = e;
			base.Close();
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

		public BackgroundWorker BackgroundWorker
		{
			get
			{
				return this.backgroundWorker;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = this.backgroundWorker.IsBusy;
			base.OnClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
			this.backgroundWorker.RunWorkerAsync();
		}

		protected override string DefaultHelpTopic
		{
			get
			{
				return string.Empty;
			}
		}

		private RunWorkerCompletedEventArgs runWorkerCompletedEventArgs;
	}
}
