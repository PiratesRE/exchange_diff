using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.Common;
using Microsoft.Exchange.Setup.CommonBase;
using Microsoft.Exchange.Setup.ExSetupUI;

namespace Microsoft.Exchange.Setup.GUI
{
	internal class SetupProgressPage : ProgressPageBase
	{
		public SetupProgressPage(RootDataHandler rootDataHandler)
		{
			this.rootDataHandler = rootDataHandler;
			this.TaskState = TaskState.NotStarted;
			this.InitializeComponent();
			base.PageTitle = Strings.SetupProgressPageTitle;
			this.setupProgressLabel.Text = string.Empty;
			this.customProgressBarWithTitle.Title = string.Empty;
			this.reportTextBox.Text = string.Empty;
			this.reportTextBox.Enabled = false;
			this.reportTextBox.Visible = false;
			this.currentStep = 1;
			base.WizardCancel += this.SetupProgressPage_WizardCancel;
			this.TaskComplete += this.SetupProgressPage_TaskComplete;
		}

		public event EventHandler TaskStateChanged;

		public event EventHandler TaskComplete;

		public event PropertyChangedEventHandler WorkUnitPropertyChanged;

		internal TaskState TaskState
		{
			get
			{
				return this.taskState;
			}
			set
			{
				if (this.taskState != value)
				{
					this.taskState = value;
					this.OnTaskStateChanged(EventArgs.Empty);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected virtual void OnTaskStateChanged(EventArgs e)
		{
			if (this.TaskStateChanged != null)
			{
				this.TaskStateChanged(this, e);
			}
		}

		protected virtual void OnTaskCompleted(EventArgs e)
		{
			if (this.TaskComplete != null)
			{
				this.TaskComplete(this, e);
			}
		}

		protected virtual void OnWorkUnitPropertyChanged(PropertyChangedEventArgs e)
		{
			if (this.WorkUnitPropertyChanged != null)
			{
				this.WorkUnitPropertyChanged(this, e);
			}
		}

		private void SetupProgressPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			this.setupProgressLabel.Enabled = false;
			this.setupProgressLabel.Visible = false;
			base.SetWizardButtons(0);
			base.SetVisibleWizardButtons(0);
			this.rootDataHandler.WorkUnits.RaiseListChangedEvents = false;
			this.rootDataHandler.UpdateWorkUnits();
			this.workUnits = this.rootDataHandler.WorkUnits;
			this.totalSteps = this.workUnits.Count.ToString();
			this.rootDataHandler.WorkUnits.RaiseListChangedEvents = true;
			foreach (WorkUnit workUnit in this.workUnits)
			{
				workUnit.Status = WorkUnitStatus.NotStarted;
				workUnit.PropertyChanged += this.WorkUnit_PropertyChanged;
			}
			this.TaskState = TaskState.InProgress;
			this.customProgressBarWithTitle.Title = this.workUnits[0].Text;
			this.customProgressBarWithTitle.Value = 0;
			base.EnableCancelButton(false);
			SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
			this.backgroundWorker.RunWorkerAsync();
			base.EnableCheckLoadedTimer(200);
		}

		private void WorkUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new PropertyChangedEventHandler(this.WorkUnit_PropertyChanged), new object[]
				{
					sender,
					e
				});
				return;
			}
			this.UpdateProgress();
		}

		private void UpdateProgress()
		{
			foreach (WorkUnit workUnit in this.workUnits)
			{
				StringBuilder stringBuilder = new StringBuilder(2048);
				switch (workUnit.Status)
				{
				case WorkUnitStatus.InProgress:
				{
					int num = this.workUnits.IndexOf(workUnit);
					if (num >= this.currentStep)
					{
						this.currentStep = num;
					}
					this.UpdateProgressBar(workUnit);
					break;
				}
				case WorkUnitStatus.Completed:
					if (!string.IsNullOrEmpty(workUnit.WarningsDescription))
					{
						stringBuilder.AppendLine(workUnit.WarningsDescription);
					}
					break;
				case WorkUnitStatus.Failed:
					if (!string.IsNullOrEmpty(workUnit.ErrorsDescription))
					{
						stringBuilder.AppendLine(workUnit.ErrorsDescription);
					}
					if (!string.IsNullOrEmpty(workUnit.WarningsDescription))
					{
						stringBuilder.AppendLine(workUnit.WarningsDescription);
					}
					break;
				}
				string value = stringBuilder.ToString().Trim();
				if (!string.IsNullOrEmpty(value))
				{
					this.reportTextBox.Enabled = true;
					this.reportTextBox.InitializeCustomScrollbar();
					this.reportTextBox.Visible = true;
					this.reportTextBox.Text = stringBuilder.ToString();
				}
			}
		}

