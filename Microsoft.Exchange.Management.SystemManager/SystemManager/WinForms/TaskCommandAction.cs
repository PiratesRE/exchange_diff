using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class TaskCommandAction : CommandAction
	{
		[DefaultValue("")]
		public string CommandText
		{
			get
			{
				return this.commandText;
			}
			set
			{
				if (this.CommandText != value)
				{
					this.commandText = (value ?? string.Empty);
				}
			}
		}

		internal MonadParameterCollection Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = (value ?? new MonadParameterCollection());
			}
		}

		[DefaultValue(null)]
		public SingleSelectionMessageDelegate SingleSelectionConfirmation
		{
			get
			{
				return this.singleSelectionConfirmationDelegate;
			}
			set
			{
				this.singleSelectionConfirmationDelegate = value;
			}
		}

		[DefaultValue(null)]
		public MultipleSelectionMessageDelegate MultipleSelectionConfirmation
		{
			get
			{
				return this.multipleSelectionConfirmationDelegate;
			}
			set
			{
				this.multipleSelectionConfirmationDelegate = value;
			}
		}

		[DefaultValue(null)]
		public SingleSelectionMessageDelegate SingleSelectionError
		{
			get
			{
				return this.singleSelectionErrorDelegate;
			}
			set
			{
				this.singleSelectionErrorDelegate = value;
			}
		}

		[DefaultValue(null)]
		public MultipleSelectionMessageDelegate MultipleSelectionError
		{
			get
			{
				return this.multipleSelectionErrorDelegate;
			}
			set
			{
				this.multipleSelectionErrorDelegate = value;
			}
		}

		[DefaultValue(null)]
		public SingleSelectionMessageDelegate SingleSelectionWarning
		{
			get
			{
				return this.singleSelectionWarningDelegate;
			}
			set
			{
				this.singleSelectionWarningDelegate = value;
			}
		}

		[DefaultValue(null)]
		public MultipleSelectionMessageDelegate MultipleSelectionWarning
		{
			get
			{
				return this.multipleSelectionWarningDelegate;
			}
			set
			{
				this.multipleSelectionWarningDelegate = value;
			}
		}

		[DefaultValue(null)]
		public IRefreshable RefreshOnFinish
		{
			get
			{
				return this.refreshOnFinish;
			}
			set
			{
				this.refreshOnFinish = value;
			}
		}

		[DefaultValue(null)]
		public IRefreshable[] MultiRefreshOnFinish
		{
			get
			{
				return this.multiRefreshOnFinish;
			}
			set
			{
				this.multiRefreshOnFinish = value;
			}
		}

		private IUIService TestUIService { get; set; }

		public IProgress CreateProgress(string operationName)
		{
			IProgressProvider progressProvider = (IProgressProvider)this.GetService(typeof(IProgressProvider));
			if (progressProvider != null)
			{
				return progressProvider.CreateProgress(operationName);
			}
			return NullProgress.Value;
		}

		protected string CommandDisplayName
		{
			get
			{
				return ExchangeUserControl.RemoveAccelerator(base.Command.Text);
			}
		}

		protected virtual bool ConfirmOperation(WorkUnitCollectionEventArgs inputArgs)
		{
			IUIService iuiservice = (IUIService)this.GetService(typeof(IUIService));
			string text = null;
			if (inputArgs.WorkUnits.Count == 1 && this.SingleSelectionConfirmation != null)
			{
				text = this.SingleSelectionConfirmation(inputArgs.WorkUnits[0].Text);
			}
			if (inputArgs.WorkUnits.Count > 1 && this.MultipleSelectionConfirmation != null)
			{
				text = this.MultipleSelectionConfirmation(inputArgs.WorkUnits.Count);
			}
			return string.IsNullOrEmpty(text) || DialogResult.No != iuiservice.ShowMessage(text, this.CommandDisplayName, MessageBoxButtons.YesNo);
		}

		protected override void OnExecute()
		{
			string commandDisplayName = this.CommandDisplayName;
			WorkUnitCollectionEventArgs inputArgs = new WorkUnitCollectionEventArgs(new WorkUnitCollection());
			this.OnInputRequested(inputArgs);
			IUIService uiService = (IUIService)this.GetService(typeof(IUIService));
			if (uiService == null)
			{
				throw new InvalidOperationException("TaskCommand must be sited and needs to be able to find an IUIService.");
			}
			Control controlToRestoreFocus = uiService.GetDialogOwnerWindow() as Control;
			IRefreshable singleRefreshOnFinish = this.RefreshOnFinish;
			IRefreshable[] multiRefreshOnFinish = (this.MultiRefreshOnFinish == null) ? null : ((IRefreshable[])this.MultiRefreshOnFinish.Clone());
			if (this.ConfirmOperation(inputArgs))
			{
				WorkUnitCollection workUnits = inputArgs.WorkUnits;
				if (workUnits.Count == 0)
				{
					WorkUnit workUnit = new WorkUnit();
					workUnit.Text = commandDisplayName;
					workUnit.Target = null;
					workUnits.Add(workUnit);
				}
				IProgress progress = this.CreateProgress(new LocalizedString(commandDisplayName));
				MonadCommand command = new LoggableMonadCommand();
				command.CommandText = this.CommandText;
				foreach (object obj in this.Parameters)
				{
					MonadParameter value = (MonadParameter)obj;
					command.Parameters.Add(value);
				}
				command.ProgressReport += delegate(object sender, ProgressReportEventArgs progressReportEventArgs)
				{
					progress.ReportProgress(workUnits.ProgressValue, workUnits.MaxProgressValue, progressReportEventArgs.ProgressRecord.StatusDescription);
				};
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += delegate(object param0, DoWorkEventArgs param1)
				{
					MonadConnection connection = new MonadConnection("timeout=30", new WinFormsCommandInteractionHandler(this.TestUIService ?? uiService), ADServerSettingsSingleton.GetInstance().CreateRunspaceServerSettingsObject(), PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo());
					command.Connection = connection;
					using (new OpenConnection(connection))
					{
						command.Execute(workUnits.ToArray());
					}
				};
				worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
				{
					command.Connection.Close();
					if (runWorkerCompletedEventArgs.Error != null)
					{
						progress.ReportProgress(0, 0, "");
						uiService.ShowError(runWorkerCompletedEventArgs.Error);
					}
					else
					{
						int num = workUnits.HasFailures ? 0 : 100;
						progress.ReportProgress(num, num, "");
						List<WorkUnit> list = new List<WorkUnit>(workUnits.FindByErrorOrWarning());
						if (workUnits.Cancelled)
						{
							WorkUnit workUnit2 = list[list.Count - 1];
							for (int i = 0; i < workUnit2.Errors.Count; i++)
							{
								if (workUnit2.Errors[i].Exception is PipelineStoppedException)
								{
									workUnit2.Errors.Remove(workUnit2.Errors[i]);
									break;
								}
							}
							if (workUnit2.Errors.Count == 0)
							{
								list.Remove(workUnit2);
							}
						}
						if (list.Count > 0)
						{
							string errorMessage = null;
							string warningMessage = null;
							if (list.Count == 1)
							{
								if (this.SingleSelectionError != null)
								{
									errorMessage = this.SingleSelectionError(list[0].Text);
								}
								else
								{
									errorMessage = Strings.SingleSelectionError(commandDisplayName, list[0].Text);
								}
								if (this.SingleSelectionWarning != null)
								{
									warningMessage = this.SingleSelectionWarning(list[0].Text);
								}
								else
								{
									warningMessage = Strings.SingleSelectionWarning(commandDisplayName, list[0].Text);
								}
							}
							else if (list.Count > 1)
							{
								if (this.MultipleSelectionError != null)
								{
									errorMessage = this.MultipleSelectionError(list.Count);
								}
								else
								{
									errorMessage = Strings.MultipleSelectionError(commandDisplayName, list.Count);
								}
								if (this.MultipleSelectionWarning != null)
								{
									warningMessage = this.MultipleSelectionWarning(list.Count);
								}
								else
								{
									warningMessage = Strings.MultipleSelectionWarning(commandDisplayName, list.Count);
								}
							}
							UIService.ShowError(errorMessage, warningMessage, list, uiService);
						}
					}
					this.PerformRefreshOnFinish(workUnits, singleRefreshOnFinish, multiRefreshOnFinish);
					this.OnCompleted(inputArgs);
				};
				bool flag = workUnits.Count > 1;
				ProgressDialog pd = null;
				if (flag)
				{
					pd = new ProgressDialog();
					pd.OkEnabled = false;
					pd.Text = Strings.TaskProgressDialogTitle(commandDisplayName);
					pd.UseMarquee = true;
					pd.StatusText = workUnits.Description;
					pd.FormClosing += delegate(object sender, FormClosingEventArgs formClosingEventArgs)
					{
						if (worker.IsBusy)
						{
							if (pd.CancelEnabled)
							{
								pd.CancelEnabled = false;
								WinformsHelper.InvokeAsync(delegate
								{
									command.Cancel();
								}, pd);
							}
							formClosingEventArgs.Cancel = worker.IsBusy;
						}
					};
					pd.FormClosed += delegate(object param0, FormClosedEventArgs param1)
					{
						if (controlToRestoreFocus != null)
						{
							controlToRestoreFocus.Focus();
						}
					};
					worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
					{
						pd.UseMarquee = false;
						pd.Maximum = 100;
						pd.Value = 100;
						pd.Close();
					};
					command.ProgressReport += delegate(object sender, ProgressReportEventArgs progressReportEventArgs)
					{
						if ((progressReportEventArgs.ProgressRecord.RecordType == ProgressRecordType.Processing && progressReportEventArgs.ProgressRecord.PercentComplete > 0 && progressReportEventArgs.ProgressRecord.PercentComplete < 100) || workUnits[0].Status == WorkUnitStatus.Completed)
						{
							pd.UseMarquee = false;
						}
						pd.Maximum = workUnits.MaxProgressValue;
						pd.Value = workUnits.ProgressValue;
						pd.StatusText = workUnits.Description;
					};
					pd.ShowModeless(uiService.GetDialogOwnerWindow() as IServiceProvider);
					uiService = pd.ShellUI;
				}
				SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
				worker.RunWorkerAsync();
				base.OnExecute();
			}
		}

		private void PerformRefreshOnFinish(WorkUnitCollection workUnits, IRefreshable singleRefresh, IRefreshable[] multiRefresh)
		{
			if (workUnits.IsDataChanged)
			{
				if (singleRefresh != null)
				{
					singleRefresh.Refresh(this.CreateProgress(base.Command.Text));
				}
				if (multiRefresh != null)
				{
					MultiRefreshableSource multiRefreshableSource = new MultiRefreshableSource();
					for (int i = 0; i < workUnits.Count; i++)
					{
						if (workUnits[i].Status != WorkUnitStatus.NotStarted)
						{
							multiRefreshableSource.RefreshableSources.Add(multiRefresh[i]);
						}
					}
					multiRefreshableSource.Refresh(this.CreateProgress(base.Command.Text));
				}
			}
		}

		public event EventHandler<WorkUnitCollectionEventArgs> InputRequested;

		protected virtual void OnInputRequested(WorkUnitCollectionEventArgs e)
		{
			if (this.InputRequested != null)
			{
				this.InputRequested(this, e);
			}
		}

		public event EventHandler<WorkUnitCollectionEventArgs> Completed;

		protected virtual void OnCompleted(WorkUnitCollectionEventArgs e)
		{
			if (this.Completed != null)
			{
				this.Completed(this, e);
			}
		}

		public override bool HasPermission()
		{
			return EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope(this.CommandText, (from MonadParameter c in this.Parameters
			select c.ParameterName).ToArray<string>());
		}

		private SingleSelectionMessageDelegate singleSelectionConfirmationDelegate;

		private MultipleSelectionMessageDelegate multipleSelectionConfirmationDelegate;

		private SingleSelectionMessageDelegate singleSelectionErrorDelegate;

		private MultipleSelectionMessageDelegate multipleSelectionErrorDelegate;

		private SingleSelectionMessageDelegate singleSelectionWarningDelegate;

		private MultipleSelectionMessageDelegate multipleSelectionWarningDelegate;

		private string commandText = string.Empty;

		private MonadParameterCollection parameters = new MonadParameterCollection();

		private IRefreshable refreshOnFinish;

		private IRefreshable[] multiRefreshOnFinish;
	}
}
