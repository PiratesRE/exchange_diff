using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class CommandLoggingDialog : ExchangeForm
	{
		public static Command GetCommandLoggingCommand()
		{
			if (CommandLoggingDialog.viewCommandLoggingCommand == null)
			{
				CommandLoggingDialog.viewCommandLoggingCommand = new Command();
				CommandLoggingDialog.viewCommandLoggingCommand.Name = "viewCommandLoggingCommand";
				CommandLoggingDialog.viewCommandLoggingCommand.Text = Strings.ViewCommandLOgging;
				CommandLoggingDialog.viewCommandLoggingCommand.Execute += delegate(object param0, EventArgs param1)
				{
					CommandLoggingDialog.ShowCommandLoggingDialog();
				};
			}
			return CommandLoggingDialog.viewCommandLoggingCommand;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!e.Cancel)
			{
				lock (CommandLoggingDialog.mutex)
				{
					CommandLoggingDialog.instance = null;
				}
				this.privateSettings.SaveDataListViewSettings(this.resultListView);
				this.privateSettings.Save();
			}
		}

		public CommandLoggingDialog()
		{
			this.InitializeComponent();
			this.fileMenuItem.Text = Strings.ObjectPickerFile;
			this.closeToolStripMenuItem.Text = Strings.ObjectPickerClose;
			this.copyCommandsToolStripMenuItem.Text = Strings.CopyCommands;
			this.actionMenuItem.Text = Strings.CommandLoggingAction;
			this.helpCommandLoggingToolStripMenuItem.Text = Strings.ShowHelpCommand;
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			this.ContextMenuStrip = contextMenuStrip;
			this.actionMenuItem.DropDown = contextMenuStrip;
			this.clearLogToolStripMenuItem.Text = Strings.ClearLogText;
			this.exportListCommand.Text = Strings.ExportListDefaultCommandText;
			this.exportListCommand.Name = "exportListCommand";
			base.Icon = Icons.CommandLogging;
			this.startLoggingDate.Text = (string.IsNullOrEmpty(CommandLoggingDialog.StartDateTime) ? Strings.CommandLoggingStopped : Strings.StartLoggingDate(CommandLoggingDialog.StartDateTime));
			Command command = new Command();
			command.Text = Strings.ModifyMaximumRecordToLog;
			command.Name = "modifyMaximumRecordToLogCommand";
			command.Execute += this.commandLoggingDialog_modifyMaximumRecordToLog;
			this.resultListView.AvailableColumns.Add(CommandLoggingSession.startExecutionTime, Strings.StartExecutionTime, true, string.Empty, null, "G", null);
			this.resultListView.AvailableColumns.Add(CommandLoggingSession.endExecutionTime, Strings.EndExecutionTime, true, string.Empty, null, "G", null);
			this.resultListView.AvailableColumns.Add(CommandLoggingSession.executionStatus, Strings.ExecutionStatus, true, "");
			this.resultListView.AvailableColumns.Add(CommandLoggingSession.command, Strings.Command, true, "");
			this.resultListView.SelectionNameProperty = CommandLoggingSession.command;
			this.resultListView.DataSource = CommandLoggingSession.GetInstance().LoggingData;
			this.addRemoveColumnsToolStripMenuItem.Command = this.resultListView.ShowColumnPickerCommand;
			this.addRemoveColumnsToolStripMenuItem.ToolTipText = "";
			this.exportListToolStripMenuItem.Command = this.exportListCommand;
			this.exportListToolStripMenuItem.ToolTipText = "";
			this.modifyMaximumRecordToLogToolStripMenuItem.Command = command;
			this.modifyMaximumRecordToLogToolStripMenuItem.ToolTipText = "";
			this.resultListView.SelectionChanged += this.resultListView_SelectionChanged;
			if (CommandLoggingDialog.settingsProvider == null)
			{
				CommandLoggingDialog.settingsProvider = new ExchangeSettingsProvider();
				CommandLoggingDialog.settingsProvider.Initialize(null, null);
			}
			this.privateSettings.UpdateProviders(CommandLoggingDialog.settingsProvider);
			this.privateSettings.Reload();
			if (CommandLoggingDialog.GlobalSettings != null && !string.IsNullOrEmpty(CommandLoggingDialog.GlobalSettings.InstanceDisplayName))
			{
				this.Text = Strings.CommandLoggingDialogTitle(CommandLoggingDialog.GlobalSettings.InstanceDisplayName);
			}
			else
			{
				this.Text = Strings.CommandLoggingDialogTitle("");
			}
			this.PrepareMenuStripItems();
			contextMenuStrip.Items.AddRange(new ToolStripItem[]
			{
				this.switchCommandLoggingToolStripMenuItem,
				this.modifyMaximumRecordToLogToolStripMenuItem,
				this.clearLogToolStripMenuItem,
				new ToolStripSeparator(),
				this.addRemoveColumnsToolStripMenuItem,
				new ToolStripSeparator(),
				this.exportListToolStripMenuItem,
				this.separator,
				this.copyCommandsToolStripMenuItem,
				new ToolStripSeparator(),
				this.helpCommandLoggingToolStripMenuItem
			});
			this.PrepareActionMenuStrip();
			base.ResizeEnd += delegate(object param0, EventArgs param1)
			{
				this.SaveCommandLoggingSettings();
			};
			this.splitContainer.SplitterMoved += delegate(object param0, SplitterEventArgs param1)
			{
				this.SaveCommandLoggingSettings();
			};
			base.Load += delegate(object param0, EventArgs param1)
			{
				this.LoadCommandLoggingSettings();
			};
			this.helpCommandLoggingToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				this.OnHelpRequested(new HelpEventArgs(Point.Empty));
			};
		}

		private void PrepareMenuStripItems()
		{
			this.exportListCommand.Execute += delegate(object param0, EventArgs param1)
			{
				WinformsHelper.ShowExportDialog(this, this.resultListView, base.ShellUI);
			};
			this.closeToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				base.Close();
			};
			this.switchCommandLoggingToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				if (CommandLoggingSession.GetInstance().CommandLoggingEnabled)
				{
					CommandLoggingSession.GetInstance().CommandLoggingEnabled = false;
					if (CommandLoggingDialog.GlobalSettings != null)
					{
						CommandLoggingDialog.GlobalSettings.IsCommandLoggingEnabled = false;
					}
					CommandLoggingDialog.StartDateTime = string.Empty;
					this.startLoggingDate.Text = Strings.CommandLoggingStopped;
					this.switchCommandLoggingToolStripMenuItem.Text = Strings.StartCommandLogging;
					return;
				}
				CommandLoggingSession.GetInstance().CommandLoggingEnabled = true;
				if (CommandLoggingDialog.GlobalSettings != null)
				{
					CommandLoggingDialog.GlobalSettings.IsCommandLoggingEnabled = true;
				}
				CommandLoggingDialog.StartDateTime = ((DateTime)ExDateTime.Now).ToString();
				this.startLoggingDate.Text = Strings.StartLoggingDate(CommandLoggingDialog.StartDateTime);
				this.switchCommandLoggingToolStripMenuItem.Text = Strings.StopCommandLogging;
			};
			this.clearLogToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				if (base.ShellUI.ShowMessage(Strings.ConfirmClearText, UIService.DefaultCaption, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					CommandLoggingSession.GetInstance().Clear();
				}
			};
			this.copyCommandsToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in this.resultListView.SelectedObjects)
				{
					DataRowView row = (DataRowView)obj;
					stringBuilder.AppendLine(this.RetrieveValueFromRow(row, CommandLoggingSession.command));
				}
				Clipboard.SetText(stringBuilder.ToString());
			};
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled)
			{
				ExchangeHelpService.ShowHelpFromHelpTopicId(HelpId.CommandLoggingDialog.ToString());
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		private void commandLoggingDialog_modifyMaximumRecordToLog(object sender, EventArgs e)
		{
			using (PromptDialog promptDialog = new PromptDialog(true))
			{
				ExchangeSettings exchangeSettings = new ExchangeSettings(new Component());
				exchangeSettings.MaximumRecordCount = CommandLoggingSession.GetInstance().MaximumRecordCount;
				promptDialog.Buttons = MessageBoxButtons.OKCancel;
				promptDialog.Title = Strings.MaximumNumberToLogTitle;
				promptDialog.Message = Strings.MaximumRecordToLogIntroduction;
				promptDialog.ContentLabel = Strings.MaximumRecordToLogText;
				promptDialog.DataSource = exchangeSettings;
				promptDialog.ValueMember = "MaximumRecordCount";
				promptDialog.Parse += delegate(object obj, ConvertEventArgs args)
				{
					int num;
					if (!int.TryParse((string)args.Value, out num) || !CommandLoggingSession.IsValidMaximumRecordCount(num))
					{
						throw new LocalizedException(Strings.InvalidMaximumRecordNumber(CommandLoggingSession.MaximumRecordCountLimit));
					}
					args.Value = num;
				};
				if (base.ShellUI.ShowDialog(promptDialog) == DialogResult.OK)
				{
					CommandLoggingSession.GetInstance().MaximumRecordCount = exchangeSettings.MaximumRecordCount;
					if (CommandLoggingDialog.GlobalSettings != null)
					{
						CommandLoggingDialog.GlobalSettings.MaximumRecordCount = exchangeSettings.MaximumRecordCount;
					}
				}
			}
		}

		private string RetrieveValueFromRow(DataRowView row, string columnName)
		{
			try
			{
				return (string)row[columnName];
			}
			catch (RowNotInTableException)
			{
			}
			return string.Empty;
		}

		internal static string StartDateTime
		{
			get
			{
				return CommandLoggingDialog.startDateTime;
			}
			set
			{
				CommandLoggingDialog.startDateTime = value;
			}
		}

		private void resultListView_SelectionChanged(object sender, EventArgs e)
		{
			this.selectedCountLabel.Text = Strings.CommandsSelected(this.resultListView.SelectedIndices.Count);
			StringBuilder stringBuilder = new StringBuilder().AppendLine(Strings.CommandTitle);
			DataRowView dataRowView = this.resultListView.SelectedObject as DataRowView;
			if (dataRowView != null && this.resultListView.SelectedIndices.Count == 1)
			{
				stringBuilder.AppendLine(this.RetrieveValueFromRow(dataRowView, CommandLoggingSession.command));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(Strings.OutputMessageTitle);
				string value = this.RetrieveValueFromRow(dataRowView, CommandLoggingSession.warning);
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.AppendLine(value);
				}
				string value2 = this.RetrieveValueFromRow(dataRowView, CommandLoggingSession.error);
				if (!string.IsNullOrEmpty(value2))
				{
					stringBuilder.AppendLine(value2);
				}
			}
			else
			{
				stringBuilder.AppendLine().AppendLine(Strings.OutputMessageTitle);
			}
			this.outputTextBox.Text = stringBuilder.ToString();
			this.PrepareActionMenuStrip();
		}

		private void PrepareActionMenuStrip()
		{
			this.copyCommandsToolStripMenuItem.Visible = (this.resultListView.SelectedIndices.Count > 0);
			this.separator.Visible = (this.resultListView.SelectedIndices.Count > 0);
		}

		public static void ShowCommandLoggingDialog()
		{
			lock (CommandLoggingDialog.mutex)
			{
				if (CommandLoggingDialog.commandLoggingThread != null && CommandLoggingDialog.commandLoggingThread.IsAlive)
				{
					if (CommandLoggingDialog.instance != null)
					{
						CommandLoggingDialog.instance.ActivateForm();
					}
				}
				else
				{
					CommandLoggingDialog.commandLoggingThread = new Thread(new ThreadStart(CommandLoggingDialog.ShowModelessInternal));
					CommandLoggingDialog.commandLoggingThread.SetApartmentState(ApartmentState.STA);
					CommandLoggingDialog.commandLoggingThread.Start();
				}
			}
		}

		public static void CloseCommandLoggingDialg()
		{
			if (CommandLoggingDialog.instance != null)
			{
				CommandLoggingDialog.instance.CloseForm();
			}
		}

		private void CloseForm()
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new MethodInvoker(this.CloseForm));
				return;
			}
			base.Close();
		}

		internal static void LogStart(Guid guid, DateTime startTime, string commandText)
		{
			lock (CommandLoggingDialog.mutex)
			{
				if (CommandLoggingDialog.instance != null)
				{
					CommandLoggingDialog.instance.EnsureThreadSafeToLogStartAndDisplay(guid, startTime, commandText);
				}
				else
				{
					CommandLoggingSession.GetInstance().LogStart(guid, startTime, commandText);
				}
			}
		}

		private void EnsureThreadSafeToLogStartAndDisplay(Guid guid, DateTime startTime, string commandText)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.Invoke(new CommandLoggingDialog.LogStartDelegate(this.EnsureThreadSafeToLogStartAndDisplay), new object[]
					{
						guid,
						startTime,
						commandText
					});
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
			CommandLoggingSession.GetInstance().LogStart(guid, startTime, commandText);
		}

		internal static void LogEnd(Guid guid, DateTime endTime)
		{
			lock (CommandLoggingDialog.mutex)
			{
				if (CommandLoggingDialog.instance != null)
				{
					CommandLoggingDialog.instance.EnsureThreadSafeToLogEndAndDisplay(guid, endTime);
				}
				else
				{
					CommandLoggingSession.GetInstance().LogEnd(guid, endTime);
				}
			}
		}

		private void EnsureThreadSafeToLogEndAndDisplay(Guid guid, DateTime endTime)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.Invoke(new CommandLoggingDialog.LogEndDelegate(this.EnsureThreadSafeToLogEndAndDisplay), new object[]
					{
						guid,
						endTime
					});
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
			CommandLoggingSession.GetInstance().LogEnd(guid, endTime);
		}

		internal static void LogWarning(Guid guid, string warning)
		{
			lock (CommandLoggingDialog.mutex)
			{
				if (CommandLoggingDialog.instance != null)
				{
					CommandLoggingDialog.instance.EnsureThreadSafeToLogWarningAndDisplay(guid, warning);
				}
				else
				{
					CommandLoggingSession.GetInstance().LogWarning(guid, warning);
				}
			}
		}

		private void EnsureThreadSafeToLogWarningAndDisplay(Guid guid, string warning)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.Invoke(new CommandLoggingDialog.LogWarningDelegate(this.EnsureThreadSafeToLogWarningAndDisplay), new object[]
					{
						guid,
						warning
					});
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
			CommandLoggingSession.GetInstance().LogWarning(guid, warning);
		}

		internal static void LogError(Guid guid, string error)
		{
			lock (CommandLoggingDialog.mutex)
			{
				if (CommandLoggingDialog.instance != null)
				{
					CommandLoggingDialog.instance.EnsureThreadSafeToLogErrorAndDisplay(guid, error);
				}
				else
				{
					CommandLoggingSession.GetInstance().LogError(guid, error);
				}
			}
		}

		private void EnsureThreadSafeToLogErrorAndDisplay(Guid guid, string error)
		{
			if (base.InvokeRequired)
			{
				try
				{
					base.Invoke(new CommandLoggingDialog.LogErrorDelegate(this.EnsureThreadSafeToLogErrorAndDisplay), new object[]
					{
						guid,
						error
					});
					return;
				}
				catch (InvalidOperationException)
				{
					return;
				}
			}
			CommandLoggingSession.GetInstance().LogError(guid, error);
		}

		private static void ShowModelessInternal()
		{
			lock (CommandLoggingDialog.mutex)
			{
				if (CommandLoggingDialog.instance == null)
				{
					CommandLoggingDialog.instance = new CommandLoggingDialog();
				}
			}
			CommandLoggingDialog.instance.ActivateForm();
			Application.Run(CommandLoggingDialog.instance);
		}

		private void ActivateForm()
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new MethodInvoker(this.ActivateForm));
				return;
			}
			if (base.WindowState == FormWindowState.Minimized)
			{
				base.WindowState = FormWindowState.Normal;
			}
			base.Activate();
		}

		public static ExchangeSettings GlobalSettings
		{
			get
			{
				return CommandLoggingDialog.globalSettings;
			}
			set
			{
				CommandLoggingDialog.globalSettings = value;
			}
		}

		private void LoadCommandLoggingSettings()
		{
			try
			{
				base.SuspendLayout();
				this.splitContainer.SuspendLayout();
				this.switchCommandLoggingToolStripMenuItem.Text = (CommandLoggingSession.GetInstance().CommandLoggingEnabled ? Strings.StopCommandLogging : Strings.StartCommandLogging);
				base.Size = ((CommandLoggingDialog.GlobalSettings != null) ? CommandLoggingDialog.GlobalSettings.CommandLoggingDialogSize : CommandLoggingDialog.DefaultDialogSize);
				base.Location = ((CommandLoggingDialog.GlobalSettings != null) ? CommandLoggingDialog.GlobalSettings.CommandLoggingDialogLocation : CommandLoggingDialog.DefaultDialogLocation);
				this.SplitterDistanceScale = ((CommandLoggingDialog.GlobalSettings != null) ? CommandLoggingDialog.GlobalSettings.CommandLoggingDialogSplitterDistanceScale : CommandLoggingDialog.DefaultSplitterDistanceScale);
				this.privateSettings.LoadDataListViewSettings(this.resultListView);
			}
			finally
			{
				this.splitContainer.ResumeLayout(true);
				base.ResumeLayout(true);
			}
		}

		private void SaveCommandLoggingSettings()
		{
			if (base.WindowState == FormWindowState.Maximized)
			{
				return;
			}
			if (CommandLoggingDialog.GlobalSettings != null)
			{
				CommandLoggingDialog.GlobalSettings.CommandLoggingDialogSize = base.Size;
				CommandLoggingDialog.GlobalSettings.CommandLoggingDialogLocation = base.Location;
				CommandLoggingDialog.GlobalSettings.CommandLoggingDialogSplitterDistanceScale = this.SplitterDistanceScale;
				return;
			}
			CommandLoggingDialog.DefaultDialogSize = base.Size;
			CommandLoggingDialog.DefaultDialogLocation = base.Location;
			CommandLoggingDialog.DefaultSplitterDistanceScale = this.SplitterDistanceScale;
		}

		private float SplitterContainerSize
		{
			get
			{
				return (float)((this.splitContainer.Orientation == Orientation.Horizontal) ? (this.splitContainer.Panel1.Height + this.splitContainer.Panel2.Height) : (this.splitContainer.Panel1.Width + this.splitContainer.Panel2.Width));
			}
		}

		private float SplitterDistanceScale
		{
			get
			{
				return (float)this.splitContainer.SplitterDistance / this.SplitterContainerSize;
			}
			set
			{
				if (value != this.SplitterDistanceScale)
				{
					this.splitContainer.SplitterDistance = (int)(value * this.SplitterContainerSize);
				}
			}
		}

		private static Thread commandLoggingThread;

		private static object mutex = new object();

		private static ExchangeSettingsProvider settingsProvider;

		internal static CommandLoggingDialog instance;

		private Command exportListCommand = new Command();

		private static Command viewCommandLoggingCommand = null;

		private ToolStripSeparator separator = new ToolStripSeparator();

		private DataListViewSettings privateSettings = new DataListViewSettings(new Component());

		private static string startDateTime = string.Empty;

		private static ExchangeSettings globalSettings;

		internal static Size DefaultDialogSize = new Size(500, 400);

		internal static Point DefaultDialogLocation = WinformsHelper.GetCentralLocation(CommandLoggingDialog.DefaultDialogSize);

		internal static float DefaultSplitterDistanceScale = 0.7f;

		private delegate void LogStartDelegate(Guid guid, DateTime datetime, string command);

		private delegate void LogEndDelegate(Guid guid, DateTime datetime);

		private delegate void LogErrorDelegate(Guid guid, string error);

		private delegate void LogWarningDelegate(Guid guid, string warning);
	}
}