		private void UpdateProgressBar(WorkUnit workUnit)
		{
			if (!this.customProgressBarWithTitle.Visible)
			{
				this.customProgressBarWithTitle.Visible = true;
				this.reportTextBox.Enabled = false;
				this.reportTextBox.Visible = false;
			}
			this.customProgressBarWithTitle.Title = Strings.CurrentStep(this.currentStep.ToString(), this.totalSteps, workUnit.Text);
			this.customProgressBarWithTitle.Value = workUnit.PercentComplete;
		}

		private void SetupProgressPage_TaskComplete(object sender, EventArgs e)
		{
			if (this.rootDataHandler.IsSucceeded)
			{
				this.customProgressBarWithTitle.Title = Strings.SetupCompleted;
			}
		}

		private void SetupProgressPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.setupProgressLabel.Name, true);
			if (array != null && array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void SetupProgressPage_WizardCancel(object sender, CancelEventArgs e)
		{
			if (this.backgroundWorker.IsBusy && this.rootDataHandler != null)
			{
				this.rootDataHandler.Cancel();
			}
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (this.rootDataHandler != null)
			{
				this.rootDataHandler.Save(null);
			}
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.TaskState = TaskState.Completed;
			this.OnTaskCompleted(e);
			if (!this.rootDataHandler.IsSucceeded)
			{
				this.RemoveSetupCompletedPage();
				base.SetWizardButtons(2);
				base.SetBtnNextText(Strings.btnExit);
				base.SetVisibleWizardButtons(2);
				return;
			}
			base.SetWizardButtons(2);
			if ((this.rootDataHandler.Mode == InstallationModes.Install || this.rootDataHandler.Mode == InstallationModes.BuildToBuildUpgrade) && !this.rootDataHandler.IsUmLanguagePackOperation && !this.rootDataHandler.IsLanguagePackOperation)
			{
				base.SetVisibleWizardButtons(2);
				base.PressButton(2);
				return;
			}
			base.SetBtnNextText(Strings.btnFinish);
			base.SetVisibleWizardButtons(2);
		}

		private void RemoveSetupCompletedPage()
		{
			this.setupCompletedPage = (SetupCompletedPage)base.FindPage("SetupCompletedPage");
			if (this.setupCompletedPage != null)
			{
				base.RemovePage(this.setupCompletedPage);
				this.setupCompletedPage = null;
			}
		}

		private void InitializeComponent()
		{
			this.setupProgressLabel = new Label();
			this.backgroundWorker = new BackgroundWorker();
			this.reportTextBox = new CustomRichTextBox();
			base.SuspendLayout();
			this.customProgressBarWithTitle.Location = new Point(0, 60);
			this.customProgressBarWithTitle.Size = new Size(721, 76);
			this.setupProgressLabel.AutoSize = true;
			this.setupProgressLabel.BackColor = Color.Transparent;
			this.setupProgressLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.setupProgressLabel.Location = new Point(0, 0);
			this.setupProgressLabel.MaximumSize = new Size(720, 0);
			this.setupProgressLabel.Name = "setupProgressLabel";
			this.setupProgressLabel.Size = new Size(191, 17);
			this.setupProgressLabel.TabIndex = 26;
			this.setupProgressLabel.Text = "[SetupProgressDescriptionText]";
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += this.BackgroundWorker_DoWork;
			this.backgroundWorker.RunWorkerCompleted += this.BackgroundWorker_RunWorkerCompleted;
			this.reportTextBox.BackColor = SystemColors.Window;
			this.reportTextBox.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.reportTextBox.Location = new Point(0, 150);
			this.reportTextBox.Margin = new Padding(0);
			this.reportTextBox.Name = "reportTextBox";
			this.reportTextBox.Size = new Size(721, 285);
			this.reportTextBox.TabIndex = 19;
			this.reportTextBox.Text = "[ReportText]";
			this.BackColor = Color.Transparent;
			base.Controls.Add(this.setupProgressLabel);
			base.Controls.Add(this.reportTextBox);
			base.Name = "SetupProgressPage";
			base.SetActive += this.SetupProgressPage_SetActive;
			base.CheckLoaded += this.SetupProgressPage_CheckLoaded;
			base.Controls.SetChildIndex(this.customProgressBarWithTitle, 0);
			base.Controls.SetChildIndex(this.reportTextBox, 0);
			base.Controls.SetChildIndex(this.setupProgressLabel, 0);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private Label setupProgressLabel;

		private BackgroundWorker backgroundWorker;

		private RootDataHandler rootDataHandler;

		private IList<WorkUnit> workUnits;

		private TaskState taskState;

		private CustomRichTextBox reportTextBox;

		private SetupCompletedPage setupCompletedPage;

		private string totalSteps;

		private int currentStep = 1;

		private IContainer components;
	}
}
