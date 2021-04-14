using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed partial class ObjectPickerForm : SearchDialog, ISelectedObjectsProvider
	{
		public ObjectPickerForm()
		{
			base.Name = "ObjectPickerForm";
			base.SupportModifyResultSize = false;
			DataListView dataListView = new DataListView();
			dataListView.AutoGenerateColumns = false;
			dataListView.Cursor = Cursors.Default;
			dataListView.Dock = DockStyle.Fill;
			dataListView.Location = new Point(0, 0);
			dataListView.Name = "resultListView";
			dataListView.Size = new Size(498, 333);
			dataListView.TabIndex = 2;
			dataListView.VirtualMode = true;
			dataListView.ItemActivate += this.resultListView_ItemActivate;
			base.ListControlPanel.Controls.Add(dataListView);
			base.ResultListView = dataListView;
		}

		public ObjectPickerForm(ObjectPicker objectPicker) : this()
		{
			this.ObjectPicker = objectPicker;
		}

		protected override void OnFindNow()
		{
			base.StartNewSearch();
		}

		protected override string ValidatingSearchText(string searchText)
		{
			if (!string.IsNullOrEmpty(searchText))
			{
				return base.ValidatingSearchText(searchText);
			}
			return null;
		}

		protected override void OnStop()
		{
			base.DataTableLoader.CancelRefresh();
		}

		protected override void OnClear()
		{
			base.SearchTextboxText = string.Empty;
			base.ResultListView.RefreshCommand.Invoke();
		}

		protected override void PerformQuery(object rootId, string searchText)
		{
			this.ObjectPicker.PerformQuery(rootId, searchText);
		}

		protected override bool IsANRSearch()
		{
			if (this.ObjectPicker.ObjectPickerProfile != null)
			{
				foreach (AbstractDataTableFiller abstractDataTableFiller in this.ObjectPicker.ObjectPickerProfile.TableFillers)
				{
					if (abstractDataTableFiller.CommandBuilder.SearchType == null)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		protected override void OnResultListViewRefresh()
		{
			base.FindNow();
		}

		protected override void OnModifyRecipientPickerScopeToolStripMenuItemClicked()
		{
			using (ModifyScopeSettingsControl modifyScopeSettingsControl = new ModifyScopeSettingsControl(this.ObjectPicker))
			{
				if (base.PromptToModifyRecipientScope(base.ShellUI, modifyScopeSettingsControl))
				{
					base.ResultListView.RefreshCommand.Invoke();
				}
			}
		}

		protected override void OnModifyExpectedResultSizeMenuItemClicked()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		[DefaultValue(null)]
		public ObjectPicker ObjectPicker
		{
			get
			{
				return this.objectPicker;
			}
			set
			{
				if (this.ObjectPicker != value)
				{
					if (this.ObjectPicker != null)
					{
						if (base.DataTableLoader != null)
						{
							base.DataTableLoader.RefreshCompleted -= this.DataTableLoader_RefreshCompleted;
							base.DataTableLoader = null;
						}
						WinformsHelper.SetDataSource(base.ResultListView, null, null);
						base.ResultListView.DataSourceRefresher = null;
						this.resultsDataTable = null;
						this.CleanupColumns();
						base.Caption = null;
						base.ScopeSettings = null;
						base.ResultListView.SortProperty = null;
						base.ResultListView.IconLibrary = null;
						base.ResultListView.ImagePropertyName = null;
					}
					this.objectPicker = value;
					if (this.ObjectPicker != null)
					{
						base.Caption = this.ObjectPicker.Caption;
						base.ScopeSettings = this.ObjectPicker.ScopeSettings;
						base.ResultListView.MultiSelect = this.ObjectPicker.AllowMultiSelect;
						base.ResultListView.NoResultsLabelText = this.ObjectPicker.NoResultsLabelText;
						base.ResultListView.SelectionNameProperty = this.ObjectPicker.NameProperty;
						if (string.IsNullOrEmpty(this.ObjectPicker.IdentityProperty))
						{
							base.ResultListView.IdentityProperty = "Identity";
						}
						else
						{
							base.ResultListView.IdentityProperty = this.ObjectPicker.IdentityProperty;
						}
						if (!string.IsNullOrEmpty(this.ObjectPicker.DefaultSortProperty))
						{
							base.ResultListView.SortProperty = this.ObjectPicker.DefaultSortProperty;
						}
						if (this.ObjectPicker.ShowListItemIcon)
						{
							base.ResultListView.IconLibrary = ObjectPicker.ObjectClassIconLibrary;
							base.ResultListView.ImagePropertyName = this.ObjectPicker.ImageProperty;
						}
						base.SupportSearch = this.ObjectPicker.SupportSearch;
						base.SupportModifyScope = this.ObjectPicker.SupportModifyScope;
						base.DisableClearButtonForEmptySearchText = true;
						this.resultsDataTable = this.ObjectPicker.DataTableLoader.Table;
						this.SetupColumns();
						WinformsHelper.SetDataSource(base.ResultListView, (this.ObjectPicker.ObjectPickerProfile == null) ? null : this.ObjectPicker.ObjectPickerProfile.UIPresentationProfile, this.ObjectPicker.DataTableLoader);
						base.ResultListView.DataSourceRefresher = this.ObjectPicker.DataTableLoader;
						base.DataTableLoader = this.ObjectPicker.DataTableLoader;
						base.DataTableLoader.RefreshCompleted += this.DataTableLoader_RefreshCompleted;
					}
				}
			}
		}

		private void DataTableLoader_RefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (ExceptionHelper.IsUICriticalException(e.Error))
			{
				throw new ObjectPickerException(e.Error.Message, e.Error);
			}
			if (!this.ObjectPicker.DataTableLoader.Refreshing && e.Error != null)
			{
				string message;
				if (e.Error is SearchTooLargeException)
				{
					message = Strings.SearchTooLargeRefineYourSearch(e.Error.Message);
				}
				else if (ExceptionHelper.IsWellknownExceptionFromServer(e.Error.InnerException))
				{
					message = e.Error.InnerException.Message;
				}
				else
				{
					message = e.Error.Message;
				}
				base.ShowError(message);
			}
		}

		private void SetupColumns()
		{
			this.columnHeaders = this.ObjectPicker.CreateColumnHeaders();
			if (this.ObjectPicker.ObjectPickerProfile == null)
			{
				DataColumnCollection columns = this.ObjectPicker.DataTableLoader.Table.Columns;
				this.dataColumnDictionary = new Dictionary<string, DataColumn>(columns.Count);
				foreach (object obj in columns)
				{
					DataColumn dataColumn = (DataColumn)obj;
					this.dataColumnDictionary.Add(dataColumn.ColumnName, dataColumn);
				}
				foreach (ExchangeColumnHeader exchangeColumnHeader in this.columnHeaders)
				{
					exchangeColumnHeader.VisibleChanged += this.header_VisibleChanged;
					this.header_VisibleChanged(exchangeColumnHeader, EventArgs.Empty);
				}
				base.ResultListView.ColumnsChanged += delegate(object param0, EventArgs param1)
				{
					base.ResultListView.RefreshCommand.Invoke();
				};
			}
			base.ResultListView.Columns.Clear();
			base.ResultListView.AvailableColumns.AddRange(this.columnHeaders);
		}

		private void CleanupColumns()
		{
			if (this.columnHeaders != null)
			{
				foreach (ExchangeColumnHeader exchangeColumnHeader in this.columnHeaders)
				{
					exchangeColumnHeader.VisibleChanged -= this.header_VisibleChanged;
				}
			}
			this.columnHeaders = null;
			this.dataColumnDictionary = null;
		}

		private void header_VisibleChanged(object sender, EventArgs e)
		{
			ExchangeColumnHeader exchangeColumnHeader = (ExchangeColumnHeader)sender;
			ExTraceGlobals.ProgramFlowTracer.TraceDebug<ObjectPickerForm, string, bool>(0L, "-->ObjectPickerForm.header_VisibleChanged:{0}. Column name = {1}, Column visible = {2}).", this, exchangeColumnHeader.Name, exchangeColumnHeader.Visible);
			if (exchangeColumnHeader.Visible && !this.resultsDataTable.Columns.Contains(exchangeColumnHeader.Name) && this.dataColumnDictionary.ContainsKey(exchangeColumnHeader.Name))
			{
				this.resultsDataTable.Columns.Add(this.dataColumnDictionary[exchangeColumnHeader.Name]);
				ExTraceGlobals.DataFlowTracer.Information(0L, "*--ObjectPickerForm.header_VisibleChanged:{0}. Data column {1} is added, column type:{2}, column expression:{3}.", new object[]
				{
					this,
					this.dataColumnDictionary[exchangeColumnHeader.Name].ColumnName,
					this.dataColumnDictionary[exchangeColumnHeader.Name].DataType,
					this.dataColumnDictionary[exchangeColumnHeader.Name].Expression
				});
			}
			if (!exchangeColumnHeader.Visible && (string.IsNullOrEmpty(base.ResultListView.SortProperty) || base.ResultListView.SortProperty != exchangeColumnHeader.Name) && this.resultsDataTable.Columns.Contains(exchangeColumnHeader.Name))
			{
				DataColumn column = this.resultsDataTable.Columns[exchangeColumnHeader.Name];
				if (!ObjectPicker.GetIsRequiredDataColumnFlag(column))
				{
					this.resultsDataTable.Columns.Remove(exchangeColumnHeader.Name);
					ExTraceGlobals.DataFlowTracer.Information<ObjectPickerForm, string>(0L, "*--ObjectPickerForm.header_VisibleChanged:{0}. Data column {1} is removed.", this, exchangeColumnHeader.Name);
				}
			}
			ExTraceGlobals.ProgramFlowTracer.TraceDebug<ObjectPickerForm>(0L, "<--ObjectPickerForm.header_VisibleChanged:{0}.", this);
		}

		public DataTable SelectedObjects
		{
			get
			{
				return this.selectedObjects;
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			this.selectedObjects = this.resultsDataTable.Clone();
			foreach (object obj in base.ResultListView.SelectedObjects)
			{
				DataRowView dataRowView = (DataRowView)obj;
				this.selectedObjects.Rows.Add(dataRowView.Row.ItemArray);
			}
			this.resultsDataTable.Clear();
			if (this.ObjectPicker.ObjectPickerProfile == null)
			{
				using (Dictionary<string, DataColumn>.ValueCollection.Enumerator enumerator2 = this.dataColumnDictionary.Values.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						DataColumn dataColumn = enumerator2.Current;
						if (!this.resultsDataTable.Columns.Contains(dataColumn.ColumnName))
						{
							this.resultsDataTable.Columns.Add(dataColumn);
						}
					}
					return;
				}
			}
			this.ObjectPicker.ObjectPickerProfile.SearchText = string.Empty;
			this.ObjectPicker.ObjectPickerProfile.Scope = null;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			base.FindNowCommand.Invoke();
		}

		private void resultListView_ItemActivate(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
		}

		protected override void OnHelpRequested(HelpEventArgs hevent)
		{
			if (!hevent.Handled)
			{
				ExchangeHelpService.ShowHelpFromHelpTopicId(this, this.objectPicker.HelpTopic);
				hevent.Handled = true;
			}
			base.OnHelpRequested(hevent);
		}

		private ObjectPicker objectPicker;

		private DataTable resultsDataTable;

		private DataTable selectedObjects;

		private ExchangeColumnHeader[] columnHeaders;

		private Dictionary<string, DataColumn> dataColumnDictionary;
	}
}
