using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.Exchange.Sqm;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Designer(typeof(ScrollableControlDesigner))]
	public class ResultPane : AbstractResultPane
	{
		public ResultPane() : this(null, null)
		{
		}

		public ResultPane(IResultsLoaderConfiguration config) : this((config != null) ? config.BuildResultsLoaderProfile() : null, null)
		{
		}

		public ResultPane(DataTableLoader loader) : this((loader != null) ? loader.ResultsLoaderProfile : null, loader)
		{
		}

		public ResultPane(ObjectPickerProfileLoader profileLoader, string profileName) : this(profileLoader.GetProfile(profileName))
		{
		}

		public ResultPane(ResultsLoaderProfile profile) : this(profile, null)
		{
		}

		protected ResultPane(ResultsLoaderProfile profile, DataTableLoader loader)
		{
			base.SuspendLayout();
			this.warningCaption = new AutoHeightLabel();
			this.warningCaption.BackColor = SystemColors.Info;
			this.warningCaption.ForeColor = SystemColors.InfoText;
			this.warningCaption.Image = IconLibrary.ToSmallBitmap(Icons.Warning);
			this.warningCaption.Dock = DockStyle.Top;
			this.warningCaption.Name = "warningCaption";
			this.warningCaption.ImageAlign = ContentAlignment.MiddleLeft;
			this.warningCaption.TextAlign = ContentAlignment.MiddleLeft;
			this.warningCaption.Padding = new Padding(20, 2, 2, 2);
			this.warningCaption.Visible = false;
			base.Controls.Add(this.warningCaption);
			base.Name = "ResultPane";
			base.ResumeLayout(false);
			Command command = Command.CreateSeparator();
			command.Visible = false;
			this.dependentResultPanesCommandsSeparator = Command.CreateSeparator();
			this.dependentResultPanesCommandsSeparator.Visible = false;
			this.deleteSelectionCommandsSeparator = Command.CreateSeparator();
			this.deleteSelectionCommandsSeparator.Visible = false;
			this.showSelectionPropertiesCommandsSeparator = Command.CreateSeparator();
			this.showSelectionPropertiesCommandsSeparator.Visible = false;
			Command command2 = Command.CreateSeparator();
			command2.Visible = false;
			this.helpCommandSeparator = Command.CreateSeparator();
			this.helpCommandSeparator.Visible = false;
			this.warningCaption.MouseEnter += delegate(object param0, EventArgs param1)
			{
				this.warningCaption.ForeColor = SystemColors.HighlightText;
				this.warningCaption.BackColor = SystemColors.Highlight;
			};
			this.warningCaption.MouseLeave += delegate(object param0, EventArgs param1)
			{
				this.warningCaption.ForeColor = SystemColors.InfoText;
				this.warningCaption.BackColor = SystemColors.Info;
			};
			this.warningCaption.Click += this.warningCaption_Click;
			this.DependentResultPanes.ListChanging += this.DependentResultPanes_ListChanging;
			this.DependentResultPanes.ListChanged += this.DependentResultPanes_ListChanged;
			this.CustomSelectionCommands.CommandAdded += new CommandEventHandler(this.CustomSelectionCommands_CommandAdded);
			this.CustomSelectionCommands.CommandRemoved += new CommandEventHandler(this.CustomSelectionCommands_CommandRemoved);
			this.DependentResultPaneCommands.CommandAdded += new CommandEventHandler(this.DependentResultPaneCommands_CommandAdded);
			this.DependentResultPaneCommands.CommandRemoved += new CommandEventHandler(this.DependentResultPaneCommands_CommandRemoved);
			this.DeleteSelectionCommands.CommandAdded += new CommandEventHandler(this.DeleteSelectionCommands_CommandAdded);
			this.DeleteSelectionCommands.CommandRemoved += new CommandEventHandler(this.DeleteSelectionCommands_CommandRemoved);
			this.ShowSelectionPropertiesCommands.CommandAdded += new CommandEventHandler(this.ShowSelectionPropertiesCommands_CommandAdded);
			this.ShowSelectionPropertiesCommands.CommandRemoved += new CommandEventHandler(this.ShowSelectionPropertiesCommands_CommandRemoved);
			base.SelectionCommands.AddRange(new Command[]
			{
				this.dependentResultPanesCommandsSeparator,
				this.deleteSelectionCommandsSeparator,
				this.showSelectionPropertiesCommandsSeparator
			});
			base.ResultPaneCommands.CommandAdded += new CommandEventHandler(this.ResultPaneCommands_CommandAdded);
			base.ResultPaneCommands.CommandRemoved += new CommandEventHandler(this.ResultPaneCommands_CommandRemoved);
			base.ExportListCommands.CommandAdded += new CommandEventHandler(this.ExportListCommands_CommandAdded);
			base.ExportListCommands.CommandRemoved += new CommandEventHandler(this.ExportListCommands_CommandRemoved);
			this.viewCommand = new Command();
			this.viewCommand.Text = Strings.ViewCommands;
			this.viewCommand.Visible = false;
			this.viewCommand.Name = "resultPaneViewCommand";
			this.viewCommand.Icon = Icons.View;
			base.ViewModeCommands.CommandAdded += new CommandEventHandler(this.ViewModeCommands_CommandAdded);
			base.ViewModeCommands.CommandRemoved += new CommandEventHandler(this.ViewModeCommands_CommandRemoved);
			this.WhitespaceCommands.AddRange(new Command[]
			{
				command,
				this.viewCommand,
				command2,
				base.RefreshCommand
			});
			this.ResultsLoaderProfile = profile;
			this.RefreshableDataSource = loader;
			this.SetupCommandsProfile();
			this.SyncCommands(this.CommandsProfile.ResultPaneCommands, base.ResultPaneCommands);
			this.SyncCommands(this.CommandsProfile.CustomSelectionCommands, this.CustomSelectionCommands);
			this.SyncCommands(this.CommandsProfile.DeleteSelectionCommands, this.DeleteSelectionCommands);
			this.SubscribedRefreshCategories.Add(ResultPane.ConfigurationDomainControllerRefreshCategory);
		}

		private void SyncCommands(List<ResultsCommandProfile> profiles, CommandCollection commands)
		{
			foreach (ResultsCommandProfile resultsCommandProfile in profiles)
			{
				if (resultsCommandProfile.HasPermission())
				{
					resultsCommandProfile.ResultPane = this;
					commands.Add(resultsCommandProfile.Command);
					if (!resultsCommandProfile.IsSeparator)
					{
						base.Components.Add(resultsCommandProfile);
					}
				}
			}
		}

		public override string SelectionHelpTopic
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool HasPermission()
		{
			return this.ResultsLoaderProfile == null || this.ResultsLoaderProfile.HasPermission();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.ResultPaneCommands.CommandAdded -= new CommandEventHandler(this.ResultPaneCommands_CommandAdded);
				base.ResultPaneCommands.CommandRemoved -= new CommandEventHandler(this.ResultPaneCommands_CommandRemoved);
				base.ExportListCommands.CommandAdded -= new CommandEventHandler(this.ExportListCommands_CommandAdded);
				base.ExportListCommands.CommandRemoved -= new CommandEventHandler(this.ExportListCommands_CommandRemoved);
				base.ViewModeCommands.CommandAdded -= new CommandEventHandler(this.ViewModeCommands_CommandAdded);
				base.ViewModeCommands.CommandRemoved -= new CommandEventHandler(this.ViewModeCommands_CommandRemoved);
				this.WhitespaceCommands.Clear();
				this.viewCommand.Commands.Clear();
				this.CustomSelectionCommands.Clear();
				this.DependentResultPaneCommands.Clear();
				this.DeleteSelectionCommands.Clear();
				this.ShowSelectionPropertiesCommands.Clear();
				this.CustomSelectionCommands.CommandAdded -= new CommandEventHandler(this.CustomSelectionCommands_CommandAdded);
				this.CustomSelectionCommands.CommandRemoved -= new CommandEventHandler(this.CustomSelectionCommands_CommandRemoved);
				this.DependentResultPaneCommands.CommandAdded -= new CommandEventHandler(this.DependentResultPaneCommands_CommandAdded);
				this.DependentResultPaneCommands.CommandRemoved -= new CommandEventHandler(this.DependentResultPaneCommands_CommandRemoved);
				this.DeleteSelectionCommands.CommandAdded -= new CommandEventHandler(this.DeleteSelectionCommands_CommandAdded);
				this.DeleteSelectionCommands.CommandRemoved -= new CommandEventHandler(this.DeleteSelectionCommands_CommandRemoved);
				this.ShowSelectionPropertiesCommands.CommandAdded -= new CommandEventHandler(this.ShowSelectionPropertiesCommands_CommandAdded);
				this.ShowSelectionPropertiesCommands.CommandRemoved -= new CommandEventHandler(this.ShowSelectionPropertiesCommands_CommandRemoved);
				this.DependentResultPanes.ListChanging -= this.DependentResultPanes_ListChanging;
				this.DependentResultPanes.ListChanged -= this.DependentResultPanes_ListChanged;
			}
			base.Dispose(disposing);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			base.Controls.SetChildIndex(this.warningCaption, 1);
		}

		private bool IsSingleMoreResultsWarning
		{
			get
			{
				return this.workUnitsForWarnings.Count == 1 && this.workUnitsForWarnings[0].Warnings.Count == 1 && 0 == string.Compare(this.workUnitsForWarnings[0].Warnings[0], ResultPane.MoreItemsWarning);
			}
		}

		private void warningCaption_Click(object sender, EventArgs e)
		{
			this.HideWarningCaption();
			if (!this.IsSingleMoreResultsWarning)
			{
				UIService.ShowError("", Strings.Warnings, this.workUnitsForWarnings, base.ShellUI);
			}
		}

		protected void HideWarningCaption()
		{
			this.warningCaption.Visible = false;
		}

		public ChangeNotifyingCollection<AbstractResultPane> DependentResultPanes
		{
			get
			{
				return this.dependentResultPanes;
			}
		}

		private void DependentResultPanes_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.RemovingDependentResultPaneAt(e.NewIndex);
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				for (int i = this.DependentResultPanes.Count - 1; i >= 0; i--)
				{
					this.RemovingDependentResultPaneAt(i);
				}
			}
		}

		private void DependentResultPanes_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.InsertedDependentResultPaneAt(e.NewIndex);
			}
		}

		private void InsertedDependentResultPaneAt(int index)
		{
			AbstractResultPane abstractResultPane = this.DependentResultPanes[index];
			if (abstractResultPane == null)
			{
				throw new InvalidOperationException("Cannot add null to ResultPane.DependentResultPanes");
			}
			if (abstractResultPane.DependedResultPane != null)
			{
				throw new InvalidOperationException("the result pane has been added to DependentResultPanes of another result pane.");
			}
			abstractResultPane.DependedResultPane = this;
		}

		private void RemovingDependentResultPaneAt(int index)
		{
			AbstractResultPane abstractResultPane = this.DependentResultPanes[index];
			abstractResultPane.DependedResultPane = null;
			abstractResultPane.Enabled = true;
		}

		public bool IsDependedResultPane(AbstractResultPane resultPane)
		{
			bool result = false;
			if (resultPane != null)
			{
				for (ResultPane dependedResultPane = resultPane.DependedResultPane; dependedResultPane != null; dependedResultPane = dependedResultPane.DependedResultPane)
				{
					if (dependedResultPane == this)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public static bool IsDependedResultPane(AbstractResultPane firstResultPane, AbstractResultPane secondResultPane)
		{
			return firstResultPane is ResultPane && (firstResultPane as ResultPane).IsDependedResultPane(secondResultPane);
		}

		protected override void OnSharedSettingsChanging()
		{
			if (base.SharedSettings != null)
			{
				base.SharedSettings.RefreshResultPane -= new CustomDataRefreshEventHandler(this.SharedSettings_RefreshResultPane);
			}
			base.OnSharedSettingsChanging();
		}

		protected override void OnSharedSettingsChanged()
		{
			if (base.SharedSettings != null)
			{
				base.SharedSettings.RefreshResultPane += new CustomDataRefreshEventHandler(this.SharedSettings_RefreshResultPane);
			}
			base.OnSharedSettingsChanged();
		}

		public CommandCollection CustomSelectionCommands
		{
			get
			{
				return this.customSelectionCommands;
			}
		}

		private void CustomSelectionCommands_CommandAdded(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Insert(this.CustomSelectionCommands.IndexOf(e.Command), e.Command);
		}

		private void CustomSelectionCommands_CommandRemoved(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Remove(e.Command);
		}

		internal CommandCollection DependentResultPaneCommands
		{
			get
			{
				return this.dependentResultPaneCommands;
			}
		}

		private void DependentResultPaneCommands_CommandAdded(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Insert(base.SelectionCommands.IndexOf(this.dependentResultPanesCommandsSeparator) + 1 + this.DependentResultPaneCommands.IndexOf(e.Command), e.Command);
		}

		private void DependentResultPaneCommands_CommandRemoved(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Remove(e.Command);
		}

		public CommandCollection DeleteSelectionCommands
		{
			get
			{
				return this.deleteSelectionCommands;
			}
		}

		private void DeleteSelectionCommands_CommandAdded(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Insert(base.SelectionCommands.IndexOf(this.deleteSelectionCommandsSeparator) + 1 + this.DeleteSelectionCommands.IndexOf(e.Command), e.Command);
		}

		private void DeleteSelectionCommands_CommandRemoved(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Remove(e.Command);
		}

		public void InvokeCurrentDeleteSelectionCommand()
		{
			foreach (object obj in this.DeleteSelectionCommands)
			{
				Command command = (Command)obj;
				if (!command.IsSeparator && command.Visible)
				{
					command.Invoke();
					return;
				}
			}
			foreach (object obj2 in this.CustomSelectionCommands)
			{
				Command command2 = (Command)obj2;
				ResultsCommandProfile profile = this.CommandsProfile.GetProfile(command2);
				if (!command2.IsSeparator && command2.Visible && profile != null && profile.Setting != null && profile.Setting.IsRemoveCommand)
				{
					command2.Invoke();
					break;
				}
			}
		}

		public CommandCollection ShowSelectionPropertiesCommands
		{
			get
			{
				return this.showSelectionPropertiesCommands;
			}
		}

		private void ShowSelectionPropertiesCommands_CommandAdded(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Insert(base.SelectionCommands.IndexOf(this.showSelectionPropertiesCommandsSeparator) + 1 + this.ShowSelectionPropertiesCommands.IndexOf(e.Command), e.Command);
		}

		private void ShowSelectionPropertiesCommands_CommandRemoved(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Remove(e.Command);
		}

		public void InvokeCurrentShowSelectionPropertiesCommand()
		{
			foreach (object obj in this.ShowSelectionPropertiesCommands)
			{
				Command command = (Command)obj;
				if (!command.IsSeparator && command.Visible)
				{
					command.Invoke();
					return;
				}
			}
			foreach (object obj2 in this.CustomSelectionCommands)
			{
				Command command2 = (Command)obj2;
				ResultsCommandProfile profile = this.CommandsProfile.GetProfile(command2);
				if (!command2.IsSeparator && command2.Visible && profile != null && profile.Setting != null && profile.Setting.IsPropertiesCommand)
				{
					command2.Invoke();
					break;
				}
			}
		}

		public virtual IRefreshable GetSelectionRefreshObjects()
		{
			return null;
		}

		public ResultsLoaderProfile ResultsLoaderProfile
		{
			get
			{
				return this.resultsLoaderProfile;
			}
			private set
			{
				if (this.ResultsLoaderProfile != value)
				{
					this.resultsLoaderProfile = value;
					if (this.ResultsLoaderProfile != null)
					{
						this.Text = this.ResultsLoaderProfile.DisplayName;
					}
				}
			}
		}

		public ResultCommandsProfile CommandsProfile
		{
			get
			{
				if (this.ResultsLoaderProfile == null)
				{
					return this.commandsProfile;
				}
				return this.ResultsLoaderProfile.CommandsProfile;
			}
		}

		protected virtual void SetupCommandsProfile()
		{
		}

		public DataTableLoader DataTableLoader
		{
			get
			{
				return this.RefreshableDataSource as DataTableLoader;
			}
		}

		protected override void OnRefreshableDataSourceChanging(EventArgs e)
		{
			if (this.DataTableLoader != null)
			{
				this.DataTableLoader.RefreshCompleted -= this.RefreshableDataSource_RefreshCompleted;
			}
			base.OnRefreshableDataSourceChanging(e);
		}

		protected override void OnRefreshableDataSourceChanged(EventArgs e)
		{
			if (this.DataTableLoader != null)
			{
				this.DataTableLoader.RefreshCompleted += this.RefreshableDataSource_RefreshCompleted;
			}
			base.OnRefreshableDataSourceChanged(e);
		}

		private void RefreshableDataSource_RefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.workUnitsForWarnings = ((DataTableLoader)sender).WorkUnits.FindByErrorOrWarning();
			if (this.warningCaption.Visible = (this.workUnitsForWarnings.Count > 0))
			{
				this.warningCaption.Text = (this.IsSingleMoreResultsWarning ? this.workUnitsForWarnings[0].Warnings[0] : Strings.WarningNotification);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CommandCollection WhitespaceCommands
		{
			get
			{
				return this.whitespaceCommands;
			}
		}

		private void ViewModeCommands_CommandAdded(object sender, CommandEventArgs e)
		{
			this.viewCommand.Commands.Insert(base.ViewModeCommands.IndexOf(e.Command), e.Command);
			e.Command.VisibleChanged += this.ViewModeCommand_VisibleChanged;
			this.ViewModeCommand_VisibleChanged(e.Command, EventArgs.Empty);
		}

		private void ViewModeCommands_CommandRemoved(object sender, CommandEventArgs e)
		{
			this.viewCommand.Commands.Remove(e.Command);
			e.Command.VisibleChanged -= this.ViewModeCommand_VisibleChanged;
			this.ViewModeCommand_VisibleChanged(e.Command, EventArgs.Empty);
		}

		private void ViewModeCommand_VisibleChanged(object sender, EventArgs e)
		{
			this.viewCommand.Visible = base.ViewModeCommands.HasVisibleCommandsUpToIndex(base.ViewModeCommands.Count);
		}

		private void ResultPaneCommands_CommandAdded(object sender, CommandEventArgs e)
		{
			this.WhitespaceCommands.Insert(base.ResultPaneCommands.IndexOf(e.Command), e.Command);
		}

		private void ResultPaneCommands_CommandRemoved(object sender, CommandEventArgs e)
		{
			this.WhitespaceCommands.Remove(e.Command);
		}

		private void ExportListCommands_CommandAdded(object sender, CommandEventArgs e)
		{
			this.WhitespaceCommands.Insert(base.ResultPaneCommands.Count + base.ExportListCommands.IndexOf(e.Command), e.Command);
		}

		private void ExportListCommands_CommandRemoved(object sender, CommandEventArgs e)
		{
			this.WhitespaceCommands.Remove(e.Command);
		}

		protected override void UpdateContextMenuCommands()
		{
			base.UpdateContextMenuCommands();
			if (base.HasSelection)
			{
				base.ContextMenuCommands.AddRange(base.SelectionCommands);
			}
			else
			{
				base.ContextMenuCommands.AddRange(this.WhitespaceCommands);
			}
			base.ContextMenuCommands.Add(this.helpCommandSeparator);
			base.ContextMenuCommands.Add(base.HelpCommand);
		}

		protected override void OnSetActive(EventArgs e)
		{
			if (base.IsActivatedFirstly)
			{
				this.SyncCommands(this.CommandsProfile.ShowSelectionPropertiesCommands, this.ShowSelectionPropertiesCommands);
			}
			if (this.RefreshableDataSource == null && this.ResultsLoaderProfile != null)
			{
				this.RefreshableDataSource = new DataTableLoader(this.ResultsLoaderProfile);
			}
			bool flag = this.RefreshableDataSource != null && this.RefreshableDataSource.Refreshed;
			if (!flag || this.refreshWhenActivated)
			{
				this.DoFullRefreshWhenActivated();
				this.refreshWhenActivated = false;
			}
			else if (this.partialRefreshRequests.Count > 0)
			{
				this.ProcessPartialRefreshRequests();
			}
			base.OnSetActive(e);
			if (ManagementGuiSqmSession.Instance.Enabled)
			{
				ManagementGuiSqmSession.Instance.AddToStreamDataPoint(SqmDataID.DATAID_EMC_GUI_ACTION, new object[]
				{
					1U,
					base.Name
				});
			}
		}

		public ArrayList SubscribedRefreshCategories
		{
			get
			{
				return this.subscribedRefreshCategories;
			}
		}

		private void SharedSettings_RefreshResultPane(object sender, CustomDataRefreshEventArgs e)
		{
			if (!this.refreshWhenActivated)
			{
				bool flag = false;
				foreach (object obj in e.RefreshArguments)
				{
					if (obj is PartialRefreshRequest)
					{
						PartialRefreshRequest partialRefreshRequest = (PartialRefreshRequest)obj;
						if (!this.partialRefreshRequests.Contains(partialRefreshRequest) && this.SubscribedRefreshCategories.Contains(partialRefreshRequest.RefreshCategory))
						{
							this.partialRefreshRequests.Add(partialRefreshRequest);
						}
					}
					else if (this.SubscribedRefreshCategories.Contains(obj))
					{
						flag = true;
						break;
					}
				}
				if (flag || this.partialRefreshRequests.Count > 5)
				{
					this.SetRefreshWhenActivated();
					return;
				}
				if (this.partialRefreshRequests.Count > 0)
				{
					this.ProcessPartialRefreshRequests();
				}
			}
		}

		public void SetRefreshWhenActivated()
		{
			if (base.IsActive)
			{
				this.DoFullRefreshWhenActivated();
			}
			else if (this.RefreshableDataSource != null && this.RefreshableDataSource.Refreshed)
			{
				this.refreshWhenActivated = true;
			}
			this.partialRefreshRequests.Clear();
		}

		protected virtual void DoFullRefreshWhenActivated()
		{
			this.DelayInvokeRefreshCommand();
		}

		private void DelayInvokeRefreshCommand()
		{
			if (base.IsHandleCreated)
			{
				if (!this.refreshCommandInvokeScheduled)
				{
					this.refreshCommandInvokeScheduled = true;
					base.BeginInvoke(new MethodInvoker(delegate()
					{
						this.refreshCommandInvokeScheduled = false;
						base.RefreshCommand.Invoke();
					}));
					return;
				}
			}
			else
			{
				base.RefreshCommand.Invoke();
			}
		}

		private void ProcessPartialRefreshRequests()
		{
			if (this.partialRefreshRequests.Count > 0 && base.IsActive)
			{
				foreach (PartialRefreshRequest partialRefreshRequest in this.partialRefreshRequests)
				{
					partialRefreshRequest.DoRefresh(this.RefreshableDataSource, base.CreateProgress(base.RefreshCommand.Text));
				}
				this.partialRefreshRequests.Clear();
			}
		}

		public IRefreshable CreateRefreshableObject(params object[] refreshArguments)
		{
			if (refreshArguments == null || refreshArguments.Length == 0)
			{
				throw new ArgumentNullException("refreshArguments");
			}
			return new ResultPane.RefreshableObject(this, new CustomDataRefreshEventArgs(refreshArguments));
		}

		internal const int PartialRefreshRequestThreshold = 5;

		public static readonly object ConfigurationDomainControllerRefreshCategory = new object();

		private AutoHeightLabel warningCaption;

		private IList<WorkUnit> workUnitsForWarnings;

		private static string MoreItemsWarning = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Configuration.Common.LocStrings.Strings", typeof(WorkUnit).Assembly).GetString("WarningMoreResultsAvailable");

		private ChangeNotifyingCollection<AbstractResultPane> dependentResultPanes = new ChangeNotifyingCollection<AbstractResultPane>();

		private CommandCollection customSelectionCommands = new CommandCollection();

		private Command dependentResultPanesCommandsSeparator;

		private CommandCollection dependentResultPaneCommands = new CommandCollection();

		private Command deleteSelectionCommandsSeparator;

		private CommandCollection deleteSelectionCommands = new CommandCollection();

		private Command showSelectionPropertiesCommandsSeparator;

		private CommandCollection showSelectionPropertiesCommands = new CommandCollection();

		private ResultsLoaderProfile resultsLoaderProfile;

		private ResultCommandsProfile commandsProfile = new ResultCommandsProfile();

		private CommandCollection whitespaceCommands = new CommandCollection();

		private Command viewCommand;

		private Command helpCommandSeparator;

		private readonly ArrayList subscribedRefreshCategories = new ArrayList();

		private List<PartialRefreshRequest> partialRefreshRequests = new List<PartialRefreshRequest>();

		private bool refreshWhenActivated;

		private bool refreshCommandInvokeScheduled;

		private class RefreshableObject : IRefreshable
		{
			public RefreshableObject(ResultPane ownerResultPane, CustomDataRefreshEventArgs refreshEventArgs)
			{
				this.ownerResultPane = ownerResultPane;
				this.refreshEventArgs = refreshEventArgs;
			}

			void IRefreshable.Refresh(IProgress progress)
			{
				if (this.ownerResultPane != null && this.ownerResultPane.SharedSettings != null)
				{
					this.ownerResultPane.SharedSettings.RaiseRefreshResultPane(this.refreshEventArgs);
				}
				if (progress != null)
				{
					progress.ReportProgress(100, 100, "");
				}
			}

			private readonly CustomDataRefreshEventArgs refreshEventArgs;

			private readonly ResultPane ownerResultPane;
		}
	}
}
