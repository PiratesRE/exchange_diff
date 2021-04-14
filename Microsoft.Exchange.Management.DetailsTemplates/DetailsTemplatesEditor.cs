using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Services;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal partial class DetailsTemplatesEditor : ExchangeForm
	{
		internal DetailsTemplatesEditorSettings PrivateSettings
		{
			get
			{
				return this.privateSettings;
			}
			set
			{
				this.privateSettings = value;
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

		internal DetailsTemplatesEditor(string templateIdentity) : this()
		{
			this.templatePage.BindingSource.DataSource = typeof(DetailsTemplate);
			MonadDataHandler monadDataHandler = new MonadDataHandler(string.Empty, typeof(DetailsTemplate));
			monadDataHandler.Identity = templateIdentity;
			this.templatePage.Context = new DataContext(monadDataHandler, true);
			this.Text = templateIdentity;
		}

		internal DetailsTemplatesEditor()
		{
			this.InitializeComponent();
			this.serviceContainer = new ServiceContainer();
			ServicedContainer servicedContainer = new ServicedContainer(this.serviceContainer);
			this.templatePage = new DetailsTemplatesPropertyPage(this.serviceContainer);
			this.templatePage.Dock = DockStyle.Fill;
			this.propertyGrid = new PropertyGrid();
			this.propertyGrid.CommandsVisibleIfAvailable = false;
			this.propertyGrid.Dock = DockStyle.Fill;
			this.toolbox = new Toolbox(this.templatePage.TemplateSurface);
			this.toolbox.Dock = DockStyle.Fill;
			this.splitContainer2.Panel1.TabIndex = 1;
			this.splitContainer2.Panel2.TabIndex = 2;
			this.splitContainer2.Panel1.Controls.Add(this.toolbox);
			this.splitContainer2.Panel2.Controls.Add(this.templatePage);
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainer1.Panel1.TabIndex = 0;
			this.splitContainer1.Panel2.TabIndex = 3;
			this.serviceContainer.AddService(typeof(IToolboxService), this.toolbox);
			this.serviceContainer.AddService(typeof(PropertyGrid), this.propertyGrid);
			this.serviceContainer.AddService(typeof(UIService), base.ShellUI);
			servicedContainer.Add(this.propertyGrid);
			servicedContainer.Add(this.toolbox);
			this.templatePage.TemplateSurface.ExchangeForm = this;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.templatePage.BindingSource.DataSource != null)
			{
				this.templatePage.OnSetActive();
				DetailsTemplate detailsTemplate = this.templatePage.BindingSource.DataSource as DetailsTemplate;
				if (detailsTemplate != null && detailsTemplate.IsValid)
				{
					this.serviceContainer.AddService(typeof(DetailsTemplateTypeService), new DetailsTemplateTypeService(detailsTemplate.TemplateType, detailsTemplate.MAPIPropertiesDictionary));
					if (detailsTemplate.TemplateType.Equals("Mailbox Agent"))
					{
						this.propertyGrid.Enabled = false;
						this.templatePage.TemplateSurface.ReadOnly = true;
					}
					else
					{
						this.propertyGrid.Enabled = true;
						this.templatePage.TemplateSurface.ReadOnly = false;
					}
					this.templatePage.TemplateSurface.DetailsTemplatesMenuService.MenuCommandStatusChanged += this.menuCommandService_StatusChanged;
					this.UpdateEditMenuItems(this.templatePage.TemplateSurface.DetailsTemplatesMenuService);
					this.toolbox.Initialize();
					this.LoadEditorSettings();
					return;
				}
				if (detailsTemplate != null)
				{
					base.ShellUI.ShowError(Strings.DetailsTemplateCorrupted);
				}
				base.Dispose();
			}
		}

		private void menuCommandService_StatusChanged(object sender, EventArgs e)
		{
			this.UpdateEditMenuItems(sender as DetailsTemplatesMenuService);
		}

		private void UpdateEditMenuItems(DetailsTemplatesMenuService menuCommandService)
		{
			this.UpdateEditMenuItems(this.editMenuItem.MenuItems, menuCommandService);
			this.UpdateEditMenuItems(this.EditorContextMenu.MenuItems, menuCommandService);
		}

		private void UpdateEditMenuItems(Menu.MenuItemCollection menuItems, DetailsTemplatesMenuService menuCommandService)
		{
			if (menuItems != null)
			{
				foreach (object obj in menuItems)
				{
					MenuItem menuItem = (MenuItem)obj;
					CommandID commandID = menuItem.Tag as CommandID;
					if (commandID != null)
					{
						menuItem.Enabled = (menuCommandService != null && menuCommandService.IsMenuCommandEnabled(commandID));
					}
				}
			}
		}

		private void SaveTemplate(CancelEventArgs e)
		{
			TabControl templateTab = this.templatePage.TemplateSurface.TemplateTab;
			MultiValuedProperty<Page> multiValuedProperty = new MultiValuedProperty<Page>();
			foreach (object obj in templateTab.TabPages)
			{
				CustomTabPage customTabPage = (CustomTabPage)obj;
				Page detailsTemplateTab = customTabPage.DetailsTemplateTab;
				detailsTemplateTab.Controls.Clear();
				DetailsTemplatesSurface.SortControls(customTabPage, true);
				foreach (object obj2 in customTabPage.Controls)
				{
					Control control = (Control)obj2;
					DetailsTemplateControl detailsTemplateControl = (control as IDetailsTemplateControlBound).DetailsTemplateControl;
					detailsTemplateTab.Controls.Add(detailsTemplateControl);
				}
				DetailsTemplatesSurface.SortControls(customTabPage, false);
				multiValuedProperty.Add(detailsTemplateTab);
			}
			DetailsTemplate detailsTemplate = this.templatePage.BindingSource.DataSource as DetailsTemplate;
			detailsTemplate.Pages = multiValuedProperty;
			if (this.templatePage.DataHandler != null)
			{
				this.templatePage.DataHandler.SpecifyParameterNames("Pages");
			}
			this.templatePage.Apply(e);
			this.RefreshOnFinish.Refresh(base.CreateProgress(Strings.Refreshing));
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SaveTemplate(new CancelEventArgs());
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.OnHelpRequested(new HelpEventArgs(Point.Empty));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!this.templatePage.OnKillActive())
			{
				e.Cancel = true;
			}
			else if (this.templatePage.Context.IsDirty)
			{
				DialogResult dialogResult = base.ShellUI.ShowMessage(Strings.SaveMessage, UIService.DefaultCaption, MessageBoxButtons.YesNoCancel);
				if (dialogResult == DialogResult.Yes)
				{
					this.SaveTemplate(e);
				}
				if (dialogResult == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
			this.SaveEditorSettings();
		}

		private void LoadEditorSettings()
		{
			if (this.PrivateSettings != null)
			{
				base.SuspendLayout();
				Rectangle workingArea = Screen.GetWorkingArea(this);
				int num = (int)this.PrivateSettings.EditorXCoordinate;
				int num2 = (int)this.PrivateSettings.EditorYCoordinate;
				int num3 = (int)this.PrivateSettings.EditorWidth;
				int num4 = (int)this.PrivateSettings.EditorHeight;
				if (workingArea.Width > 0 && workingArea.Height > 0)
				{
					if (num3 > workingArea.Width)
					{
						num3 = workingArea.Width;
						num = 0;
					}
					if (num4 > workingArea.Height)
					{
						num4 = workingArea.Height;
						num2 = 0;
					}
					if (num3 + num > workingArea.Width)
					{
						num = workingArea.Width - num3;
					}
					if (num4 + num2 > workingArea.Height)
					{
						num2 = workingArea.Height - num4;
					}
					num = Math.Max(num, workingArea.Left);
					num2 = Math.Max(num2, workingArea.Top);
				}
				base.Left = num;
				base.Top = num2;
				base.Width = num3;
				base.Height = num4;
				if (this.PrivateSettings.IsEditorMaximized)
				{
					base.WindowState = FormWindowState.Maximized;
				}
				base.ResumeLayout();
			}
		}

		private void SaveEditorSettings()
		{
			if (this.PrivateSettings != null)
			{
				if (base.WindowState == FormWindowState.Normal)
				{
					this.PrivateSettings.EditorXCoordinate = (uint)base.Left;
					this.PrivateSettings.EditorYCoordinate = (uint)base.Top;
					this.PrivateSettings.EditorWidth = (uint)base.Width;
					this.PrivateSettings.EditorHeight = (uint)base.Height;
				}
				else
				{
					this.PrivateSettings.EditorXCoordinate = (uint)base.RestoreBounds.Left;
					this.PrivateSettings.EditorYCoordinate = (uint)base.RestoreBounds.Top;
					this.PrivateSettings.EditorWidth = (uint)base.RestoreBounds.Width;
					this.PrivateSettings.EditorHeight = (uint)base.RestoreBounds.Height;
				}
				if (FormWindowState.Maximized == base.WindowState)
				{
					this.PrivateSettings.IsEditorMaximized = true;
					return;
				}
				this.PrivateSettings.IsEditorMaximized = false;
			}
		}

		public ContextMenu EditorContextMenu
		{
			get
			{
				return this.editorContextMenu;
			}
		}

		private void editMenuItems_Click(object sender, EventArgs e)
		{
			MenuItem menuItem = sender as MenuItem;
			if (menuItem != null)
			{
				CommandID commandId = menuItem.Tag as CommandID;
				this.ExecuteCommand(commandId);
			}
		}

		internal bool ExecuteCommand(CommandID commandId)
		{
			bool result = false;
			IMenuCommandService menuCommandService = this.templatePage.TemplateSurface.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if (menuCommandService != null && commandId != null)
			{
				result = menuCommandService.GlobalInvoke(commandId);
			}
			return result;
		}

		internal void SelectNextSiblingOfDesignSurface(bool forward)
		{
			if (forward)
			{
				this.propertyGrid.Select();
				this.propertyGrid.Focus();
				return;
			}
			this.toolbox.Focus();
			this.toolbox.Select();
		}

		private DetailsTemplatesPropertyPage templatePage;

		private PropertyGrid propertyGrid;

		private Toolbox toolbox;

		private ServiceContainer serviceContainer;

		private IRefreshable refreshOnFinish;

		private DetailsTemplatesEditorSettings privateSettings;
	}
}
