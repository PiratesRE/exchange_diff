using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplatesMenuService : MenuCommandService
	{
		internal DetailsTemplatesMenuService(DetailsTemplatesSurface detailsTemplatesDesignSurface) : base(detailsTemplatesDesignSurface)
		{
			this.designSurface = detailsTemplatesDesignSurface;
			this.AddCommand(new MenuCommand(new EventHandler(this.ExecuteUndo), StandardCommands.Undo)
			{
				Enabled = false
			});
			this.AddCommand(new MenuCommand(new EventHandler(this.ExecuteRedo), StandardCommands.Redo)
			{
				Enabled = false
			});
			MenuCommand command = new MenuCommand(new EventHandler(this.AddNewTabPage), DetailsTemplatesMenuService.AddTabPageCommandId);
			this.AddCommand(command);
			MenuCommand command2 = new MenuCommand(new EventHandler(this.RemoveCurrentTabPage), DetailsTemplatesMenuService.RemoveTabPageCommandId);
			this.AddCommand(command2);
			MenuCommand command3 = new MenuCommand(new EventHandler(this.DoSelectNextControl), DetailsTemplatesMenuService.SelectNextControl);
			this.AddCommand(command3);
			this.readOnlyCommandList.Add(DetailsTemplatesMenuService.SelectNextControl);
			MenuCommand command4 = new MenuCommand(new EventHandler(this.DoSelectPreviousControl), DetailsTemplatesMenuService.SelectPreviousControl);
			this.AddCommand(command4);
			this.readOnlyCommandList.Add(DetailsTemplatesMenuService.SelectPreviousControl);
			MenuCommand command5 = new MenuCommand(new EventHandler(this.DoSwitchTabPage), DetailsTemplatesMenuService.SwitchTabPage);
			this.AddCommand(command5);
			this.readOnlyCommandList.Add(DetailsTemplatesMenuService.SwitchTabPage);
			MenuCommand command6 = new MenuCommand(new EventHandler(this.DoSelectAllInCurrentTab), DetailsTemplatesMenuService.SelectAllCommandId);
			this.AddCommand(command6);
			this.readOnlyCommandList.Add(DetailsTemplatesMenuService.SelectAllCommandId);
			this.selectionService = (base.GetService(typeof(ISelectionService)) as ISelectionService);
			if (this.selectionService != null)
			{
				this.selectionService.SelectionChanged += this.selectionService_SelectionChanged;
			}
		}

		private bool IsReadOnlyCommand(CommandID commandID)
		{
			return commandID != null && this.readOnlyCommandList.Contains(commandID);
		}

		private void selectionService_SelectionChanged(object sender, EventArgs e)
		{
			if (this.Enabled)
			{
				this.OnMenuCommandStatusChanged();
			}
		}

		public bool IsMenuCommandEnabled(CommandID commandID)
		{
			bool flag = false;
			if (commandID != null && (this.Enabled || this.IsReadOnlyCommand(commandID)))
			{
				object obj = (this.selectionService != null) ? this.selectionService.PrimarySelection : null;
				int num = (this.selectionService != null) ? this.selectionService.SelectionCount : 0;
				MenuCommand menuCommand = base.FindCommand(commandID);
				if (menuCommand != null)
				{
					if (StandardCommands.Undo.Equals(commandID) || StandardCommands.Redo.Equals(commandID) || DetailsTemplatesMenuService.SelectNextControl.Equals(commandID) || DetailsTemplatesMenuService.SelectPreviousControl.Equals(commandID) || DetailsTemplatesMenuService.SwitchTabPage.Equals(commandID))
					{
						flag = true;
					}
					else if (DetailsTemplatesMenuService.AddTabPageCommandId.Equals(commandID) || DetailsTemplatesMenuService.RemoveTabPageCommandId.Equals(commandID))
					{
						flag = (obj is TabControl);
					}
					else if (StandardCommands.Paste.Equals(commandID))
					{
						flag = (obj is TabPage && num == 1);
					}
					else
					{
						flag = (DetailsTemplatesMenuService.SelectAllCommandId.Equals(commandID) || (!this.ControlTypeSelected(typeof(TabControl)) && !this.ControlTypeSelected(typeof(TabPage))));
					}
					flag = (flag && menuCommand.Enabled);
				}
			}
			return flag;
		}

		private bool ControlTypeSelected(Type type)
		{
			bool result = false;
			ICollection collection = (this.selectionService == null) ? null : this.selectionService.GetSelectedComponents();
			if (collection != null)
			{
				foreach (object obj in collection)
				{
					if (type.IsAssignableFrom(obj.GetType()))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public override bool GlobalInvoke(CommandID commandID)
		{
			return this.IsMenuCommandEnabled(commandID) && base.GlobalInvoke(commandID);
		}

		protected override void OnCommandsChanged(MenuCommandsChangedEventArgs e)
		{
			base.OnCommandsChanged(e);
			this.OnMenuCommandStatusChanged();
		}

		public override void ShowContextMenu(CommandID menuID, int x, int y)
		{
			ContextMenu contextMenu = (this.designSurface != null) ? this.designSurface.GetContextMenu() : null;
			Control control = (this.selectionService != null) ? (this.selectionService.PrimarySelection as Control) : null;
			if (control != null && contextMenu != null)
			{
				Point point = control.PointToScreen(new Point(0, 0));
				contextMenu.Show(control, new Point(x - point.X, y - point.Y));
			}
		}

		[DefaultValue(false)]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (this.Enabled != value)
				{
					this.enabled = value;
					this.OnMenuCommandStatusChanged();
				}
			}
		}

		public event EventHandler MenuCommandStatusChanged;

		private void OnMenuCommandStatusChanged()
		{
			if (this.MenuCommandStatusChanged != null)
			{
				this.MenuCommandStatusChanged(this, EventArgs.Empty);
			}
		}

		private void AddNewTabPage(object sender, EventArgs args)
		{
			DetailsTemplatesMenuService.AddTab(this.designSurface);
		}

		private void RemoveCurrentTabPage(object sender, EventArgs args)
		{
			DetailsTemplatesMenuService.RemoveTab(this.designSurface);
		}

		private static void RemoveTab(DetailsTemplatesSurface designSurface)
		{
			UIService uiservice = null;
			IDesignerHost designerHost = designSurface.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if (designerHost != null)
			{
				uiservice = (designerHost.GetService(typeof(UIService)) as UIService);
			}
			if (uiservice != null)
			{
				if (designSurface.TemplateTab.Controls.Count <= 1)
				{
					uiservice.ShowMessage(Strings.CannotDeletePage);
					return;
				}
				TabPage selectedTab = designSurface.TemplateTab.SelectedTab;
				if (selectedTab != null)
				{
					DialogResult dialogResult = uiservice.ShowMessage(Strings.ConfirmDeleteTab(selectedTab.Text), UIService.DefaultCaption, MessageBoxButtons.OKCancel);
					if (dialogResult == DialogResult.OK)
					{
						DesignerTransaction designerTransaction = null;
						try
						{
							designerTransaction = designerHost.CreateTransaction("TabControlRemoveTabPage" + designSurface.TemplateTab.Site.Name);
							designerHost.DestroyComponent(selectedTab);
						}
						finally
						{
							if (designerTransaction != null)
							{
								designerTransaction.Commit();
							}
						}
						designSurface.DataContext.IsDirty = true;
					}
				}
			}
		}

		internal static void AddTab(DetailsTemplatesSurface designSurface)
		{
			if (designSurface != null)
			{
				IDesignerHost designerHost = designSurface.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (designerHost != null)
				{
					DesignerTransaction designerTransaction = null;
					try
					{
						designerTransaction = designerHost.CreateTransaction("TabControlAddTabPage" + designSurface.TemplateTab.Site.Name);
						Hashtable hashtable = new Hashtable();
						hashtable["Parent"] = designSurface.TemplateTab;
						hashtable["TabPageIndex"] = designSurface.TemplateTab.SelectedIndex + 1;
						ToolboxItem toolboxItem = new ToolboxItem(typeof(CustomTabPage));
						toolboxItem.CreateComponents(designerHost, hashtable);
					}
					finally
					{
						if (designerTransaction != null)
						{
							designerTransaction.Commit();
						}
					}
				}
			}
		}

		private void ExecuteUndo(object sender, EventArgs e)
		{
			DetailsTemplateUndoEngine detailsTemplateUndoEngine = base.GetService(typeof(UndoEngine)) as DetailsTemplateUndoEngine;
			if (detailsTemplateUndoEngine != null)
			{
				detailsTemplateUndoEngine.DoUndo();
			}
		}

		private void ExecuteRedo(object sender, EventArgs e)
		{
			DetailsTemplateUndoEngine detailsTemplateUndoEngine = base.GetService(typeof(UndoEngine)) as DetailsTemplateUndoEngine;
			if (detailsTemplateUndoEngine != null)
			{
				detailsTemplateUndoEngine.DoRedo();
			}
		}

		private void DoSelectNextControl(object sender, EventArgs e)
		{
			this.SelectNextControlInCurrentTabPage(true);
		}

		private void DoSelectPreviousControl(object sender, EventArgs e)
		{
			this.SelectNextControlInCurrentTabPage(false);
		}

		private void SelectNextControlInCurrentTabPage(bool forward)
		{
			if (this.selectionService != null && this.designSurface.TemplateTab != null && this.designSurface.TemplateTab.SelectedTab != null)
			{
				Control control = null;
				Component component = this.selectionService.PrimarySelection as Component;
				if (forward && component == this.designSurface.TemplateTab)
				{
					control = this.designSurface.TemplateTab.SelectedTab;
				}
				else if (component != null)
				{
					control = this.designSurface.TemplateTab.SelectedTab.GetNextControl(component as Control, forward);
				}
				if (!forward)
				{
					if (component == this.designSurface.TemplateTab.SelectedTab)
					{
						control = this.designSurface.TemplateTab;
					}
					else if (component == this.designSurface.TemplateTab)
					{
						control = null;
					}
					else if (control == null)
					{
						control = this.designSurface.TemplateTab.SelectedTab;
					}
				}
				if (control != null)
				{
					this.selectionService.SetSelectedComponents(new Component[]
					{
						control
					}, SelectionTypes.Replace);
					return;
				}
				DetailsTemplatesEditor detailsTemplatesEditor = this.designSurface.ExchangeForm as DetailsTemplatesEditor;
				if (detailsTemplatesEditor != null)
				{
					detailsTemplatesEditor.SelectNextSiblingOfDesignSurface(forward);
				}
			}
		}

		private void DoSwitchTabPage(object sender, EventArgs e)
		{
			if (this.selectionService != null && this.designSurface.TemplateTab != null)
			{
				int num = this.designSurface.TemplateTab.SelectedIndex;
				num++;
				if (this.designSurface.TemplateTab.TabCount > 0)
				{
					num %= this.designSurface.TemplateTab.TabCount;
				}
				else
				{
					num = -1;
				}
				if (num >= 0)
				{
					this.selectionService.SetSelectedComponents(new Component[]
					{
						this.designSurface.TemplateTab.TabPages[num]
					}, SelectionTypes.Replace);
				}
			}
		}

		private void DoSelectAllInCurrentTab(object sender, EventArgs e)
		{
			if (this.selectionService != null && this.designSurface.TemplateTab != null && this.designSurface.TemplateTab.SelectedTab != null)
			{
				this.selectionService.SetSelectedComponents(this.designSurface.TemplateTab.SelectedTab.Controls, SelectionTypes.Replace);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.selectionService != null)
			{
				this.selectionService.SelectionChanged -= this.selectionService_SelectionChanged;
				this.selectionService = null;
				this.designSurface = null;
			}
			base.Dispose(disposing);
		}

		internal static CommandID AddTabPageCommandId = new CommandID(Guid.NewGuid(), 1);

		internal static CommandID RemoveTabPageCommandId = new CommandID(DetailsTemplatesMenuService.AddTabPageCommandId.Guid, 2);

		internal static CommandID SelectNextControl = new CommandID(DetailsTemplatesMenuService.AddTabPageCommandId.Guid, 3);

		internal static CommandID SelectPreviousControl = new CommandID(DetailsTemplatesMenuService.AddTabPageCommandId.Guid, 4);

		internal static CommandID SwitchTabPage = new CommandID(DetailsTemplatesMenuService.AddTabPageCommandId.Guid, 5);

		internal static CommandID SelectAllCommandId = new CommandID(DetailsTemplatesMenuService.AddTabPageCommandId.Guid, 6);

		private DetailsTemplatesSurface designSurface;

		private ISelectionService selectionService;

		private HashSet<CommandID> readOnlyCommandList = new HashSet<CommandID>();

		private bool enabled;
	}
}
