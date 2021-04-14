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
	internal class PreCheckPage : ProgressPageBase
	{
		public PreCheckPage(RootDataHandler rootDataHandler)
		{
			this.rootDataHandler = rootDataHandler;
			this.TaskState = TaskState.NotStarted;
			this.InitializeComponent();
			base.PageTitle = Strings.PreCheckPageTitle;
			this.preChecksLabel.Text = Strings.PreCheckDescriptionText;
			this.customProgressBarWithTitle.Title = string.Empty;
			this.reportTextBox.Text = string.Empty;
			this.reportTextBox.Enabled = false;
			this.reportTextBox.Visible = false;
			base.WizardRetry += this.PreCheckPage_Retry;
			base.WizardCancel += this.PreCheckPage_WizardCancel;
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

		private void PreCheckPage_SetActive(object sender, CancelEventArgs e)
		{
			base.SetPageTitle(base.PageTitle);
			this.StartPrecheck();
			base.EnableCheckLoadedTimer(200);
		}

		private void StartPrecheck()
		{
			if (this.rootDataHandler.Mode == InstallationModes.Uninstall)
			{
				base.SetBtnNextText(Strings.Uninstall);
			}
			else
			{
				base.SetBtnNextText(Strings.Install);
			}
			base.SetWizardButtons(0);
			base.SetVisibleWizardButtons(3);
			this.reportTextBox.Text = string.Empty;
			this.reportTextBox.Enabled = false;
			this.reportTextBox.Visible = false;
			base.SetRetryFlag(false);
			this.reportTextBox.LinkClicked = new LinkClickedEventHandler(this.ReportTextBox_LinkClicked);
			if (this.rootDataHandler.IsUmLanguagePackOperation)
			{
				if (this.rootDataHandler.Mode == InstallationModes.Install)
				{
					AddUmLanguagePackModeDataHandler addUmLanguagePackModeDataHandler = this.rootDataHandler.ModeDatahandler as AddUmLanguagePackModeDataHandler;
					this.preCheckDataHandler = addUmLanguagePackModeDataHandler.PreCheckDataHandler;
				}
			}
			else
			{
				this.preCheckDataHandler = this.rootDataHandler.ModeDatahandler.PreCheckDataHandler;
			}
			this.preCheckDataHandler.WorkUnits.RaiseListChangedEvents = false;
			this.preCheckDataHandler.UpdateWorkUnits();
			this.workUnits = this.preCheckDataHandler.WorkUnits;
			this.preCheckDataHandler.WorkUnits.RaiseListChangedEvents = true;
			foreach (WorkUnit workUnit in this.workUnits)
			{
				workUnit.Status = WorkUnitStatus.NotStarted;
				workUnit.PropertyChanged += this.WorkUnit_PropertyChanged;
			}
			this.TaskState = TaskState.InProgress;
			this.customProgressBarWithTitle.Title = this.workUnits[0].Text;
			this.customProgressBarWithTitle.Value = 0;
			SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
			this.backgroundWorker.RunWorkerAsync();
		}

		private void ReportTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			SetupFormBase.ShowHelpFromUrl(e.LinkText);
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
			if (this.TaskState != TaskState.InProgress)
			{
				return;
			}
			foreach (WorkUnit workUnit in this.workUnits)
			{
				StringBuilder stringBuilder = new StringBuilder(2048);
				switch (workUnit.Status)
				{
				case WorkUnitStatus.InProgress:
					this.UpdateProgressBar(workUnit);
					break;
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
					else
					{
						stringBuilder.AppendLine(Strings.FatalError);
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
				if (workUnit.Status == WorkUnitStatus.Failed)
				{
					this.TaskState = TaskState.Completed;
					this.backgroundWorker.CancelAsync();
					break;
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
			this.customProgressBarWithTitle.Title = workUnit.Text;
			this.customProgressBarWithTitle.Value = workUnit.PercentComplete;
		}

		private void PreCheckPage_CheckLoaded(object sender, CancelEventArgs e)
		{
			Control[] array = base.Controls.Find(this.preChecksLabel.Name, true);
			if (array.Length > 0)
			{
				this.OnSetLoaded(new CancelEventArgs());
				SetupLogger.Log(Strings.PageLoaded(base.Name));
			}
		}

		private void PreCheckPage_WizardCancel(object sender, CancelEventArgs e)
		{
			this.StopPreCheckDataHandler();
			ExSetupUI.ExitApplication(ExitCode.Success);
		}

		private void StopPreCheckDataHandler()
		{
			if (this.backgroundWorker.IsBusy && this.preCheckDataHandler != null)
			{
				this.preCheckDataHandler.Cancel();
			}
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker backgroundWorker = sender as BackgroundWorker;
			if (backgroundWorker.CancellationPending)
			{
				e.Cancel = true;
				this.StopPreCheckDataHandler();
				return;
			}
			if (this.preCheckDataHandler != null)
			{
				this.preCheckDataHandler.Save(null);
			}
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.TaskState = TaskState.Completed;
			this.OnTaskCompleted(e);
			this.backgroundWorker.CancelAsync();
			if (this.preCheckDataHandler.IsSucceeded)
			{
				base.SetRetryFlag(false);
			}
			else
			{
				base.SetRetryFlag(true);
				base.SetBtnNextText(Strings.btnRetry);
				RestoreServer restoreServer = new RestoreServer();
				restoreServer.RestoreServerOnPrereqFailure();
			}
			base.SetWizardButtons(3);
			base.SetVisibleWizardButtons(2);
		}

		private void PreCheckPage_Retry(object sender, CancelEventArgs e)
		{
			base.Focus();
			this.StartPrecheck();
		}

		private void InitializeComponent()
		{
			this.preChecksLabel = new Label();
			this.backgroundWorker = new BackgroundWorker();
			this.reportTextBox = new CustomRichTextBox();
			base.SuspendLayout();
			this.customProgressBarWithTitle.Location = new Point(0, 60);
			this.customProgressBarWithTitle.Size = new Size(721, 76);
			this.preChecksLabel.AutoSize = true;
			this.preChecksLabel.BackColor = Color.Transparent;
			this.preChecksLabel.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.preChecksLabel.Location = new Point(0, 0);
			this.preChecksLabel.MaximumSize = new Size(740, 0);
			this.preChecksLabel.Name = "preChecksLabel";
			this.preChecksLabel.Size = new Size(159, 17);
			this.preChecksLabel.TabIndex = 26;
			this.preChecksLabel.Text = "[PreCheckDescriptionText]";
			this.backgroundWorker.WorkerSupportsCancellation = true;
			this.backgroundWorker.DoWork += this.BackgroundWorker_DoWork;
			this.backgroundWorker.RunWorkerCompleted += this.BackgroundWorker_RunWorkerCompleted;
			this.reportTextBox.BackColor = SystemColors.Window;
			this.reportTextBox.Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel, 0);
			this.reportTextBox.ForeColor = Color.FromArgb(51, 51, 51);
			this.reportTextBox.Location = new Point(0, 150);
			this.reportTextBox.Margin = new Padding(0);
			this.reportTextBox.Name = "reportTextBox";
			this.reportTextBox.Size = new Size(721, 285);
			this.reportTextBox.TabIndex = 19;
			this.reportTextBox.Text = "[ReportText]";
			this.BackColor = Color.Transparent;
			base.Controls.Add(this.preChecksLabel);
			base.Controls.Add(this.reportTextBox);
			base.Name = "PreCheckPage";
			base.SetActive += this.PreCheckPage_SetActive;
			base.CheckLoaded += this.PreCheckPage_CheckLoaded;
			base.Controls.SetChildIndex(this.customProgressBarWithTitle, 0);
			base.Controls.SetChildIndex(this.reportTextBox, 0);
			base.Controls.SetChildIndex(this.preChecksLabel, 0);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private Label preChecksLabel;

		private BackgroundWorker backgroundWorker;

		private PreCheckDataHandler preCheckDataHandler;

		private IList<WorkUnit> workUnits;

		private TaskState taskState;

		private CustomRichTextBox reportTextBox;

		private RootDataHandler rootDataHandler;

		private IContainer components;
	}
}
