using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class AbstractResultPane : ExchangeUserControl, IResultPaneControl, IPersistComponentSettings, IHasPermission
	{
		protected AbstractResultPane()
		{
			base.Name = "AbstractResultPane";
			base.SuspendLayout();
			base.SetStyle(ControlStyles.ContainerControl, true);
			base.ResumeLayout(false);
			SystemEvents.UserPreferenceChanged += this.SystemEvents_UserPreferenceChanged;
			this.UpdateFontSetting();
			this.SharedSettingsBindingSource = new BindingSource(base.Components);
			this.RegisterCommandsEvent(this.ResultPaneCommands);
			this.RegisterCommandsEvent(this.ExportListCommands);
			this.RegisterCommandsEvent(this.ViewModeCommands);
			this.RegisterCommandsEvent(this.SelectionCommands);
			this.RefreshCommand.Text = Strings.RefreshCommand;
			this.RefreshCommand.Icon = Icons.Refresh;
			this.RefreshCommand.Name = "Refresh";
			this.RefreshCommand.Execute += this.RefreshCommand_Execute;
			this.RefreshCommand.EnabledChanged += delegate(object param0, EventArgs param1)
			{
				this.RefreshCommand.Visible = this.RefreshCommand.Enabled;
			};
			this.RefreshCommand.Enabled = false;
			this.HelpCommand.Name = "Help";
			this.HelpCommand.Icon = Icons.Help;
			this.HelpCommand.Text = Strings.ShowHelpCommand;
			this.HelpCommand.Execute += delegate(object param0, EventArgs param1)
			{
				this.OnHelpRequested(new HelpEventArgs(Point.Empty));
			};
			this.contextMenuLocal = new CommandContextMenu(this.contextMenuCommands);
			this.ContextMenu = this.contextMenuLocal;
			this.ContextMenu.Popup += this.ContextMenu_Popup;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (this.RefreshableDataSource is DataTableLoader || this.RefreshableDataSource is AggregateDataTableLoadersRefreshableSource)
				{
					base.Components.Remove(this.RefreshableDataSource as IComponent);
				}
				this.ContextMenuCommands.Clear();
				this.ContextMenu = null;
				if (this.contextMenuLocal != null)
				{
					this.contextMenuLocal.Dispose();
					this.contextMenuLocal = null;
				}
				this.RefreshableDataSource = null;
				this.SelectionCommands.Clear();
				this.UnregisterCommandsEvent(this.SelectionCommands);
				this.ViewModeCommands.Clear();
				this.UnregisterCommandsEvent(this.ViewModeCommands);
				this.ExportListCommands.Clear();
				this.UnregisterCommandsEvent(this.ExportListCommands);
				this.ResultPaneCommands.Clear();
				this.UnregisterCommandsEvent(this.ResultPaneCommands);
				this.SharedSettings = null;
				SystemEvents.UserPreferenceChanged -= this.SystemEvents_UserPreferenceChanged;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.SettingsKey = base.Name;
		}

		protected override void OnParentChanged(EventArgs e)
		{
			if (base.Parent != null && base.Parent.GetType().Name == "FormViewContainerControl")
			{
				base.SetBounds(0, 0, base.Parent.Width, base.Parent.Height);
			}
			base.OnParentChanged(e);
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
			e.Command.VisibleChanged += this.Command_VisibleChanged;
			this.ScanAllSeparators(sender as CommandCollection);
		}

		private void Command_VisibleChanged(object sender, EventArgs e)
		{
			Command command = sender as Command;
			if (!command.IsSeparator)
			{
				this.ScanAllSeparators(this.GetCommandCollection(command));
			}
		}

		private void Commands_CommandRemoved(object sender, CommandEventArgs e)
		{
			e.Command.VisibleChanged -= this.Command_VisibleChanged;
			this.ScanAllSeparators(sender as CommandCollection);
		}

		private CommandCollection GetCommandCollection(Command command)
		{
			CommandCollection result = null;
			if (this.ResultPaneCommands.Contains(command))
			{
				result = this.ResultPaneCommands;
			}
			else if (this.ViewModeCommands.Contains(command))
			{
				result = this.ViewModeCommands;
			}
			else if (this.ExportListCommands.Contains(command))
			{
				result = this.ExportListCommands;
			}
			else if (this.SelectionCommands.Contains(command))
			{
				result = this.SelectionCommands;
			}
			return result;
		}

		public virtual bool HasPermission()
		{
			return true;
		}

		public ContainerResultPane ContainerResultPane
		{
			get
			{
				return this.containerResultPane;
			}
			internal set
			{
				this.containerResultPane = value;
			}
		}

		public ResultPane DependedResultPane
		{
			get
			{
				return this.dependedResultPane;
			}
			internal set
			{
				if (this.DependedResultPane != value)
				{
					this.dependedResultPane = value;
				}
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(400, 400);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		private bool ShouldSerializeFont()
		{
			return this.Font == FontHelper.GetDefaultFont();
		}

		private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category == UserPreferenceCategory.Window)
			{
				this.UpdateFontSetting();
			}
		}

		private void UpdateFontSetting()
		{
			base.SuspendLayout();
			this.Font = FontHelper.GetDefaultFont();
			base.ResumeLayout(false);
		}

		[Category("Result Pane")]
		[DefaultValue(null)]
		public Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (this.Icon != value)
				{
					this.icon = value;
					this.OnIconChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIconChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventIconChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IconChanged
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventIconChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventIconChanged, value);
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			this.needSuppressHelp = (Keys.F1 != keyData && (Keys.F1 & keyData) == Keys.F1 && ((Keys)458864 & keyData) == keyData);
			return Keys.F1 == keyData || base.ProcessCmdKey(ref msg, keyData);
		}

		public BindingSource SharedSettingsBindingSource
		{
			get
			{
				return this.sharedSettingsBindingSource;
			}
			private set
			{
				this.sharedSettingsBindingSource = value;
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ExchangeSettings SharedSettings
		{
			get
			{
				return this.sharedSettings;
			}
			set
			{
				if (value != this.SharedSettings)
				{
					this.OnSharedSettingsChanging();
					this.sharedSettings = value;
					this.SharedSettingsBindingSource.DataSource = this.sharedSettings;
					this.OnSharedSettingsChanged();
				}
			}
		}

		protected virtual void OnSharedSettingsChanging()
		{
		}

		protected virtual void OnSharedSettingsChanged()
		{
			if (this.SharedSettings != null && !this.dataBoundToSettings)
			{
				this.OnBindToSharedSettings();
				this.dataBoundToSettings = true;
			}
		}

		protected virtual void OnBindToSharedSettings()
		{
		}

		public ExchangeSettings PrivateSettings
		{
			get
			{
				if (this.privateSettings == null)
				{
					this.privateSettings = this.CreatePrivateSettings(this);
				}
				return this.privateSettings;
			}
		}

		protected virtual ExchangeSettings CreatePrivateSettings(IComponent owner)
		{
			return new ExchangeSettings(owner);
		}

		[DefaultValue(true)]
		public virtual bool SaveSettings
		{
			get
			{
				return true;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DefaultValue("")]
		public string SettingsKey
		{
			get
			{
				return this.PrivateSettings.SettingsKey;
			}
			set
			{
				this.PrivateSettings.SettingsKey = value;
			}
		}

		public virtual void LoadComponentSettings()
		{
			ISettingsProviderService provSvc = this.Site.GetService(typeof(ISettingsProviderService)) as ISettingsProviderService;
			this.PrivateSettings.UpdateProviders(provSvc);
			this.PrivateSettings.Reload();
		}

		public virtual void ResetComponentSettings()
		{
			this.PrivateSettings.Reset();
		}

		public virtual void SaveComponentSettings()
		{
			ISettingsProviderService provSvc = this.Site.GetService(typeof(ISettingsProviderService)) as ISettingsProviderService;
			this.PrivateSettings.UpdateProviders(provSvc);
			this.PrivateSettings.Save();
		}

		[Category("Result Pane")]
		[DefaultValue("")]
		public string Status
		{
			get
			{
				return this.status;
			}
			set
			{
				if (string.Compare(this.Status, value) != 0)
				{
					if (value == null)
					{
						this.status = "";
					}
					else
					{
						this.status = value;
					}
					this.OnStatusChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnStatusChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventStatusChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler StatusChanged
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventStatusChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventStatusChanged, value);
			}
		}

		[DefaultValue(false)]
		public bool IsModified
		{
			get
			{
				return this.isModified;
			}
			set
			{
				if (this.IsModified != value)
				{
					this.isModified = value;
					this.OnIsModifiedChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnIsModifiedChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventIsModifiedChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsModifiedChanged
		{
			add
			{
				SynchronizedDelegate.Combine(base.Events, AbstractResultPane.EventIsModifiedChanged, value);
			}
			remove
			{
				SynchronizedDelegate.Remove(base.Events, AbstractResultPane.EventIsModifiedChanged, value);
			}
		}

		public CommandCollection ResultPaneCommands
		{
			get
			{
				return this.resultPaneCommands;
			}
		}

		public CommandCollection ExportListCommands
		{
			get
			{
				return this.exportListCommands;
			}
		}

		public CommandCollection ViewModeCommands
		{
			get
			{
				return this.viewModeCommands;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CommandCollection SelectionCommands
		{
			get
			{
				return this.selectionCommands;
			}
		}

		public Command RefreshCommand
		{
			get
			{
				return this.refreshCommand;
			}
		}

		private void RefreshCommand_Execute(object sender, EventArgs e)
		{
			if (this.RefreshableDataSource != null)
			{
				this.RefreshableDataSource.Refresh(base.CreateProgress(this.RefreshCommand.Text));
			}
		}

		[DefaultValue(null)]
		public virtual IRefreshableNotification RefreshableDataSource
		{
			get
			{
				return this.refreshableDataSource;
			}
			set
			{
				if (this.RefreshableDataSource != value)
				{
					this.OnRefreshableDataSourceChanging(EventArgs.Empty);
					if (this.RefreshableDataSource != null)
					{
						this.RefreshableDataSource.RefreshingChanged -= this.RefreshableDataSource_RefreshingChanged;
					}
					this.refreshableDataSource = value;
					if (this.RefreshableDataSource != null)
					{
						this.RefreshableDataSource.RefreshingChanged += this.RefreshableDataSource_RefreshingChanged;
						this.RefreshableDataSource_RefreshingChanged(this.RefreshableDataSource, EventArgs.Empty);
						if (this.RefreshableDataSource is DataTableLoader || this.RefreshableDataSource is AggregateDataTableLoadersRefreshableSource)
						{
							base.Components.Add(this.RefreshableDataSource as IComponent);
						}
					}
					else
					{
						this.RefreshCommand.Enabled = false;
					}
					this.OnRefreshableDataSourceChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnRefreshableDataSourceChanging(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventRefreshableDataSourceChanging];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler RefreshableDataSourceChanging
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventRefreshableDataSourceChanging, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventRefreshableDataSourceChanging, value);
			}
		}

		protected virtual void OnRefreshableDataSourceChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventRefreshableDataSourceChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler RefreshableDataSourceChanged
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventRefreshableDataSourceChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventRefreshableDataSourceChanged, value);
			}
		}

		private void RefreshableDataSource_RefreshingChanged(object sender, EventArgs e)
		{
			this.RefreshCommand.Enabled = !this.RefreshableDataSource.Refreshing;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract string SelectionHelpTopic { get; }

		public Command HelpCommand
		{
			get
			{
				return this.helpCommand;
			}
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			try
			{
				if (this.needSuppressHelp)
				{
					hevent.Handled = true;
				}
				else if (!hevent.Handled && !string.IsNullOrEmpty(this.SelectionHelpTopic))
				{
					hevent.Handled = true;
					ExchangeHelpService.ShowHelpFromHelpTopicId(this, this.SelectionHelpTopic);
				}
				base.OnHelpRequested(hevent);
			}
			finally
			{
				this.needSuppressHelp = false;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ContextMenu ContextMenu
		{
			get
			{
				return base.ContextMenu;
			}
			set
			{
				base.ContextMenu = value;
			}
		}

		public CommandCollection ContextMenuCommands
		{
			get
			{
				return this.contextMenuCommands;
			}
		}

		private void ContextMenu_Popup(object sender, EventArgs e)
		{
			this.UpdateContextMenu();
		}

		internal void UpdateContextMenu()
		{
			this.UpdateContextMenuCommands();
			this.ScanAllSeparators(this.ContextMenuCommands);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void UpdateContextMenuCommands()
		{
			this.ContextMenuCommands.Clear();
		}

		public bool IsActive
		{
			get
			{
				return this.isActive;
			}
			private set
			{
				this.isActive = value;
			}
		}

		protected virtual void DelayInitialize()
		{
			this.delayInitialized = true;
		}

		public void OnSetActive()
		{
			if (!this.delayInitialized)
			{
				this.DelayInitialize();
			}
			this.numberOfActivation++;
			this.OnSettingActive(EventArgs.Empty);
			this.IsActive = true;
			this.PublishSelection();
			this.OnSetActive(EventArgs.Empty);
		}

		public bool IsActivatedFirstly
		{
			get
			{
				return this.numberOfActivation == 1;
			}
		}

		protected virtual void OnSettingActive(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventSettingActive];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler SettingActive
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventSettingActive, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventSettingActive, value);
			}
		}

		protected virtual void OnSetActive(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventSetActive];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler SetActive
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventSetActive, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventSetActive, value);
			}
		}

		public void OnKillActive()
		{
			this.OnKillingActive(EventArgs.Empty);
			this.IsActive = false;
			this.OnKillActive(EventArgs.Empty);
		}

		protected virtual void OnKillingActive(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventKillingActive];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler KillingActive
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventKillingActive, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventKillingActive, value);
			}
		}

		protected virtual void OnKillActive(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventKillActive];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler KillActive
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventKillActive, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventKillActive, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(true)]
		public new bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (this.Enabled != value)
				{
					this.OnEnabledChanging(EventArgs.Empty);
					this.enabled = value;
					if (this.Enabled != base.Enabled)
					{
						base.Enabled = this.Enabled;
					}
				}
			}
		}

		protected virtual void OnEnabledChanging(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventEnabledChanging];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler EnabledChanging
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventEnabledChanging, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventEnabledChanging, value);
			}
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			if (this.Enabled != base.Enabled)
			{
				this.Enabled = base.Enabled;
			}
			base.OnEnabledChanged(e);
		}

		protected void UpdateSelection(ICollection selectedObjects, string displayName, DataObject dataObject)
		{
			this.UpdateSelection(selectedObjects, displayName, dataObject, string.Empty);
		}

		protected virtual void UpdateSelection(ICollection selectedObjects, string displayName, DataObject dataObject, string description)
		{
			this.selectionDataObject = ((dataObject == null) ? new DataObject() : dataObject);
			this.SelectedObjects = selectedObjects;
			this.selectionDataObject.SetText(displayName);
			this.SelectionDataObject.SetData("SelectedObjects", this.SelectedObjects);
			this.selectionDataObject.SetData("Description", description);
			if (this.IsActive)
			{
				this.PublishSelection();
			}
			this.OnSelectionChanged(EventArgs.Empty);
			this.UpdateContextMenu();
		}

		private void PublishSelection()
		{
			ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (selectionService != null)
			{
				selectionService.SetSelectedComponents(this.SelectedObjects);
			}
		}

		public ICollection SelectedObjects
		{
			get
			{
				return this.selectedObjects;
			}
			private set
			{
				if (value == null)
				{
					value = AbstractResultPane.emptySelectedObjects;
				}
				this.selectedObjects = value;
			}
		}

		public DataObject SelectionDataObject
		{
			get
			{
				return this.selectionDataObject;
			}
		}

		public virtual string SelectedName
		{
			get
			{
				return this.SelectionDataObject.GetText();
			}
		}

		public virtual string SelectedObjectDetailsType
		{
			get
			{
				return string.Empty;
			}
		}

		public bool HasSelection
		{
			get
			{
				return this.SelectedObjects.Count > 0;
			}
		}

		public bool HasSingleSelection
		{
			get
			{
				return this.SelectedObjects.Count == 1;
			}
		}

		public bool HasMultiSelection
		{
			get
			{
				return this.SelectedObjects.Count > 1;
			}
		}

		protected virtual void OnSelectionChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[AbstractResultPane.EventSelectionChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler SelectionChanged
		{
			add
			{
				base.Events.AddHandler(AbstractResultPane.EventSelectionChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(AbstractResultPane.EventSelectionChanged, value);
			}
		}

		private void ScanAllSeparators(CommandCollection commands)
		{
			bool flag = false;
			Command command = null;
			foreach (object obj in commands)
			{
				Command command2 = (Command)obj;
				if (flag)
				{
					if (command2.IsSeparator)
					{
						command2.Visible = true;
						command = command2;
						flag = false;
					}
				}
				else if (command2.IsSeparator)
				{
					command2.Visible = false;
				}
				else if (command2.Visible)
				{
					flag = true;
				}
			}
			if (!flag && command != null)
			{
				command.Visible = false;
			}
		}

		public override RightToLeft RightToLeft
		{
			get
			{
				if (!LayoutHelper.CultureInfoIsRightToLeft)
				{
					return base.RightToLeft;
				}
				return RightToLeft.Yes;
			}
			set
			{
			}
		}

		void IResultPaneControl.add_HelpRequested(HelpEventHandler A_1)
		{
			base.HelpRequested += A_1;
		}

		void IResultPaneControl.remove_HelpRequested(HelpEventHandler A_1)
		{
			base.HelpRequested -= A_1;
		}

		private const Icon DefaultIcon = null;

		private const string DefaultStatus = "";

		private ContextMenu contextMenuLocal;

		private ContainerResultPane containerResultPane;

		private ResultPane dependedResultPane;

		private Icon icon;

		private static readonly object EventIconChanged = new object();

		private bool needSuppressHelp;

		[AccessedThroughProperty("SharedSettingsBindingSource")]
		private BindingSource sharedSettingsBindingSource;

		private bool dataBoundToSettings;

		private ExchangeSettings sharedSettings;

		private ExchangeSettings privateSettings;

		private string status = "";

		private static readonly object EventStatusChanged = new object();

		private bool isModified;

		private static readonly object EventIsModifiedChanged = new object();

		private CommandCollection resultPaneCommands = new CommandCollection();

		private CommandCollection exportListCommands = new CommandCollection();

		private CommandCollection viewModeCommands = new CommandCollection();

		private CommandCollection selectionCommands = new CommandCollection();

		private Command refreshCommand = new Command();

		private IRefreshableNotification refreshableDataSource;

		private static readonly object EventRefreshableDataSourceChanging = new object();

		private static readonly object EventRefreshableDataSourceChanged = new object();

		private Command helpCommand = new Command();

		private CommandCollection contextMenuCommands = new CommandCollection();

		private bool isActive;

		private bool delayInitialized;

		private int numberOfActivation;

		private static readonly object EventSettingActive = new object();

		private static readonly object EventSetActive = new object();

		private static readonly object EventKillingActive = new object();

		private static readonly object EventKillActive = new object();

		private bool enabled = true;

		private static readonly object EventEnabledChanging = new object();

		private ICollection selectedObjects = AbstractResultPane.emptySelectedObjects;

		private static ICollection emptySelectedObjects = new object[0];

		private DataObject selectionDataObject = new DataObject();

		private static readonly object EventSelectionChanged = new object();
	}
}
