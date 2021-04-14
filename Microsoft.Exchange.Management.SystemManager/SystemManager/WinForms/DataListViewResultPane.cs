using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DataListViewResultPane : ResultPane, IExchangeFormOwner
	{
		public DataListViewResultPane() : this(null, null)
		{
		}

		public DataListViewResultPane(IResultsLoaderConfiguration config) : this((config != null) ? config.BuildResultsLoaderProfile() : null, null)
		{
		}

		public DataListViewResultPane(DataTableLoader loader) : this((loader != null) ? loader.ResultsLoaderProfile : null, loader)
		{
		}

		public DataListViewResultPane(ObjectPickerProfileLoader profileLoader, string profileName) : this(profileLoader.GetProfile(profileName))
		{
		}

		public DataListViewResultPane(ResultsLoaderProfile profile) : this(profile, null)
		{
		}

		protected DataListViewResultPane(ResultsLoaderProfile profile, DataTableLoader loader) : base(profile, loader)
		{
			this.listContextMenu = base.ContextMenu;
			base.ContextMenu = null;
			base.Name = "DataListViewResultPane";
			this.saveDefaultFilterCommand = new Command();
			this.saveDefaultFilterCommand.Description = LocalizedString.Empty;
			this.saveDefaultFilterCommand.Name = "commandSaveFilter";
			this.saveDefaultFilterCommand.Text = Strings.SaveAsDefaultFilterCommandText;
			this.saveDefaultFilterCommand.Execute += this.saveDefaultFilterCommand_Execute;
			base.ViewModeCommands.AddRange(new Command[]
			{
				CommandLoggingDialog.GetCommandLoggingCommand(),
				Command.CreateSeparator()
			});
			base.ViewModeCommands.Add(Theme.VisualEffectsCommands);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (this.ObjectList != null)
				{
					WinformsHelper.SetDataSource(this.ObjectList, null, null);
				}
				else if (this.ListControl != null)
				{
					WinformsHelper.SetDataSource(this.ListControl, null, null);
				}
				this.FilterControl = null;
				this.ListControl = null;
			}
		}

		protected override void OnSetActive(EventArgs e)
		{
			if (this.RefreshableDataSource == null && base.ResultsLoaderProfile != null)
			{
				this.RefreshableDataSource = new DataTableLoader(base.ResultsLoaderProfile);
			}
			if (base.DataTableLoader != null && this.ListControl.DataSource == null)
			{
				if (this.ObjectList != null)
				{
					WinformsHelper.SetDataSource(this.ObjectList, (base.ResultsLoaderProfile == null) ? null : base.ResultsLoaderProfile.UIPresentationProfile, base.DataTableLoader);
				}
				else if (this.ListControl != null)
				{
					WinformsHelper.SetDataSource(this.ListControl, (base.ResultsLoaderProfile == null) ? null : base.ResultsLoaderProfile.UIPresentationProfile, base.DataTableLoader);
				}
			}
			base.OnSetActive(e);
		}

		private ObjectList ObjectList
		{
			get
			{
				if (base.Controls.Count <= 0)
				{
					return null;
				}
				return base.Controls[0] as ObjectList;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataListView ListControl
		{
			get
			{
				return this.listControl;
			}
			set
			{
				if (this.listControl != null)
				{
					if (this.SaveSettings)
					{
						this.PrivateSettings.SaveDataListViewSettings(this.listControl);
					}
					this.listControl.ColumnWidthChanged -= new ColumnWidthChangedEventHandler(this.MakeModified);
					this.listControl.ColumnReordered -= new ColumnReorderedEventHandler(this.MakeModified);
					this.listControl.ColumnClick -= new ColumnClickEventHandler(this.MakeModified);
					base.Components.Remove(this.listControl);
					this.listControl.SelectionChanged -= this.OnListSelectionChanged;
					this.listControl.ItemsForRowsCreated -= this.listControl_ItemsForRowsCreated;
					this.listControl.ShowSelectionPropertiesCommand = null;
					this.listControl.RefreshCommand = null;
					this.listControl.DeleteSelectionCommand = null;
					this.listControl.ContextMenu = null;
					this.listControl.DataSourceRefresher = null;
					base.ViewModeCommands.Remove(this.listControl.ShowColumnPickerCommand);
				}
				this.listControl = value;
				if (this.listControl != null)
				{
					if (base.ResultsLoaderProfile != null)
					{
						this.ListControl.AutoGenerateColumns = base.ResultsLoaderProfile.AutoGenerateColumns;
						this.LoadAvailableColumns();
						this.ListControl.SortProperty = base.ResultsLoaderProfile.SortProperty;
						this.ListControl.ImagePropertyName = base.ResultsLoaderProfile.ImageProperty;
						this.ListControl.SelectionNameProperty = base.ResultsLoaderProfile.NameProperty;
						this.Text = base.ResultsLoaderProfile.DisplayName;
						this.ListControl.IdentityProperty = base.ResultsLoaderProfile.DistinguishIdentity;
						this.ListControl.MultiSelect = base.ResultsLoaderProfile.MultiSelect;
					}
					this.PrivateSettings.LoadDataListViewSettings(this.listControl);
					this.listControl.ColumnWidthChanged += new ColumnWidthChangedEventHandler(this.MakeModified);
					this.listControl.ColumnReordered += new ColumnReorderedEventHandler(this.MakeModified);
					this.listControl.ColumnClick += new ColumnClickEventHandler(this.MakeModified);
					base.Components.Add(this.listControl);
					this.listControl.SelectionChanged += this.OnListSelectionChanged;
					this.listControl.ItemsForRowsCreated += this.listControl_ItemsForRowsCreated;
					this.listControl.ShowSelectionPropertiesCommand = new Command();
					this.listControl.ShowSelectionPropertiesCommand.Execute += delegate(object param0, EventArgs param1)
					{
						base.InvokeCurrentShowSelectionPropertiesCommand();
					};
					this.listControl.RefreshCommand = base.RefreshCommand;
					this.listControl.DeleteSelectionCommand = new Command();
					this.listControl.DeleteSelectionCommand.Execute += delegate(object param0, EventArgs param1)
					{
						base.InvokeCurrentDeleteSelectionCommand();
					};
					this.listControl.ContextMenu = this.listContextMenu;
					this.listControl.DataSourceRefresher = this.RefreshableDataSource;
					this.listControl.VirtualMode = this.listControl.SupportsVirtualMode;
					if ("" == this.ListControl.IdentityProperty)
					{
						this.ListControl.IdentityProperty = "Identity";
					}
					if ("" == this.ListControl.SelectionNameProperty)
					{
						this.ListControl.SelectionNameProperty = "Name";
					}
					if ("" == this.ListControl.SortProperty)
					{
						this.ListControl.SortProperty = "Name";
					}
					this.listControl.NoResultsLabelText = Strings.NoItemsToShow;
					this.listControl.DrawLockedString = !this.HasPermission();
					this.listControl.DrawLockedIcon = this.listControl.DrawLockedString;
					base.ViewModeCommands.Insert(2, this.listControl.ShowColumnPickerCommand);
					this.InitializeExportListCommands();
				}
			}
		}

		protected virtual void LoadAvailableColumns()
		{
			if (!base.ResultsLoaderProfile.AutoGenerateColumns)
			{
				this.ListControl.AvailableColumns.Clear();
				this.ListControl.AvailableColumns.AddRange(base.ResultsLoaderProfile.CreateColumnHeaders());
			}
		}

		private void InitializeExportListCommands()
		{
			if (this.exportListCommandSeparator == null)
			{
				this.exportListCommandSeparator = Command.CreateSeparator();
			}
			if (this.exportListCommand == null)
			{
				this.exportListCommand = new Command();
				this.exportListCommand.Text = Strings.ExportListDefaultCommandText;
				this.exportListCommand.Icon = Icons.ExportList;
				this.exportListCommand.Name = "ExportListCommand";
				this.exportListCommand.Execute += this.ExportListCommand_Execute;
			}
			base.ExportListCommands.AddRange(new Command[]
			{
				this.exportListCommandSeparator,
				this.exportListCommand
			});
			if (this.ListControl is DataTreeListView)
			{
				if (this.expandAllCommand == null)
				{
					this.expandAllCommand = new Command();
					this.expandAllCommand.Text = Strings.ExpandAllDefaultCommandText;
					this.expandAllCommand.Name = "ExpandAllCommand";
					this.expandAllCommand.Icon = Icons.ExpandAll;
					this.expandAllCommand.Execute += delegate(object param0, EventArgs param1)
					{
						DataTreeListView dataTreeListView = this.ListControl as DataTreeListView;
						if (dataTreeListView != null)
						{
							dataTreeListView.ExpandAll();
						}
					};
				}
				base.ExportListCommands.Insert(base.ExportListCommands.IndexOf(this.exportListCommand), this.expandAllCommand);
				if (this.collapseRootsCommand == null)
				{
					this.collapseRootsCommand = new Command();
					this.collapseRootsCommand.Text = Strings.CollapseAllDefaultCommandText;
					this.collapseRootsCommand.Name = "CollapseAllCommand";
					this.collapseRootsCommand.Icon = Icons.CollapseAll;
					this.collapseRootsCommand.Execute += delegate(object param0, EventArgs param1)
					{
						DataTreeListView dataTreeListView = this.ListControl as DataTreeListView;
						if (dataTreeListView != null)
						{
							dataTreeListView.CollapseRootItems();
						}
					};
				}
				base.ExportListCommands.Insert(base.ExportListCommands.IndexOf(this.exportListCommand), this.collapseRootsCommand);
			}
		}

		protected void SetExpandAllCommandText(LocalizedString text)
		{
			this.expandAllCommand.Text = text;
		}

		protected void SetCollapseRootsCommandText(LocalizedString text)
		{
			this.collapseRootsCommand.Text = text;
		}

		public override string SelectedObjectDetailsType
		{
			get
			{
				if (this.ListControl == null)
				{
					return base.SelectedObjectDetailsType;
				}
				return this.ListControl.SelectedObjectDetailsType;
			}
		}

		public new IList SelectedObjects
		{
			get
			{
				if (this.ListControl == null)
				{
					return new object[0];
				}
				return this.ListControl.SelectedObjects;
			}
		}

		public new bool HasSelection
		{
			get
			{
				return this.SelectedObjects.Count > 0;
			}
		}

		public new bool HasSingleSelection
		{
			get
			{
				return this.SelectedObjects.Count == 1;
			}
		}

		public new bool HasMultiSelection
		{
			get
			{
				return this.SelectedObjects.Count > 1;
			}
		}

		public object SelectedObject
		{
			get
			{
				if (this.SelectedObjects.Count > 0)
				{
					return this.SelectedObjects[0];
				}
				return null;
			}
		}

		public DataRowView SelectedDataRowView
		{
			get
			{
				return this.SelectedObject as DataRowView;
			}
		}

		public DataRow SelectedDataRow
		{
			get
			{
				DataRowView selectedDataRowView = this.SelectedDataRowView;
				if (selectedDataRowView == null)
				{
					return null;
				}
				return selectedDataRowView.Row;
			}
		}

		public Icon SelectedIcon
		{
			get
			{
				if (this.ListControl == null)
				{
					return null;
				}
				return this.ListControl.SelectedItemIcon;
			}
		}

		public object SelectedIdentity
		{
			get
			{
				if (this.ListControl == null)
				{
					return null;
				}
				return this.ListControl.SelectedIdentity;
			}
		}

		public IList SelectedIdentities
		{
			get
			{
				if (this.ListControl == null)
				{
					return new object[0];
				}
				return this.ListControl.SelectedIdentities;
			}
		}

		internal WorkUnit[] GetSelectedWorkUnits()
		{
			return this.ListControl.GetSelectedWorkUnits();
		}

		protected virtual int MultiRowRefreshThreshold
		{
			get
			{
				return 5;
			}
		}

		public override IRefreshable GetSelectionRefreshObjects()
		{
			object[] array = this.SelectedIdentities.OfType<object>().ToArray<object>();
			if (array.Length <= this.MultiRowRefreshThreshold)
			{
				return new MultiRowRefreshObject(array, (ISupportFastRefresh)this.RefreshableDataSource);
			}
			return this.RefreshableDataSource;
		}

		private void listControl_ItemsForRowsCreated(object sender, EventArgs e)
		{
			int itemsCountDisplayedAsStatus = this.GetItemsCountDisplayedAsStatus();
			if (itemsCountDisplayedAsStatus == 1)
			{
				base.Status = Strings.ItemCountSingle;
				return;
			}
			base.Status = Strings.ItemCountPlural(itemsCountDisplayedAsStatus);
		}

		protected virtual int GetItemsCountDisplayedAsStatus()
		{
			return this.ListControl.TotalItemsCount;
		}

		private void MakeModified(object sender, EventArgs e)
		{
			base.IsModified = true;
		}

		public Command SaveDefaultFilterCommand
		{
			get
			{
				return this.saveDefaultFilterCommand;
			}
		}

		private void saveDefaultFilterCommand_Execute(object sender, EventArgs e)
		{
			if (this.FilterControl.IsApplied)
			{
				base.IsModified = true;
				this.PrivateSettings.FilterExpression = this.FilterControl.PersistedExpression;
				return;
			}
			base.ShellUI.ShowError(Strings.ErrorHitApplyBeforeSaveFilter);
		}

		[DefaultValue(null)]
		public FilterControl FilterControl
		{
			get
			{
				return this.filterControl;
			}
			set
			{
				if (this.FilterControl != value)
				{
					this.filterControl = value;
					if (this.FilterControl != null && base.ResultsLoaderProfile != null && base.ResultsLoaderProfile.UIPresentationProfile.FilterableProperties.Count > 0)
					{
						this.FilterControl.PropertiesToFilter.Clear();
						foreach (FilterablePropertyDescription item in base.ResultsLoaderProfile.UIPresentationProfile.FilterableProperties.Values)
						{
							this.FilterControl.PropertiesToFilter.Add(item);
						}
						this.FilterControl.ObjectSchema = base.ResultsLoaderProfile.FilterObjectSchema;
					}
					if (this.FilterControl == null)
					{
						if (base.ViewModeCommands.Contains(this.SaveDefaultFilterCommand))
						{
							base.ViewModeCommands.Remove(this.SaveDefaultFilterCommand);
						}
					}
					else
					{
						if (!base.ViewModeCommands.Contains(this.SaveDefaultFilterCommand))
						{
							base.ViewModeCommands.Add(this.SaveDefaultFilterCommand);
						}
						this.FilterControl.PersistedExpression = this.PrivateSettings.FilterExpression;
					}
					this.OnFilterControlChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnFilterControlChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[DataListViewResultPane.EventFilterControlChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler FilterControlChanged
		{
			add
			{
				base.Events.AddHandler(DataListViewResultPane.EventFilterControlChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataListViewResultPane.EventFilterControlChanged, value);
			}
		}

		private void OnListSelectionChanged(object sender, EventArgs e)
		{
			string text = this.ListControl.SelectionName;
			if (string.IsNullOrEmpty(text))
			{
				text = Strings.SelectionNameDoesNotExist(this.ListControl.Columns[this.ListControl.SelectionNameProperty].Text);
			}
			base.UpdateSelection(this.SelectedIdentities, text, null);
		}

		protected override void OnRefreshableDataSourceChanged(EventArgs e)
		{
			if (this.ListControl != null)
			{
				this.ListControl.DataSourceRefresher = this.RefreshableDataSource;
			}
			base.OnRefreshableDataSourceChanged(e);
		}

		protected override ExchangeSettings CreatePrivateSettings(IComponent owner)
		{
			return new DataListViewSettings(owner);
		}

		public new DataListViewSettings PrivateSettings
		{
			get
			{
				return (DataListViewSettings)base.PrivateSettings;
			}
		}

		public override void LoadComponentSettings()
		{
			base.LoadComponentSettings();
			if (this.ListControl != null)
			{
				this.PrivateSettings.LoadDataListViewSettings(this.ListControl);
			}
			if (this.FilterControl != null && this.PrivateSettings.FilterExpression != null)
			{
				this.FilterControl.PersistedExpression = this.PrivateSettings.FilterExpression;
			}
		}

		public override void ResetComponentSettings()
		{
			base.ResetComponentSettings();
			if (this.ListControl != null)
			{
				this.PrivateSettings.LoadDataListViewSettings(this.ListControl);
			}
		}

		public override void SaveComponentSettings()
		{
			if (this.ListControl != null)
			{
				this.PrivateSettings.SaveDataListViewSettings(this.ListControl);
			}
			base.SaveComponentSettings();
		}

		void IExchangeFormOwner.OnExchangeFormClosed(ExchangeForm formToClose)
		{
			WizardForm wizardForm = formToClose as WizardForm;
			if (wizardForm != null && wizardForm.WizardPages.Count > 0 && this.ListControl != null)
			{
				WizardPage wizardPage = wizardForm.WizardPages[wizardForm.WizardPages.Count - 1];
				if (wizardPage != null && wizardPage.Context != null && wizardPage.Context.DataHandler != null && wizardPage.Context.DataHandler.SavedResults != null && wizardPage.Context.DataHandler.SavedResults.Count == 1)
				{
					this.ListControl.SelectItemBySpecifiedIdentity(this.GetIdentityFromObject(wizardPage.Context.DataHandler.SavedResults[0]), false);
				}
			}
			if (formToClose != null)
			{
				this.childrenFormsList.Remove(formToClose);
			}
		}

		void IExchangeFormOwner.OnExchangeFormLoad(ExchangeForm form)
		{
			if (form != null)
			{
				this.childrenFormsList.Add(form);
			}
		}

		protected virtual object GetIdentityFromObject(object newObject)
		{
			object obj = null;
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(newObject);
			PropertyDescriptor propertyDescriptor = null;
			if (!string.IsNullOrEmpty(this.ListControl.IdentityProperty))
			{
				propertyDescriptor = properties[this.ListControl.IdentityProperty];
			}
			propertyDescriptor = (propertyDescriptor ?? properties["Identity"]);
			if (propertyDescriptor != null)
			{
				obj = propertyDescriptor.GetValue(newObject);
				ADObjectId adobjectId = obj as ADObjectId;
				if (adobjectId != null)
				{
					obj = new object[]
					{
						adobjectId,
						adobjectId.DistinguishedName,
						adobjectId.ToString(),
						adobjectId.ObjectGuid.ToString()
					};
				}
				else if (obj is Guid)
				{
					obj = ((Guid)obj).ToString();
				}
			}
			return obj;
		}

		protected virtual object GetObjectRefreshCategory(object obj)
		{
			return null;
		}

		public virtual void RefreshObject(object obj)
		{
			this.RefreshObjects(new object[]
			{
				obj
			});
		}

		public virtual void RefreshObjects(IList<object> list)
		{
			IProgress progress = base.CreateProgress(base.RefreshCommand.Text);
			IList<object> list2;
			IDictionary<object, IList<object>> dictionary = this.GroupObjectsByCategory(list, out list2);
			if (dictionary.Count > 0)
			{
				this.CreateRefreshableObjectForObjects(dictionary).Refresh(progress);
			}
			if (list2.Count > 0)
			{
				if (list2.Count <= this.MultiRowRefreshThreshold)
				{
					base.DataTableLoader.Refresh(progress, list2.ToArray<object>(), 0);
					return;
				}
				base.RefreshCommand.Invoke();
			}
		}

		public IRefreshable CreateRefreshableObjectForSelection(params object[] refreshCategories)
		{
			if (refreshCategories == null || refreshCategories.Length == 0)
			{
				throw new ArgumentNullException("refreshCategories");
			}
			object[] value = this.SelectedIdentities.Cast<object>().ToArray<object>();
			IDictionary<object, IList<object>> dictionary = new Dictionary<object, IList<object>>();
			foreach (object key in refreshCategories)
			{
				dictionary.Add(key, value);
			}
			return this.CreateRefreshableObjectForObjects(dictionary);
		}

		private IDictionary<object, IList<object>> GroupObjectsByCategory(IList<object> objects, out IList<object> noCategoryList)
		{
			Dictionary<object, IList<object>> dictionary = new Dictionary<object, IList<object>>();
			noCategoryList = new List<object>();
			foreach (object obj in objects)
			{
				object objectRefreshCategory = this.GetObjectRefreshCategory(obj);
				object obj2 = this.GetIdentityFromObject(obj);
				object[] array = obj2 as object[];
				if (array != null)
				{
					obj2 = array[0];
				}
				if (objectRefreshCategory == null)
				{
					noCategoryList.Add(obj2);
				}
				else if (dictionary.ContainsKey(objectRefreshCategory))
				{
					dictionary[objectRefreshCategory].Add(obj2);
				}
				else
				{
					dictionary[objectRefreshCategory] = new List<object>
					{
						obj2
					};
				}
			}
			return dictionary;
		}

		private IRefreshable CreateRefreshableObjectForObjects(IDictionary<object, IList<object>> categoryToIdentitiesMap)
		{
			IList<object> list = new List<object>();
			foreach (KeyValuePair<object, IList<object>> keyValuePair in categoryToIdentitiesMap)
			{
				list.Add((keyValuePair.Value.Count <= this.MultiRowRefreshThreshold) ? new MultiRowRefreshRequest(keyValuePair.Key, keyValuePair.Value.ToArray<object>()) : keyValuePair.Key);
			}
			return base.CreateRefreshableObject(list.ToArray<object>());
		}

		protected ICollection<ExchangeForm> ChildrenExchangeForms
		{
			get
			{
				return this.childrenFormsList.ToArray();
			}
		}

		private void ExportListCommand_Execute(object sender, EventArgs e)
		{
			if (this.ListControl != null)
			{
				WinformsHelper.ShowExportDialog(this, this.ListControl, base.ShellUI);
			}
		}

		private DataListView listControl;

		private Command saveDefaultFilterCommand;

		private ContextMenu listContextMenu;

		private Command exportListCommandSeparator;

		private Command exportListCommand;

		private Command expandAllCommand;

		private Command collapseRootsCommand;

		private FilterControl filterControl;

		private static readonly object EventFilterControlChanged = new object();

		private List<ExchangeForm> childrenFormsList = new List<ExchangeForm>();
	}
}
