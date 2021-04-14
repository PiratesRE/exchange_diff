using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ProgressTridentsWizardPage : TridentsWizardPage
	{
		public ProgressTridentsWizardPage()
		{
			this.InitializeComponent();
			this.Text = Strings.WizardCompletionTitleText;
		}

		private void InitializeComponent()
		{
			this.worker = new BackgroundWorker();
			((ISupportInitialize)base.BindingSource).BeginInit();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			this.worker.WorkerSupportsCancellation = true;
			this.worker.DoWork += this.worker_DoWork;
			this.worker.RunWorkerCompleted += this.worker_RunWorkerCompleted;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Name = "ProgressTridentsWizardPage";
			((ISupportInitialize)base.BindingSource).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		[DefaultValue(false)]
		public bool NeverRunTaskAgain
		{
			get
			{
				return this.neverRunTaskAgain;
			}
			set
			{
				this.neverRunTaskAgain = value;
			}
		}

		[DefaultValue(false)]
		public bool RunTaskOnSetActive
		{
			get
			{
				return this.runTaskOnSetActive;
			}
			set
			{
				this.runTaskOnSetActive = value;
			}
		}

		protected override void OnContextChanged(EventArgs e)
		{
			base.OnContextChanged(e);
			if (base.DataHandler != null)
			{
				base.WorkUnits = base.DataHandler.WorkUnits;
				return;
			}
			base.WorkUnits = null;
		}

		protected override void OnSetActive(EventArgs e)
		{
			base.ElapsedTimeText = string.Empty;
			string text = base.ShortDescription.Trim();
			if (string.IsNullOrEmpty(text))
			{
				if (base.Wizard != null && base.Wizard.ParentForm is WizardForm)
				{
					string action = ExchangeUserControl.RemoveAccelerator(base.NextButtonText);
					text = Strings.WizardWillUseConfigurationBelow(action);
				}
				else
				{
					text = base.DataHandler.InProgressDescription.Trim();
				}
				base.ShortDescription = text;
			}
			base.WorkUnitsPanel.TaskState = 0;
			this.scenario = this.GetPageScenario();
			if (base.CheckReadOnlyAndDisablePage())
			{
				base.CanGoForward = false;
				this.allowTaskToRun = false;
				base.CanCancel = true;
				base.InformationDescription = string.Empty;
				base.Status = string.Empty;
				if (base.Context != null)
				{
					base.ShortDescription = Strings.WizardCannotEditObject(base.DataHandler.ObjectReadOnlyReason);
				}
				base.WorkUnitsPanel.Visible = false;
			}
			else if (base.DataHandler != null && this.NeedToRunTask)
			{
				CollapsiblePanel.Animate = false;
				base.WorkUnitsPanel.SuspendLayout();
				base.DataHandler.WorkUnits.RaiseListChangedEvents = false;
				try
				{
					base.DataHandler.UpdateWorkUnits();
					WizardForm wizardForm = (WizardForm)base.FindForm();
					if (wizardForm != null && wizardForm.Icon != null)
					{
						foreach (WorkUnit workUnit in base.WorkUnits)
						{
							if (workUnit.Icon == null)
							{
								workUnit.Icon = wizardForm.Icon;
							}
							workUnit.Status = WorkUnitStatus.NotStarted;
						}
					}
				}
				finally
				{
					base.Status = base.DataHandler.WorkUnits.Description;
					base.WorkUnitsPanel.ResumeLayout(false);
					base.DataHandler.WorkUnits.RaiseListChangedEvents = true;
					base.DataHandler.WorkUnits.ResetBindings();
					CollapsiblePanel.Animate = true;
				}
			}
			base.OnSetActive(e);
			if ((this.RunTaskOnSetActive || this.scenario.RunTaskOnSetActive) && this.NeedToRunTask)
			{
				this.StartTheTask();
			}
		}

		protected override void OnGoBack(CancelEventArgs e)
		{
			base.OnGoBack(e);
			if (!e.Cancel)
			{
				this.NeedToRetryTask = false;
			}
		}

		protected override void OnGoForward(CancelEventArgs e)
		{
			base.OnGoForward(e);
			if (e.Cancel)
			{
				return;
			}
			if ((this.NeedToRunTask && !this.RunTaskOnSetActive && !this.scenario.RunTaskOnSetActive) || this.NeedToRetryTask)
			{
				this.NeedToRetryTask = false;
				this.StartTheTask();
				e.Cancel = true;
			}
			if (!e.Cancel)
			{
				this.allowTaskToRun = false;
				if (base.Wizard != null)
				{
					for (int i = 0; i <= this.PageIndex - 1; i++)
					{
						base.Wizard.WizardPages[i].Enabled = false;
					}
				}
			}
		}

		private void StartTheTask()
		{
			base.SuspendLayout();
			try
			{
				foreach (WorkUnit workUnit in base.DataHandler.WorkUnits)
				{
					workUnit.Status = WorkUnitStatus.NotStarted;
				}
				base.WorkUnitsPanel.TaskState = 1;
				base.ElapsedTimeText = base.WorkUnits.ElapsedTimeText;
				base.IsDirty = true;
				base.CanGoForward = false;
				base.CanGoBack = false;
				base.CanFinish = false;
				base.CanCancel = (this.CanCancelDataHandler && !this.scenario.DisableCancelDuringTask);
				if ((this.scenario.AutomaticallyMoveNext || this.scenario.ShowCompletionText) && base.DataHandler != null)
				{
					base.ShortDescription = base.DataHandler.InProgressDescription.Trim();
				}
				SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
				this.worker.RunWorkerAsync();
			}
			finally
			{
				base.ResumeLayout();
			}
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (base.DataHandler != null)
			{
				base.DataHandler.Save(new WinFormsCommandInteractionHandler(base.ShellUI));
			}
		}

		private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			base.WorkUnitsPanel.TaskState = 2;
			base.ElapsedTimeText = base.WorkUnits.ElapsedTimeText;
			if (e.Error != null && !ExceptionHelper.IsWellknownCommandExecutionException(e.Error))
			{
				base.ShellUI.ShowError(e.Error);
			}
			if (base.DataHandler != null && base.DataHandler.WorkUnits.Cancelled)
			{
				base.Status = Strings.TheWizardWasCancelled;
				base.ShortDescription = (this.scenario.ShowCompletionText ? (Strings.WizardCancelledNotAllActionsCompleted + " " + Strings.FinishWizardDescription) : Strings.WizardCancelledNotAllActionsCompleted);
			}
			else if (this.scenario.ShowCompletionText && base.DataHandler != null)
			{
				base.Status = base.DataHandler.CompletionStatus;
				string text = base.DataHandler.CompletionDescription.Trim();
				if (string.IsNullOrEmpty(text))
				{
					if (base.Wizard != null && base.Wizard.ParentForm is WizardForm)
					{
						if (base.DataHandler.IsSucceeded)
						{
							text = (base.WorkUnits.AllCompleted ? Strings.WizardCompletionSucceededDescription : Strings.WizardCompletionPartialSucceededDescription);
						}
						else
						{
							text = Strings.WizardCompletionFailedDescription;
						}
					}
					text = text + " " + Strings.FinishWizardDescription;
				}
				base.ShortDescription = text;
			}
			else if (base.DataHandler != null && this.scenario.AutomaticallyMoveNext)
			{
				base.Status = base.DataHandler.CompletionStatus;
				base.ShortDescription = base.DataHandler.CompletionDescription;
			}
			bool flag = e.Error != null || (base.DataHandler != null && base.DataHandler.WorkUnits.HasFailures);
			this.runTaskIfAllowed = (flag ? this.scenario.CanRunTaskAgainIfFail : this.scenario.CanRunTaskAgainIfOK);
			if (this.scenario.CanDoRetryIfFail && flag)
			{
				this.NeedToRetryTask = true;
			}
			base.CanGoForward = (!flag || this.scenario.CanGoForwardIfFail || this.NeedToRetryTask);
			base.CanGoBack = ((flag ? this.scenario.CanRunTaskAgainIfFail : this.scenario.CanRunTaskAgainIfOK) && !base.DataHandler.Cancelled && !this.NeverRunTaskAgain);
			base.CanFinish = true;
			if (this.scenario.DisableCancelDuringTask || !this.CanCancelDataHandler)
			{
				base.CanCancel = true;
			}
			if (this.scenario.DisableCancelAfterTask)
			{
				base.CanCancel = false;
			}
			this.OnTaskCompleted(e);
			bool flag2 = flag ? this.scenario.DisableTaskIfFail : this.scenario.DisableTaskIfOK;
			if (flag2)
			{
				this.allowTaskToRun = false;
				base.NextButtonText = Strings.Next;
			}
			if (base.Wizard != null)
			{
				base.Wizard.IsDataChanged = base.DataHandler.WorkUnits.IsDataChanged;
				bool flag3 = flag ? this.scenario.DisablePreviousPagesIfFail : this.scenario.DisablePreviousPagesIfOK;
				if (flag3)
				{
					for (int i = 0; i <= this.PageIndex - 1; i++)
					{
						base.Wizard.WizardPages[i].Enabled = false;
					}
				}
				bool flag4 = this.scenario.MoveNextIfNoTask && base.DataHandler != null && !base.DataHandler.HasWorkUnits;
				if (this.scenario.AutomaticallyMoveNext || flag4)
				{
					base.Wizard.CurrentPageIndex++;
				}
			}
			if (this.closeOnFinish)
			{
				Form form = base.FindForm();
				if (form != null)
				{
					form.Close();
				}
			}
		}

		public event EventHandler TaskComplete;

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnTaskCompleted(RunWorkerCompletedEventArgs e)
		{
			if (this.TaskComplete != null)
			{
				this.TaskComplete(this, e);
			}
		}

		protected override void OnCancel(CancelEventArgs e)
		{
			base.OnCancel(e);
			if (!e.Cancel && this.worker.IsBusy)
			{
				this.closeOnFinish = this.scenario.CloseWizardIfCancelled;
				if (base.CanCancel)
				{
					base.CanCancel = false;
					if (base.DataHandler != null)
					{
						WinformsHelper.InvokeAsync(delegate
						{
							base.DataHandler.Cancel();
						}, this);
					}
				}
				e.Cancel = (this.worker.IsBusy || !this.closeOnFinish);
			}
		}

		private bool NeedToRetryTask
		{
			get
			{
				return this.needToRetryTask;
			}
			set
			{
				if (value != this.NeedToRetryTask)
				{
					this.needToRetryTask = value;
					if (this.NeedToRetryTask)
					{
						this.previousNextButtonText = base.NextButtonText;
						base.NextButtonText = Strings.Retry;
						return;
					}
					if (!string.IsNullOrEmpty(this.previousNextButtonText))
					{
						base.NextButtonText = this.previousNextButtonText;
					}
				}
			}
		}

		protected int PageIndex
		{
			get
			{
				if (base.Wizard == null)
				{
					return -1;
				}
				return base.Wizard.WizardPages.IndexOf(this);
			}
		}

		private bool CanCancelDataHandler
		{
			get
			{
				return base.DataHandler != null && base.DataHandler.CanCancel;
			}
		}

		protected bool NeedToRunTask
		{
			get
			{
				return this.allowTaskToRun && this.runTaskIfAllowed;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Padding Padding
		{
			get
			{
				return base.Padding;
			}
			set
			{
				base.Padding = value;
			}
		}

		private ProgressTridentsWizardPage.Scenario GetPageScenario()
		{
			if (base.Wizard == null)
			{
				return ProgressTridentsWizardPage.Scenario.LightPage;
			}
			int pageIndex = this.PageIndex;
			if (pageIndex == base.Wizard.WizardPages.Count - 1)
			{
				return ProgressTridentsWizardPage.Scenario.LightPage;
			}
			if (base.Wizard.WizardPages[pageIndex + 1] is ProgressTridentsWizardPage)
			{
				return ProgressTridentsWizardPage.Scenario.PreReqPage;
			}
			if (pageIndex > 0 && base.Wizard.WizardPages[pageIndex - 1] is ProgressTridentsWizardPage)
			{
				return ProgressTridentsWizardPage.Scenario.InstallPage;
			}
			return ProgressTridentsWizardPage.Scenario.NormalPage;
		}

		private BackgroundWorker worker;

		private bool closeOnFinish;

		private bool allowTaskToRun = true;

		private bool runTaskIfAllowed = true;

		private bool needToRetryTask;

		private LocalizedString previousNextButtonText;

		private ProgressTridentsWizardPage.Scenario scenario;

		private bool neverRunTaskAgain;

		private bool runTaskOnSetActive;

		private class Scenario
		{
			public Scenario(bool runTaskOnSetActive, bool automaticallyMoveNext, bool disablePreviousPagesIfOK, bool disablePreviousPagesIfFail, bool disableTaskIfOK, bool disableTaskIfFail, bool disableCancelDuringTask, bool disableCancelAfterTask, bool canGoForwardIfFail, bool canRunTaskAgainIfOK, bool canRunTaskAgainIfFail, bool showCompletionText, bool closeWizardIfCancelled, bool canDoRetryIfFail, bool moveNextIfNoTask)
			{
				this.RunTaskOnSetActive = runTaskOnSetActive;
				this.AutomaticallyMoveNext = automaticallyMoveNext;
				this.DisablePreviousPagesIfOK = disablePreviousPagesIfOK;
				this.DisablePreviousPagesIfFail = disablePreviousPagesIfFail;
				this.DisableTaskIfOK = disableTaskIfOK;
				this.DisableTaskIfFail = disableTaskIfFail;
				this.DisableCancelDuringTask = disableCancelDuringTask;
				this.DisableCancelAfterTask = disableCancelAfterTask;
				this.CanGoForwardIfFail = canGoForwardIfFail;
				this.CanRunTaskAgainIfOK = canRunTaskAgainIfOK;
				this.CanRunTaskAgainIfFail = canRunTaskAgainIfFail;
				this.ShowCompletionText = showCompletionText;
				this.CloseWizardIfCancelled = closeWizardIfCancelled;
				this.CanDoRetryIfFail = canDoRetryIfFail;
				this.MoveNextIfNoTask = moveNextIfNoTask;
			}

			public static readonly ProgressTridentsWizardPage.Scenario LightPage = new ProgressTridentsWizardPage.Scenario(true, false, true, false, true, false, false, true, false, false, true, true, false, false, false);

			public static readonly ProgressTridentsWizardPage.Scenario NormalPage = new ProgressTridentsWizardPage.Scenario(false, true, true, false, true, false, false, false, true, false, true, false, false, false, false);

			public static readonly ProgressTridentsWizardPage.Scenario PreReqPage = new ProgressTridentsWizardPage.Scenario(true, false, false, false, false, false, false, false, false, true, true, false, true, true, true);

			public static readonly ProgressTridentsWizardPage.Scenario InstallPage = new ProgressTridentsWizardPage.Scenario(true, true, true, true, true, true, true, true, true, false, false, false, false, false, false);

			public readonly bool RunTaskOnSetActive;

			public readonly bool AutomaticallyMoveNext;

			public readonly bool DisablePreviousPagesIfOK;

			public readonly bool DisablePreviousPagesIfFail;

			public readonly bool DisableTaskIfOK;

			public readonly bool DisableTaskIfFail;

			public readonly bool DisableCancelDuringTask;

			public readonly bool DisableCancelAfterTask;

			public readonly bool CanGoForwardIfFail;

			public readonly bool CanRunTaskAgainIfOK;

			public readonly bool CanRunTaskAgainIfFail;

			public readonly bool ShowCompletionText;

			public readonly bool CloseWizardIfCancelled;

			public readonly bool CanDoRetryIfFail;

			public readonly bool MoveNextIfNoTask;
		}
	}
}
