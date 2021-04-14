using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.Services;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ContainerResultPane : AbstractResultPane, IResultPaneSelectionService, IServiceProvider
	{
		public ContainerResultPane()
		{
			this.RegisterCommandsEvent(base.ResultPaneCommands);
			this.RegisterCommandsEvent(base.SelectionCommands);
			this.ResultPanes.ListChanging += this.ResultPanes_ListChanging;
			this.ResultPanes.ListChanged += this.ResultPanes_ListChanged;
			this.ResultPanesActiveToContainer.ListChanging += this.ResultPanesActiveToContainer_ListChanging;
			this.ResultPanesActiveToContainer.ListChanged += this.ResultPanesActiveToContainer_ListChanged;
			ISelectionService serviceInstance = new Selection();
			ServiceContainer serviceContainer = new ServiceContainer(this);
			serviceContainer.AddService(typeof(ISelectionService), serviceInstance);
			this.components = new ServicedContainer(serviceContainer);
			base.Name = "ContainerResultPane";
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ResultPanesActiveToContainer.ListChanging -= this.ResultPanesActiveToContainer_ListChanging;
				this.ResultPanesActiveToContainer.ListChanged -= this.ResultPanesActiveToContainer_ListChanged;
				this.ResultPanes.ListChanging -= this.ResultPanes_ListChanging;
				this.ResultPanes.ListChanged -= this.ResultPanes_ListChanged;
				this.UnregisterCommandsEvent(base.SelectionCommands);
				this.UnregisterCommandsEvent(base.ResultPaneCommands);
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void RegisterCommandsEvent(CommandCollection commands)
		{
			commands.CommandAdded += new CommandEventHandler(this.Commands_CommandAdded);
			commands.CommandRemoved += new CommandEventHandler(this.Commands_CommandRemoved);
		}

		private void UnregisterCommandsEvent(CommandCollection commands)
		{
			commands.CommandAdded -= new CommandEventHandler(this.Commands_CommandAdded);
			commands.CommandRemoved -= new CommandEventHandler(this.Commands_CommandRemoved);
		}

		private void Commands_CommandAdded(object sender, CommandEventArgs e)
		{
			e.Command.Executing += this.Command_Executing;
		}

		private void Commands_CommandRemoved(object sender, CommandEventArgs e)
		{
			e.Command.Executing -= this.Command_Executing;
		}

		private void Command_Executing(object sender, CancelEventArgs e)
		{
			this.SelectResultPaneByCommand(sender as Command);
		}

		private void SelectResultPaneByCommand(Command command)
		{
			Predicate<AbstractResultPane> selectionCondition = delegate(AbstractResultPane resultPane)
			{
				bool result = false;
				if (resultPane.ResultPaneCommands.Contains(command) || resultPane.SelectionCommands.Contains(command))
				{
					result = true;
				}
				return result;
			};
			this.SelectResultPane(selectionCondition);
		}

		public ChangeNotifyingCollection<AbstractResultPane> ResultPanes
		{
			get
			{
				return this.resultPanes;
			}
		}

		private void ResultPanes_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.RemovingResultPaneAt(e.NewIndex);
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				for (int i = this.ResultPanes.Count - 1; i >= 0; i--)
				{
					this.RemovingResultPaneAt(i);
				}
			}
		}

		private void ResultPanes_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.InsertedResultPaneAt(e.NewIndex);
			}
		}

		private void InsertedResultPaneAt(int index)
		{
			AbstractResultPane abstractResultPane = this.ResultPanes[index];
			if (abstractResultPane == null)
			{
				throw new InvalidOperationException("Cannot insert null to ResultPanes.");
			}
			if (abstractResultPane.ContainerResultPane != null)
			{
				throw new InvalidOperationException("The result pane has been inserted into anonther ContanerResultPane as Contained result pane.");
			}
			if (abstractResultPane.IsActive)
			{
				throw new InvalidOperationException("The inserted result pane can not be Active.");
			}
			this.components.Add(abstractResultPane);
			abstractResultPane.ContainerResultPane = this;
			abstractResultPane.SharedSettings = base.SharedSettings;
			abstractResultPane.IsModifiedChanged += this.ResultPane_IsModifiedChanged;
			this.ResultPane_IsModifiedChanged(abstractResultPane, EventArgs.Empty);
			abstractResultPane.SetActive += this.ResultPane_SetActive;
			abstractResultPane.KillingActive += this.ResultPane_KillingActive;
			abstractResultPane.EnabledChanging += this.ResultPane_EnabledChanging;
			abstractResultPane.EnabledChanged += this.ResultPane_EnabledChanged;
			this.TryToBindEnabledResultPaneToContainer(abstractResultPane);
		}

		private void RemovingResultPaneAt(int index)
		{
			AbstractResultPane abstractResultPane = this.ResultPanes[index];
			if (abstractResultPane.IsActive)
			{
				abstractResultPane.OnKillActive();
			}
			else if (this.IsActiveToContainer(abstractResultPane))
			{
				this.ResultPanesActiveToContainer.Remove(abstractResultPane);
			}
			this.TryToUnbindEnabledResultPaneFromContainer(abstractResultPane);
			abstractResultPane.EnabledChanging -= this.ResultPane_EnabledChanging;
			abstractResultPane.EnabledChanged -= this.ResultPane_EnabledChanged;
			abstractResultPane.SetActive -= this.ResultPane_SetActive;
			abstractResultPane.KillingActive -= this.ResultPane_KillingActive;
			abstractResultPane.IsModifiedChanged -= this.ResultPane_IsModifiedChanged;
			this.ResultPane_IsModifiedChanged(abstractResultPane, EventArgs.Empty);
			this.components.Remove(abstractResultPane);
			abstractResultPane.ContainerResultPane = null;
		}

		private void ResultPane_IsModifiedChanged(object sender, EventArgs e)
		{
			bool flag = false;
			foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
			{
				flag |= abstractResultPane.IsModified;
			}
			base.IsModified = (base.IsModified || flag);
		}

		private void ResultPane_SetActive(object sender, EventArgs e)
		{
			if (!base.IsActive)
			{
				throw new InvalidOperationException("container result pane must be activated at the first");
			}
			AbstractResultPane abstractResultPane = sender as AbstractResultPane;
			if (!this.isInSetActive && !this.IsActiveToContainer(abstractResultPane))
			{
				this.ResultPanesActiveToContainer.Add(abstractResultPane);
			}
		}

		private void ResultPane_KillingActive(object sender, EventArgs e)
		{
			if (!base.IsActive)
			{
				throw new InvalidOperationException("container result pane must be de-activated at the last");
			}
			AbstractResultPane abstractResultPane = sender as AbstractResultPane;
			if (!this.isInKillingActive && this.IsActiveToContainer(abstractResultPane))
			{
				this.ResultPanesActiveToContainer.Remove(abstractResultPane);
			}
		}

		private void ResultPane_EnabledChanging(object sender, EventArgs e)
		{
			if (this.ResultPanes.Count((AbstractResultPane childResultPane) => childResultPane.Enabled) == 0)
			{
				base.Enabled = true;
			}
			AbstractResultPane abstractResultPane = sender as AbstractResultPane;
			if (abstractResultPane.Enabled && this.SelectedResultPane == abstractResultPane)
			{
				this.SelectedResultPane = (abstractResultPane.DependedResultPane ?? abstractResultPane);
			}
			this.TryToUnbindEnabledResultPaneFromContainer(abstractResultPane);
		}

		private void ResultPane_EnabledChanged(object sender, EventArgs e)
		{
			AbstractResultPane resultPane = sender as AbstractResultPane;
			this.TryToBindEnabledResultPaneToContainer(resultPane);
			if (this.ResultPanes.Count((AbstractResultPane childResultPane) => childResultPane.Enabled) == 0)
			{
				base.Enabled = false;
			}
		}

		private bool TryToBindEnabledResultPaneToContainer(AbstractResultPane resultPane)
		{
			return resultPane.Enabled && (this.TryToBindToResultPaneCommandsOfContainer(resultPane) | this.TryToBindToDependedResultPaneInContainer(resultPane) | this.TryToBindDependentResultPanesInContainerTo(resultPane) | this.TryToBindToStatusOfContainer(resultPane) | this.TryToBindToSelectionCommandsOfContainer(resultPane) | this.TryToBindToSelectionOfContainer(resultPane));
		}

		private bool TryToUnbindEnabledResultPaneFromContainer(AbstractResultPane resultPane)
		{
			return resultPane.Enabled && (this.TryToUnbindFromStatusOfContainer(resultPane) | this.TryToUnbindFromSelectionCommandsOfContainer(resultPane) | this.TryToUnbindFromSelectionOfContainer(resultPane) | this.TryToUnbindDependentResultPanesInContainerFrom(resultPane) | this.TryToUnbindFromDependedResultPaneInContainer(resultPane) | this.TryToUnbindFromResultPaneCommandsOfContainer(resultPane));
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AbstractResultPane SelectedResultPane
		{
			get
			{
				return this.selectedResultPane;
			}
			set
			{
				if (this.SelectedResultPane != value)
				{
					if (value != null)
					{
						if (this.ResultPanes.IndexOf(value) == -1)
						{
							throw new ArgumentOutOfRangeException();
						}
						if (base.IsActive && !value.IsActive)
						{
							value.OnSetActive();
						}
						else if (!this.IsActiveToContainer(value))
						{
							this.ResultPanesActiveToContainer.Add(value);
						}
					}
					if (this.SelectedResultPane != null)
					{
						this.TryToUnbindActiveResultPaneFromContainer(this.SelectedResultPane);
					}
					this.selectedResultPane = value;
					if (this.SelectedResultPane != null)
					{
						this.TryToBindActiveResultPaneToContainer(this.SelectedResultPane);
					}
					this.OnSelectedResultPaneChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedResultPaneChanged
		{
			add
			{
				base.Events.AddHandler(ContainerResultPane.EventSelectedResultPaneChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ContainerResultPane.EventSelectedResultPaneChanged, value);
			}
		}

		protected virtual void OnSelectedResultPaneChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ContainerResultPane.EventSelectedResultPaneChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public ResultPane SelectedAtomResultPane
		{
			get
			{
				ContainerResultPane containerResultPane = this.SelectedResultPane as ContainerResultPane;
				if (containerResultPane == null)
				{
					return this.SelectedResultPane as ResultPane;
				}
				return containerResultPane.SelectedAtomResultPane;
			}
		}

		public ChangeNotifyingCollection<AbstractResultPane> ResultPanesActiveToContainer
		{
			get
			{
				return this.resultPanesActiveToContainer;
			}
		}

		public bool IsActiveToContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanesActiveToContainer.Contains(resultPane);
		}

		private void ResultPanesActiveToContainer_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.RemovingActiveResultPaneAt(e.NewIndex);
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				for (int i = this.ResultPanesActiveToContainer.Count - 1; i >= 0; i--)
				{
					this.RemovingActiveResultPaneAt(i);
				}
			}
		}

		private void ResultPanesActiveToContainer_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.InsertedActiveResultPaneAt(e.NewIndex);
			}
		}

		private void InsertedActiveResultPaneAt(int index)
		{
			AbstractResultPane abstractResultPane = this.ResultPanesActiveToContainer[index];
			if (!this.ResultPanes.Contains(abstractResultPane))
			{
				throw new InvalidOperationException("The result pane inserted into ResultPanesActiveToContainer must be contained in this.ResultPanes.");
			}
			if (base.IsActive && !abstractResultPane.IsActive)
			{
				throw new InvalidOperationException("when current ContainerResultPane is active, resultPane must be active before inserting it into ResultPanesActiveToContainer");
			}
			this.TryToBindActiveResultPaneToContainer(abstractResultPane);
		}

		protected void RemovingActiveResultPaneAt(int index)
		{
			AbstractResultPane abstractResultPane = this.ResultPanesActiveToContainer[index];
			if (base.IsActive && !abstractResultPane.IsActive)
			{
				throw new InvalidOperationException("when current ContainerResultPane is active, resultPane must be active before removing it from ResultPanesActiveToContainer");
			}
			if (this.SelectedResultPane == abstractResultPane)
			{
				this.SelectedResultPane = null;
			}
			this.TryToUnbindActiveResultPaneFromContainer(abstractResultPane);
		}

		private bool TryToBindActiveResultPaneToContainer(AbstractResultPane resultPane)
		{
			return this.IsActiveToContainer(resultPane) && (this.TryToBindToStatusOfContainer(resultPane) | this.TryToBindToExportListCommandsOfContainer(resultPane) | this.TryToBindToViewModeCommandsOfContainer(resultPane) | this.TryToBindToSelectionCommandsOfContainer(resultPane) | this.TryToBindToRefreshableDataSourceOfContainer(resultPane) | this.TryToBindToSelectionOfContainer(resultPane));
		}

		private bool TryToUnbindActiveResultPaneFromContainer(AbstractResultPane resultPane)
		{
			return this.IsActiveToContainer(resultPane) && (this.TryToUnbindFromSelectionOfContainer(resultPane) | this.TryToUnbindFromRefreshableDataSourceOfContainer(resultPane) | this.TryToUnbindFromSelectionCommandsOfContainer(resultPane) | this.TryToUnbindFromViewModeCommandsOfContainer(resultPane) | this.TryToUnbindFromExportListCommandsOfContainer(resultPane) | this.TryToUnbindFromStatusOfContainer(resultPane));
		}

		private bool HasImpactOnDependedResultPaneInContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane.Enabled && resultPane.DependedResultPane != null && this.ResultPanes.Contains(resultPane.DependedResultPane);
		}

		private bool TryToBindToDependedResultPaneInContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnDependedResultPaneInContainer(resultPane))
			{
				this.BindResultPaneCommandsToDependedResultPane(resultPane);
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromDependedResultPaneInContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnDependedResultPaneInContainer(resultPane))
			{
				this.UnbindResultPaneCommandsFromDependedResultPane(resultPane);
				return true;
			}
			return false;
		}

		private bool HasImpactFromDependentResultPanes(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane.Enabled && resultPane is ResultPane;
		}

		private bool TryToBindDependentResultPanesInContainerTo(AbstractResultPane resultPane)
		{
			if (this.HasImpactFromDependentResultPanes(resultPane))
			{
				ResultPane resultPane2 = resultPane as ResultPane;
				resultPane2.DependentResultPaneCommands.CommandAdded += new CommandEventHandler(this.DependentResultPaneCommandsOfResultPane_CommandAdded);
				resultPane2.DependentResultPaneCommands.CommandRemoved += new CommandEventHandler(this.DependentResultPaneCommandsOfResultPane_CommandRemoved);
				List<AbstractResultPane> directEnabledDependentResultPanesInContainer = this.GetDirectEnabledDependentResultPanesInContainer(resultPane2);
				foreach (AbstractResultPane resultPane3 in directEnabledDependentResultPanesInContainer)
				{
					this.BindResultPaneCommandsToDependedResultPane(resultPane3);
				}
				return true;
			}
			return false;
		}

		private bool TryToUnbindDependentResultPanesInContainerFrom(AbstractResultPane resultPane)
		{
			if (this.HasImpactFromDependentResultPanes(resultPane))
			{
				ResultPane resultPane2 = resultPane as ResultPane;
				List<AbstractResultPane> directEnabledDependentResultPanesInContainer = this.GetDirectEnabledDependentResultPanesInContainer(resultPane2);
				foreach (AbstractResultPane resultPane3 in directEnabledDependentResultPanesInContainer)
				{
					this.UnbindResultPaneCommandsFromDependedResultPane(resultPane3);
				}
				resultPane2.DependentResultPaneCommands.CommandAdded -= new CommandEventHandler(this.DependentResultPaneCommandsOfResultPane_CommandAdded);
				resultPane2.DependentResultPaneCommands.CommandRemoved -= new CommandEventHandler(this.DependentResultPaneCommandsOfResultPane_CommandRemoved);
				return true;
			}
			return false;
		}

		private void BindResultPaneCommandsToDependedResultPane(AbstractResultPane resultPane)
		{
			ContainerResultPane.BindResultPaneCommandsToCommands(resultPane.DependedResultPane.DependentResultPaneCommands, resultPane.DependedResultPane.DependentResultPanes, resultPane, new CommandEventHandler(this.ResultPaneCommandsOfDependentResultPane_CommandAdded), new CommandEventHandler(this.ResultPaneCommandsOfDependentResultPane_CommandRemoved));
		}

		private void UnbindResultPaneCommandsFromDependedResultPane(AbstractResultPane resultPane)
		{
			ContainerResultPane.UnbindResultPaneCommandsFromCommands(resultPane.DependedResultPane.DependentResultPaneCommands, resultPane.DependedResultPane.DependentResultPanes, resultPane, new CommandEventHandler(this.ResultPaneCommandsOfDependentResultPane_CommandAdded), new CommandEventHandler(this.ResultPaneCommandsOfDependentResultPane_CommandRemoved));
		}

		private void ResultPaneCommandsOfDependentResultPane_CommandAdded(object sender, CommandEventArgs e)
		{
			CommandCollection resultPaneCommands = sender as CommandCollection;
			AbstractResultPane abstractResultPane = this.FindResultPaneWithResultPaneCommandsInContainer(resultPaneCommands);
			ContainerResultPane.AddResultPaneCommandToCommands(abstractResultPane.DependedResultPane.DependentResultPaneCommands, abstractResultPane.DependedResultPane.DependentResultPanes, abstractResultPane, e.Command);
		}

		private void ResultPaneCommandsOfDependentResultPane_CommandRemoved(object sender, CommandEventArgs e)
		{
			CommandCollection resultPaneCommands = sender as CommandCollection;
			AbstractResultPane abstractResultPane = this.FindResultPaneWithResultPaneCommandsInContainer(resultPaneCommands);
			ContainerResultPane.RemoveResultPaneCommandFromCommands(abstractResultPane.DependedResultPane.DependentResultPaneCommands, abstractResultPane.DependedResultPane.DependentResultPanes, abstractResultPane, e.Command);
		}

		private void DependentResultPaneCommandsOfResultPane_CommandAdded(object sender, CommandEventArgs e)
		{
			e.Command.Executing += this.DependentResultPaneCommand_Executing;
		}

		private void DependentResultPaneCommandsOfResultPane_CommandRemoved(object sender, CommandEventArgs e)
		{
			e.Command.Executing -= this.DependentResultPaneCommand_Executing;
		}

		private void DependentResultPaneCommand_Executing(object sender, CancelEventArgs e)
		{
			this.SelectResultPaneByResultPaneCommand(sender as Command);
		}

		private void SelectResultPaneByResultPaneCommand(Command command)
		{
			Predicate<AbstractResultPane> selectionCondition = delegate(AbstractResultPane resultPane)
			{
				bool result = false;
				if (resultPane.ResultPaneCommands.Contains(command))
				{
					result = true;
				}
				return result;
			};
			this.SelectResultPane(selectionCondition);
		}

		protected override void OnSharedSettingsChanged()
		{
			foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
			{
				abstractResultPane.SharedSettings = base.SharedSettings;
			}
			base.OnSharedSettingsChanged();
		}

		public override void LoadComponentSettings()
		{
			base.LoadComponentSettings();
			foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
			{
				abstractResultPane.SettingsKey = abstractResultPane.Name;
				abstractResultPane.LoadComponentSettings();
			}
		}

		public override void ResetComponentSettings()
		{
			base.ResetComponentSettings();
			foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
			{
				abstractResultPane.ResetComponentSettings();
			}
		}

		public override void SaveComponentSettings()
		{
			foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
			{
				abstractResultPane.SaveComponentSettings();
			}
			base.SaveComponentSettings();
		}

		private bool HasImpactOnStatusOfContainer(AbstractResultPane resultPane)
		{
			if (this.ResultPanes.Contains(resultPane) && resultPane.Enabled && this.IsActiveToContainer(resultPane) && this.SelectedResultPane != null)
			{
				if (resultPane == this.SelectedResultPane)
				{
					return true;
				}
				if (ResultPane.IsDependedResultPane(resultPane, this.SelectedResultPane))
				{
					return this.GetTopEnabledResultPaneInContainer(resultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer)) == resultPane;
				}
			}
			return false;
		}

		private bool TryToBindToStatusOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnStatusOfContainer(resultPane))
			{
				AbstractResultPane abstractResultPane;
				if (resultPane != this.SelectedResultPane)
				{
					abstractResultPane = this.GetTopEnabledResultPaneInContainer(this.SelectedResultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer), (AbstractResultPane dependedResultPane) => dependedResultPane == resultPane);
				}
				else
				{
					abstractResultPane = null;
				}
				AbstractResultPane abstractResultPane2 = abstractResultPane;
				AbstractResultPane topResultPane = (resultPane == this.SelectedResultPane) ? this.GetTopEnabledResultPaneInContainer(this.SelectedResultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer)) : resultPane;
				if (abstractResultPane2 != null)
				{
					this.UnbindTopResultPaneFromStatusOfContainer(abstractResultPane2);
				}
				this.BindTopResultPaneToStatusOfContainer(topResultPane);
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromStatusOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnStatusOfContainer(resultPane))
			{
				AbstractResultPane topResultPane = (resultPane == this.SelectedResultPane) ? this.GetTopEnabledResultPaneInContainer(this.SelectedResultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer)) : resultPane;
				AbstractResultPane abstractResultPane;
				if (resultPane != this.SelectedResultPane)
				{
					abstractResultPane = this.GetTopEnabledResultPaneInContainer(this.SelectedResultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer), (AbstractResultPane dependedResultPane) => dependedResultPane == resultPane);
				}
				else
				{
					abstractResultPane = null;
				}
				AbstractResultPane abstractResultPane2 = abstractResultPane;
				this.UnbindTopResultPaneFromStatusOfContainer(topResultPane);
				if (abstractResultPane2 != null)
				{
					this.BindTopResultPaneToStatusOfContainer(abstractResultPane2);
				}
				return true;
			}
			return false;
		}

		private void BindTopResultPaneToStatusOfContainer(AbstractResultPane topResultPane)
		{
			topResultPane.StatusChanged += this.TopResultPane_StatusChanged;
			this.TopResultPane_StatusChanged(topResultPane, EventArgs.Empty);
		}

		private void UnbindTopResultPaneFromStatusOfContainer(AbstractResultPane topResultPane)
		{
			topResultPane.StatusChanged -= this.TopResultPane_StatusChanged;
			base.Status = string.Empty;
		}

		private void TopResultPane_StatusChanged(object sender, EventArgs e)
		{
			AbstractResultPane abstractResultPane = sender as AbstractResultPane;
			base.Status = abstractResultPane.Status;
		}

		private bool HasImpactOnResultPaneCommandsOfContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane.Enabled && this.GetTopEnabledResultPaneInContainer(resultPane) == resultPane;
		}

		private bool TryToBindToResultPaneCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnResultPaneCommandsOfContainer(resultPane))
			{
				if (resultPane is ResultPane)
				{
					List<AbstractResultPane> enabledDependentResultPanesInContainer = this.GetEnabledDependentResultPanesInContainer(resultPane as ResultPane, (AbstractResultPane dependentResultPane) => this.GetTopEnabledResultPaneInContainer(dependentResultPane) == resultPane);
					foreach (AbstractResultPane resultPane2 in enabledDependentResultPanesInContainer)
					{
						this.UnbindResultPaneCommandsFromResultPaneCommandsOfContainer(resultPane2);
					}
				}
				this.BindResultPaneCommandsToResultPaneCommandsOfContainer(resultPane);
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromResultPaneCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnResultPaneCommandsOfContainer(resultPane))
			{
				this.UnbindResultPaneCommandsFromResultPaneCommandsOfContainer(resultPane);
				if (resultPane is ResultPane)
				{
					List<AbstractResultPane> enabledDependentResultPanesInContainer = this.GetEnabledDependentResultPanesInContainer(resultPane as ResultPane, (AbstractResultPane dependentResultPane) => this.GetTopEnabledResultPaneInContainer(dependentResultPane) == resultPane);
					foreach (AbstractResultPane resultPane2 in enabledDependentResultPanesInContainer)
					{
						this.BindResultPaneCommandsToResultPaneCommandsOfContainer(resultPane2);
					}
				}
				return true;
			}
			return false;
		}

		private void BindResultPaneCommandsToResultPaneCommandsOfContainer(AbstractResultPane resultPane)
		{
			ContainerResultPane.BindResultPaneCommandsToCommands(base.ResultPaneCommands, this.ResultPanes, resultPane, new CommandEventHandler(this.ResultPaneCommandsOfResultPane_CommandAdded), new CommandEventHandler(this.ResultPaneCommandsOfResultPane_CommandRemoved));
		}

		private void UnbindResultPaneCommandsFromResultPaneCommandsOfContainer(AbstractResultPane resultPane)
		{
			ContainerResultPane.UnbindResultPaneCommandsFromCommands(base.ResultPaneCommands, this.ResultPanes, resultPane, new CommandEventHandler(this.ResultPaneCommandsOfResultPane_CommandAdded), new CommandEventHandler(this.ResultPaneCommandsOfResultPane_CommandRemoved));
		}

		private void ResultPaneCommandsOfResultPane_CommandAdded(object sender, CommandEventArgs e)
		{
			CommandCollection resultPaneCommands = sender as CommandCollection;
			AbstractResultPane resultPane = this.FindResultPaneWithResultPaneCommandsInContainer(resultPaneCommands);
			ContainerResultPane.AddResultPaneCommandToCommands(base.ResultPaneCommands, this.ResultPanes, resultPane, e.Command);
		}

		private void ResultPaneCommandsOfResultPane_CommandRemoved(object sender, CommandEventArgs e)
		{
			CommandCollection resultPaneCommands = sender as CommandCollection;
			AbstractResultPane resultPane = this.FindResultPaneWithResultPaneCommandsInContainer(resultPaneCommands);
			ContainerResultPane.RemoveResultPaneCommandFromCommands(base.ResultPaneCommands, this.ResultPanes, resultPane, e.Command);
		}

		private bool HasImpactOnExportListCommandsOfContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane == this.SelectedResultPane;
		}

		private bool TryToBindToExportListCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnExportListCommandsOfContainer(resultPane))
			{
				ContainerResultPane.BindCommandsOfSelectedResultPaneToCommandsOfContainer(this.SelectedResultPane.ExportListCommands, base.ExportListCommands, new CommandEventHandler(this.ExportListCommandsOfSelectedResultPane_CommandAdded), new CommandEventHandler(this.ExportListCommandsOfSelectedResultPane_CommandRemoved));
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromExportListCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnExportListCommandsOfContainer(resultPane))
			{
				ContainerResultPane.UnbindCommandsOfSelectedResultPaneFromCommandsOfContainer(this.SelectedResultPane.ExportListCommands, base.ExportListCommands, new CommandEventHandler(this.ExportListCommandsOfSelectedResultPane_CommandAdded), new CommandEventHandler(this.ExportListCommandsOfSelectedResultPane_CommandRemoved));
				return true;
			}
			return false;
		}

		private void ExportListCommandsOfSelectedResultPane_CommandAdded(object sender, CommandEventArgs e)
		{
			ContainerResultPane.CommandsOfSelectedResultPane_CommandAdded(this.SelectedResultPane.ExportListCommands, base.ExportListCommands, e);
		}

		private void ExportListCommandsOfSelectedResultPane_CommandRemoved(object sender, CommandEventArgs e)
		{
			base.ExportListCommands.Remove(e.Command);
		}

		private bool HasImpactOnViewModeCommandsOfContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane == this.SelectedResultPane;
		}

		private bool TryToBindToViewModeCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnViewModeCommandsOfContainer(resultPane))
			{
				ContainerResultPane.BindCommandsOfSelectedResultPaneToCommandsOfContainer(this.SelectedResultPane.ViewModeCommands, base.ViewModeCommands, new CommandEventHandler(this.ViewModeCommandsOfSelectedResultPane_CommandAdded), new CommandEventHandler(this.ViewModeCommandsOfSelectedResultPane_CommandRemoved));
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromViewModeCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnViewModeCommandsOfContainer(resultPane))
			{
				ContainerResultPane.UnbindCommandsOfSelectedResultPaneFromCommandsOfContainer(this.SelectedResultPane.ViewModeCommands, base.ViewModeCommands, new CommandEventHandler(this.ViewModeCommandsOfSelectedResultPane_CommandAdded), new CommandEventHandler(this.ViewModeCommandsOfSelectedResultPane_CommandRemoved));
				return true;
			}
			return false;
		}

		private void ViewModeCommandsOfSelectedResultPane_CommandAdded(object sender, CommandEventArgs e)
		{
			ContainerResultPane.CommandsOfSelectedResultPane_CommandAdded(this.SelectedResultPane.ViewModeCommands, base.ViewModeCommands, e);
		}

		private void ViewModeCommandsOfSelectedResultPane_CommandRemoved(object sender, CommandEventArgs e)
		{
			base.ViewModeCommands.Remove(e.Command);
		}

		private bool HasImpactOnSelectionCommandsOfContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane.Enabled && this.IsActiveToContainer(resultPane) && this.SelectedResultPane != null && (resultPane == this.SelectedResultPane || ResultPane.IsDependedResultPane(this.SelectedResultPane, resultPane) || ResultPane.IsDependedResultPane(resultPane, this.SelectedResultPane));
		}

		private bool TryToBindToSelectionCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnSelectionCommandsOfContainer(resultPane))
			{
				if ((resultPane is ContainerResultPane && resultPane.DependedResultPane == null) || (resultPane is ResultPane && resultPane.DependedResultPane == null && (resultPane as ResultPane).DependentResultPanes.Count == 0))
				{
					ContainerResultPane.BindCommandsOfSelectedResultPaneToCommandsOfContainer(this.SelectedResultPane.SelectionCommands, base.SelectionCommands, new CommandEventHandler(this.SelectionCommandsOfSelectedResultPane_CommandAdded), new CommandEventHandler(this.SelectionCommandsOfSelectedResultPane_CommandRemoved));
				}
				else
				{
					List<AbstractResultPane> allResultPanesHavingImpactOnSelectionCommandsOfContainer = this.GetAllResultPanesHavingImpactOnSelectionCommandsOfContainer();
					List<AbstractResultPane> list = new List<AbstractResultPane>();
					list.Add(resultPane);
					if (resultPane == this.SelectedResultPane)
					{
						list = allResultPanesHavingImpactOnSelectionCommandsOfContainer;
					}
					for (int i = 0; i < list.Count; i++)
					{
						AbstractResultPane abstractResultPane = list[i];
						int positionInSelectionCommandsOfContainer = base.SelectionCommands.Count;
						int num = allResultPanesHavingImpactOnSelectionCommandsOfContainer.IndexOf(resultPane);
						if (num > 0)
						{
							positionInSelectionCommandsOfContainer = base.SelectionCommands.IndexOf(this.GetOwnerSelectionCommandInContainer(allResultPanesHavingImpactOnSelectionCommandsOfContainer[num - 1].SelectionCommands)) + 1;
						}
						else if (allResultPanesHavingImpactOnSelectionCommandsOfContainer.Count > 1 && this.GetOwnerSelectionCommandInContainer(allResultPanesHavingImpactOnSelectionCommandsOfContainer[1].SelectionCommands) != null)
						{
							positionInSelectionCommandsOfContainer = base.SelectionCommands.IndexOf(this.GetOwnerSelectionCommandInContainer(allResultPanesHavingImpactOnSelectionCommandsOfContainer[1].SelectionCommands));
						}
						this.CreateOwnerSelectionCommandInContainer(abstractResultPane, positionInSelectionCommandsOfContainer);
						abstractResultPane.SelectionChanged += this.ResultPaneHavingImpactOnSelectionCommandsOfContainer_SelectionChanged;
						this.ResultPaneHavingImpactOnSelectionCommandsOfContainer_SelectionChanged(abstractResultPane, EventArgs.Empty);
					}
				}
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromSelectionCommandsOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnSelectionCommandsOfContainer(resultPane))
			{
				if ((resultPane is ContainerResultPane && resultPane.DependedResultPane == null) || (resultPane is ResultPane && resultPane.DependedResultPane == null && (resultPane as ResultPane).DependentResultPanes.Count == 0))
				{
					ContainerResultPane.UnbindCommandsOfSelectedResultPaneFromCommandsOfContainer(this.SelectedResultPane.SelectionCommands, base.SelectionCommands, new CommandEventHandler(this.SelectionCommandsOfSelectedResultPane_CommandAdded), new CommandEventHandler(this.SelectionCommandsOfSelectedResultPane_CommandRemoved));
				}
				else
				{
					List<AbstractResultPane> list = new List<AbstractResultPane>();
					list.Add(resultPane);
					if (resultPane == this.SelectedResultPane)
					{
						list = this.GetAllResultPanesHavingImpactOnSelectionCommandsOfContainer();
					}
					foreach (AbstractResultPane abstractResultPane in list)
					{
						this.RemoveOwnerSelectionCommandInContainer(abstractResultPane);
						abstractResultPane.SelectionChanged -= this.ResultPaneHavingImpactOnSelectionCommandsOfContainer_SelectionChanged;
					}
				}
				return true;
			}
			return false;
		}

		private void SelectionCommandsOfSelectedResultPane_CommandAdded(object sender, CommandEventArgs e)
		{
			ContainerResultPane.CommandsOfSelectedResultPane_CommandAdded(this.SelectedResultPane.SelectionCommands, base.SelectionCommands, e);
		}

		private void SelectionCommandsOfSelectedResultPane_CommandRemoved(object sender, CommandEventArgs e)
		{
			base.SelectionCommands.Remove(e.Command);
		}

		private List<AbstractResultPane> GetAllResultPanesHavingImpactOnSelectionCommandsOfContainer()
		{
			List<AbstractResultPane> list = new List<AbstractResultPane>();
			list.AddRange(this.GetEnabledDependedResultPanesInContainer(this.SelectedResultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer)));
			list.Add(this.SelectedResultPane);
			if (this.SelectedResultPane is ResultPane)
			{
				list.AddRange(this.GetEnabledDependentResultPanesInContainer(this.SelectedResultPane as ResultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer)));
			}
			return list;
		}

		private void ResultPaneHavingImpactOnSelectionCommandsOfContainer_SelectionChanged(object sender, EventArgs e)
		{
			AbstractResultPane abstractResultPane = sender as AbstractResultPane;
			Command ownerSelectionCommandInContainer = this.GetOwnerSelectionCommandInContainer(abstractResultPane.SelectionCommands);
			string text = abstractResultPane.SelectionDataObject.GetText();
			ownerSelectionCommandInContainer.Visible = abstractResultPane.HasSelection;
			ownerSelectionCommandInContainer.Text = ((text == null || string.IsNullOrEmpty(text.Trim())) ? new LocalizedString(ownerSelectionCommandInContainer.Name) : new LocalizedString(this.EscapeAccelerators(text)));
		}

		private string EscapeAccelerators(string originalText)
		{
			return originalText.Replace("&", "&&");
		}

		private void CreateOwnerSelectionCommandInContainer(AbstractResultPane resultPane, int positionInSelectionCommandsOfContainer)
		{
			Command command = new Command();
			command.Style = 4;
			this.SetOwnerSelectionCommandInContainer(resultPane.SelectionCommands, command);
			command.Commands.CommandAdded += new CommandEventHandler(this.CommandsOfOwnerSelectionCommandInContainer_CommandAdded);
			command.Commands.CommandRemoved += new CommandEventHandler(this.CommandsOfOwnerSelectionCommandInContainer_CommandRemoved);
			command.Commands.AddRange(resultPane.SelectionCommands);
			resultPane.SelectionCommands.CommandAdded += new CommandEventHandler(this.SelectionCommandsOfResultPane_CommandAdded);
			resultPane.SelectionCommands.CommandRemoved += new CommandEventHandler(this.SelectionCommandsOfResultPane_CommandRemoved);
			base.SelectionCommands.Insert(positionInSelectionCommandsOfContainer, command);
		}

		private void RemoveOwnerSelectionCommandInContainer(AbstractResultPane resultPane)
		{
			resultPane.SelectionCommands.CommandAdded -= new CommandEventHandler(this.SelectionCommandsOfResultPane_CommandAdded);
			resultPane.SelectionCommands.CommandRemoved -= new CommandEventHandler(this.SelectionCommandsOfResultPane_CommandRemoved);
			Command ownerSelectionCommandInContainer = this.GetOwnerSelectionCommandInContainer(resultPane.SelectionCommands);
			this.SetOwnerSelectionCommandInContainer(resultPane.SelectionCommands, null);
			ownerSelectionCommandInContainer.Commands.Clear();
			ownerSelectionCommandInContainer.Commands.CommandAdded -= new CommandEventHandler(this.CommandsOfOwnerSelectionCommandInContainer_CommandAdded);
			ownerSelectionCommandInContainer.Commands.CommandRemoved -= new CommandEventHandler(this.CommandsOfOwnerSelectionCommandInContainer_CommandRemoved);
			base.SelectionCommands.Remove(ownerSelectionCommandInContainer);
		}

		private void CommandsOfOwnerSelectionCommandInContainer_CommandAdded(object sender, CommandEventArgs e)
		{
			e.Command.Executing += this.CommandInOwnerSelectionCommandInContainer_Executing;
		}

		private void CommandsOfOwnerSelectionCommandInContainer_CommandRemoved(object sender, CommandEventArgs e)
		{
			e.Command.Executing -= this.CommandInOwnerSelectionCommandInContainer_Executing;
		}

		private void CommandInOwnerSelectionCommandInContainer_Executing(object sender, CancelEventArgs e)
		{
			this.GetOwnerSelectionCommandInContainer(sender as Command).Invoke();
		}

		private void SelectionCommandsOfResultPane_CommandAdded(object sender, CommandEventArgs e)
		{
			Command ownerSelectionCommandInContainer = this.GetOwnerSelectionCommandInContainer(sender as CommandCollection);
			ownerSelectionCommandInContainer.Commands.Insert((sender as CommandCollection).IndexOf(e.Command), e.Command);
		}

		private void SelectionCommandsOfResultPane_CommandRemoved(object sender, CommandEventArgs e)
		{
			Command ownerSelectionCommandInContainer = this.GetOwnerSelectionCommandInContainer(sender as CommandCollection);
			ownerSelectionCommandInContainer.Commands.Remove(e.Command);
		}

		private Command GetOwnerSelectionCommandInContainer(Command selectionCommandOfResultPane)
		{
			foreach (Command command in this.ownerSelectionCommandsInContainer.Values)
			{
				if (command.Commands.Contains(selectionCommandOfResultPane))
				{
					return command;
				}
			}
			return null;
		}

		private Command GetOwnerSelectionCommandInContainer(CommandCollection selectionCommandsOfResultPane)
		{
			if (!this.ownerSelectionCommandsInContainer.ContainsKey(selectionCommandsOfResultPane))
			{
				return null;
			}
			return this.ownerSelectionCommandsInContainer[selectionCommandsOfResultPane];
		}

		private void SetOwnerSelectionCommandInContainer(CommandCollection selectionCommandsOfResultPane, Command ownerSelectionCommandInContainer)
		{
			if (ownerSelectionCommandInContainer == null)
			{
				this.ownerSelectionCommandsInContainer.Remove(selectionCommandsOfResultPane);
				return;
			}
			this.ownerSelectionCommandsInContainer[selectionCommandsOfResultPane] = ownerSelectionCommandInContainer;
		}

		private bool HasImpactOnRefreshableDataSourceOfContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane == this.SelectedResultPane;
		}

		private bool TryToBindToRefreshableDataSourceOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnRefreshableDataSourceOfContainer(resultPane))
			{
				this.RefreshableDataSource = this.SelectedResultPane.RefreshableDataSource;
				this.SelectedResultPane.RefreshableDataSourceChanged += this.SelectedResultPane_RefreshableDataSourceChanged;
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromRefreshableDataSourceOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnRefreshableDataSourceOfContainer(resultPane))
			{
				this.RefreshableDataSource = null;
				this.SelectedResultPane.RefreshableDataSourceChanged -= this.SelectedResultPane_RefreshableDataSourceChanged;
				return true;
			}
			return false;
		}

		private void SelectedResultPane_RefreshableDataSourceChanged(object sender, EventArgs e)
		{
			this.RefreshableDataSource = this.SelectedResultPane.RefreshableDataSource;
		}

		public override string SelectionHelpTopic
		{
			get
			{
				string result = null;
				for (AbstractResultPane dependedResultPane = this.SelectedResultPane; dependedResultPane != null; dependedResultPane = dependedResultPane.DependedResultPane)
				{
					if (this.ResultPanes.Contains(dependedResultPane) && dependedResultPane.Enabled && this.IsActiveToContainer(dependedResultPane) && dependedResultPane.HasSelection && !string.IsNullOrEmpty(dependedResultPane.SelectionHelpTopic))
					{
						result = dependedResultPane.SelectionHelpTopic;
						break;
					}
				}
				return result;
			}
		}

		protected override void OnSetActive(EventArgs e)
		{
			try
			{
				this.isInSetActive = true;
				foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
				{
					if (this.IsActiveToContainer(abstractResultPane))
					{
						abstractResultPane.OnSetActive();
					}
				}
			}
			finally
			{
				this.isInSetActive = false;
			}
			base.OnSetActive(e);
		}

		protected override void OnKillingActive(EventArgs e)
		{
			try
			{
				this.isInKillingActive = true;
				foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
				{
					if (abstractResultPane.IsActive)
					{
						abstractResultPane.OnKillActive();
					}
				}
			}
			finally
			{
				this.isInKillingActive = false;
			}
			base.OnKillingActive(e);
		}

		private bool HasImpactOnSelectionOfContainer(AbstractResultPane resultPane)
		{
			return this.ResultPanes.Contains(resultPane) && resultPane.Enabled && this.IsActiveToContainer(resultPane) && this.SelectedResultPane != null && (resultPane == this.SelectedResultPane || ResultPane.IsDependedResultPane(resultPane, this.SelectedResultPane));
		}

		private bool TryToBindToSelectionOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnSelectionOfContainer(resultPane))
			{
				List<AbstractResultPane> list = new List<AbstractResultPane>();
				list.Add(resultPane);
				if (resultPane == this.SelectedResultPane)
				{
					list = this.GetAllResultPanesHavingImpactOnSelectionOfContainer();
				}
				foreach (AbstractResultPane abstractResultPane in list)
				{
					abstractResultPane.SelectionChanged += this.ResultPaneHavingImpactOnSelectionOfContainer_SelectionChanged;
				}
				this.ResultPaneHavingImpactOnSelectionOfContainer_SelectionChanged(resultPane, EventArgs.Empty);
				return true;
			}
			return false;
		}

		private bool TryToUnbindFromSelectionOfContainer(AbstractResultPane resultPane)
		{
			if (this.HasImpactOnSelectionOfContainer(resultPane))
			{
				List<AbstractResultPane> list = new List<AbstractResultPane>();
				list.Add(resultPane);
				if (resultPane == this.SelectedResultPane)
				{
					list = this.GetAllResultPanesHavingImpactOnSelectionOfContainer();
				}
				foreach (AbstractResultPane abstractResultPane in list)
				{
					abstractResultPane.SelectionChanged -= this.ResultPaneHavingImpactOnSelectionOfContainer_SelectionChanged;
				}
				if (resultPane == this.SelectedResultPane)
				{
					base.UpdateSelection(null, " ", null);
				}
				else
				{
					this.ResultPaneHavingImpactOnSelectionOfContainer_SelectionChanged(this.SelectedResultPane, EventArgs.Empty);
				}
				return true;
			}
			return false;
		}

		private List<AbstractResultPane> GetAllResultPanesHavingImpactOnSelectionOfContainer()
		{
			List<AbstractResultPane> enabledDependedResultPanesInContainer = this.GetEnabledDependedResultPanesInContainer(this.SelectedResultPane, new Predicate<AbstractResultPane>(this.IsActiveToContainer));
			enabledDependedResultPanesInContainer.Add(this.SelectedResultPane);
			return enabledDependedResultPanesInContainer;
		}

		private void ResultPaneHavingImpactOnSelectionOfContainer_SelectionChanged(object sender, EventArgs e)
		{
			this.UpdateSelection();
		}

		private void UpdateSelection()
		{
			AbstractResultPane dependedResultPane = this.SelectedResultPane;
			while (dependedResultPane != null && (!this.ResultPanes.Contains(dependedResultPane) || !dependedResultPane.Enabled || !this.IsActiveToContainer(dependedResultPane) || !dependedResultPane.HasSelection))
			{
				dependedResultPane = dependedResultPane.DependedResultPane;
			}
			if (dependedResultPane != null)
			{
				string text = " ";
				if ((dependedResultPane is ContainerResultPane && dependedResultPane.DependedResultPane == null) || (dependedResultPane is ResultPane && dependedResultPane.DependedResultPane == null && (dependedResultPane as ResultPane).DependentResultPanes.Count == 0))
				{
					text = dependedResultPane.SelectionDataObject.GetText();
					if (string.IsNullOrEmpty(text))
					{
						DataListViewResultPane dataListViewResultPane = dependedResultPane as DataListViewResultPane;
						if (dataListViewResultPane != null)
						{
							text = Strings.SelectionNameDoesNotExist(dataListViewResultPane.ListControl.Columns[dataListViewResultPane.ListControl.SelectionNameProperty].Text);
						}
					}
				}
				base.UpdateSelection(dependedResultPane.SelectedObjects, text, null);
				return;
			}
			base.UpdateSelection(null, " ", null);
		}

		public void SelectResultPaneByName(string resultPaneName)
		{
			Predicate<AbstractResultPane> selectionCondition = (AbstractResultPane resultPane) => string.Compare(resultPane.Name, resultPaneName, StringComparison.InvariantCultureIgnoreCase) == 0;
			this.SelectResultPane(selectionCondition);
		}

		public bool SelectResultPane(Predicate<AbstractResultPane> selectionCondition)
		{
			foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
			{
				if (selectionCondition(abstractResultPane))
				{
					this.SelectedResultPane = abstractResultPane;
					return true;
				}
				ContainerResultPane containerResultPane = abstractResultPane as ContainerResultPane;
				if (containerResultPane != null && containerResultPane.SelectResultPane(selectionCondition))
				{
					this.SelectedResultPane = containerResultPane;
					return true;
				}
			}
			return false;
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			if (this.Site != null)
			{
				return this.Site.GetService(serviceType);
			}
			return null;
		}

		private List<AbstractResultPane> GetEnabledDependedResultPanesInContainer(AbstractResultPane resultPane)
		{
			return this.GetEnabledDependedResultPanesInContainer(resultPane, (AbstractResultPane param0) => true);
		}

		private List<AbstractResultPane> GetEnabledDependedResultPanesInContainer(AbstractResultPane resultPane, Predicate<AbstractResultPane> resultCondition)
		{
			return this.GetEnabledDependedResultPanesInContainer(resultPane, resultCondition, (AbstractResultPane dependedResultPane) => dependedResultPane == null);
		}

		private List<AbstractResultPane> GetEnabledDependedResultPanesInContainer(AbstractResultPane resultPane, Predicate<AbstractResultPane> resultCondition, Predicate<AbstractResultPane> stopCondition)
		{
			List<AbstractResultPane> list = new List<AbstractResultPane>();
			ResultPane dependedResultPane = resultPane.DependedResultPane;
			while (dependedResultPane != null && !stopCondition(dependedResultPane))
			{
				if (this.ResultPanes.Contains(dependedResultPane) && dependedResultPane.Enabled && resultCondition(dependedResultPane))
				{
					list.Insert(0, dependedResultPane);
				}
				dependedResultPane = dependedResultPane.DependedResultPane;
			}
			return list;
		}

		private AbstractResultPane GetTopEnabledResultPaneInContainer(AbstractResultPane resultPane)
		{
			return this.GetTopEnabledResultPaneInContainer(resultPane, (AbstractResultPane param0) => true);
		}

		private AbstractResultPane GetTopEnabledResultPaneInContainer(AbstractResultPane resultPane, Predicate<AbstractResultPane> resultCondition)
		{
			return this.GetTopEnabledResultPaneInContainer(resultPane, resultCondition, (AbstractResultPane dependedResultPane) => dependedResultPane == null);
		}

		private AbstractResultPane GetTopEnabledResultPaneInContainer(AbstractResultPane resultPane, Predicate<AbstractResultPane> resultCondition, Predicate<AbstractResultPane> stopCondition)
		{
			AbstractResultPane result = resultPane;
			ResultPane dependedResultPane = resultPane.DependedResultPane;
			while (dependedResultPane != null && !stopCondition(dependedResultPane))
			{
				if (this.ResultPanes.Contains(dependedResultPane) && dependedResultPane.Enabled && resultCondition(dependedResultPane))
				{
					result = dependedResultPane;
				}
				dependedResultPane = dependedResultPane.DependedResultPane;
			}
			return result;
		}

		private List<AbstractResultPane> GetEnabledDependentResultPanesInContainer(ResultPane resultPane)
		{
			return this.GetEnabledDependentResultPanesInContainer(resultPane, (AbstractResultPane param0) => true);
		}

		private List<AbstractResultPane> GetEnabledDependentResultPanesInContainer(ResultPane resultPane, Predicate<AbstractResultPane> resultCondition)
		{
			List<AbstractResultPane> list = new List<AbstractResultPane>();
			List<AbstractResultPane> list2 = new List<AbstractResultPane>();
			list2.Add(resultPane);
			while (list2.Count > 0)
			{
				AbstractResultPane abstractResultPane = list2[0];
				list2.RemoveAt(0);
				if (abstractResultPane is ResultPane)
				{
					foreach (AbstractResultPane abstractResultPane2 in (abstractResultPane as ResultPane).DependentResultPanes)
					{
						if (this.ResultPanes.Contains(abstractResultPane2) && abstractResultPane2.Enabled && resultCondition(abstractResultPane2))
						{
							list.Add(abstractResultPane2);
						}
						list2.Add(abstractResultPane2);
					}
				}
			}
			return list;
		}

		private List<AbstractResultPane> GetDirectEnabledDependentResultPanesInContainer(ResultPane resultPane)
		{
			return this.GetDirectEnabledDependentResultPanesInContainer(resultPane, (AbstractResultPane param0) => true);
		}

		private List<AbstractResultPane> GetDirectEnabledDependentResultPanesInContainer(ResultPane resultPane, Predicate<AbstractResultPane> resultCondition)
		{
			List<AbstractResultPane> list = new List<AbstractResultPane>();
			foreach (AbstractResultPane abstractResultPane in resultPane.DependentResultPanes)
			{
				if (this.ResultPanes.Contains(abstractResultPane) && abstractResultPane.Enabled && resultCondition(abstractResultPane))
				{
					list.Add(abstractResultPane);
				}
			}
			return list;
		}

		private static void BindCommandsOfSelectedResultPaneToCommandsOfContainer(CommandCollection commandsOfSelectedResultPane, CommandCollection commandsOfContainer, CommandEventHandler commandAddedHandler, CommandEventHandler commandRemovedHandler)
		{
			commandsOfContainer.InsertRange(0, commandsOfSelectedResultPane);
			commandsOfSelectedResultPane.CommandAdded += commandAddedHandler;
			commandsOfSelectedResultPane.CommandRemoved += commandRemovedHandler;
		}

		private static void UnbindCommandsOfSelectedResultPaneFromCommandsOfContainer(CommandCollection commandsOfSelectedResultPane, CommandCollection commandsOfContainer, CommandEventHandler commandAddedHandler, CommandEventHandler commandRemovedHandler)
		{
			commandsOfSelectedResultPane.CommandAdded -= commandAddedHandler;
			commandsOfSelectedResultPane.CommandRemoved -= commandRemovedHandler;
			if (commandsOfSelectedResultPane.Count > 0)
			{
				int num = commandsOfContainer.IndexOf(commandsOfSelectedResultPane[0]);
				if (num >= 0)
				{
					commandsOfContainer.RemoveRange(num, commandsOfSelectedResultPane.Count);
				}
			}
		}

		private static void CommandsOfSelectedResultPane_CommandAdded(CommandCollection commandsOfSelectedResultPane, CommandCollection commandsOfContainer, CommandEventArgs e)
		{
			if (commandsOfSelectedResultPane.Count > 1)
			{
				int num = commandsOfContainer.IndexOf(commandsOfSelectedResultPane[0]);
				int num2 = commandsOfSelectedResultPane.IndexOf(e.Command);
				commandsOfContainer.Insert(num + num2, e.Command);
				return;
			}
			commandsOfContainer.Insert(0, e.Command);
		}

		private AbstractResultPane FindResultPaneWithResultPaneCommandsInContainer(CommandCollection resultPaneCommands)
		{
			foreach (AbstractResultPane abstractResultPane in this.ResultPanes)
			{
				if (abstractResultPane.ResultPaneCommands == resultPaneCommands)
				{
					return abstractResultPane;
				}
			}
			return null;
		}

		private static void BindResultPaneCommandsToCommands(CommandCollection targetCommands, IList<AbstractResultPane> sourceResultPanes, AbstractResultPane resultPane, CommandEventHandler commandAddedEventHandler, CommandEventHandler commandRemovedEventHandler)
		{
			targetCommands.InsertRange(ContainerResultPane.FindInsertionPostionForResultPaneCommands(targetCommands, sourceResultPanes, resultPane), resultPane.ResultPaneCommands);
			resultPane.ResultPaneCommands.CommandAdded += commandAddedEventHandler;
			resultPane.ResultPaneCommands.CommandRemoved += commandRemovedEventHandler;
		}

		private static void UnbindResultPaneCommandsFromCommands(CommandCollection targetCommands, IList<AbstractResultPane> sourceResultPanes, AbstractResultPane resultPane, CommandEventHandler commandAddedEventHandler, CommandEventHandler commandRemovedEventHandler)
		{
			resultPane.ResultPaneCommands.CommandAdded -= commandAddedEventHandler;
			resultPane.ResultPaneCommands.CommandRemoved -= commandRemovedEventHandler;
			foreach (object obj in resultPane.ResultPaneCommands)
			{
				Command command = (Command)obj;
				targetCommands.Remove(command);
			}
		}

		private static void AddResultPaneCommandToCommands(CommandCollection targetCommands, IList<AbstractResultPane> sourceResultPanes, AbstractResultPane resultPane, Command resultPaneCommand)
		{
			int num = ContainerResultPane.FindInsertionPostionForResultPaneCommands(targetCommands, sourceResultPanes, resultPane) + resultPane.ResultPaneCommands.IndexOf(resultPaneCommand);
			targetCommands.Insert(num, resultPaneCommand);
		}

		private static void RemoveResultPaneCommandFromCommands(CommandCollection targetCommands, IList<AbstractResultPane> sourceResultPanes, AbstractResultPane resultPane, Command resultPaneCommand)
		{
			targetCommands.Remove(resultPaneCommand);
		}

		private static int FindInsertionPostionForResultPaneCommands(CommandCollection targetCommands, IList<AbstractResultPane> sourceResultPanes, AbstractResultPane resultPane)
		{
			int result = 0;
			int num = sourceResultPanes.IndexOf(resultPane);
			for (int i = num - 1; i >= 0; i--)
			{
				AbstractResultPane abstractResultPane = sourceResultPanes[i];
				if (abstractResultPane.ResultPaneCommands.Count > 0 && targetCommands.Contains(abstractResultPane.ResultPaneCommands[0]))
				{
					result = targetCommands.IndexOf(abstractResultPane.ResultPaneCommands[0]) + abstractResultPane.ResultPaneCommands.Count;
					break;
				}
			}
			return result;
		}

		private ServicedContainer components;

		private ChangeNotifyingCollection<AbstractResultPane> resultPanes = new ChangeNotifyingCollection<AbstractResultPane>();

		private AbstractResultPane selectedResultPane;

		private static readonly object EventSelectedResultPaneChanged = new object();

		private ChangeNotifyingCollection<AbstractResultPane> resultPanesActiveToContainer = new ChangeNotifyingCollection<AbstractResultPane>();

		private Dictionary<CommandCollection, Command> ownerSelectionCommandsInContainer = new Dictionary<CommandCollection, Command>();

		private bool isInSetActive;

		private bool isInKillingActive;
	}
}
