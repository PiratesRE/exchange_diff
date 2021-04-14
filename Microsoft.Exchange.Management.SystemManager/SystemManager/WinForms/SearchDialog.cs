using System;
using System.ComponentModel;
using System.Drawing;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract partial class SearchDialog : ExchangeForm
	{
		protected SearchDialog()
		{
			this.InitializeComponent();
			this.okButton.Text = Strings.Ok;
			this.cancelButton.Text = Strings.Cancel;
			this.helpButton.Text = Strings.Help;
			base.Icon = Icons.ObjectPicker;
			this.fileToolStripMenuItem.Text = Strings.ObjectPickerFile;
			this.viewToolStripMenuItem.Text = Strings.ObjectPickerView;
			this.closeToolStripMenuItem.Text = Strings.ObjectPickerClose;
			this.scopeToolStripMenuItem.Text = Strings.ObjectPickerScope;
			this.modifyRecipientPickerScopeToolStripMenuItem.Text = Strings.ObjectPickerModifyRecipientPickerScope;
			this.modifyExpectedResultSizeMenuItem.Text = Strings.SearchDialogModifyExpectedResultSize;
			this.toolStripLabelForName.Text = Strings.ObjectPickerSearch;
			this.findNowCommand = new Command();
			this.findNowCommand.Text = Strings.ObjectPickerFindNow;
			this.findNowCommand.Description = Strings.ObjectPickerFindNowDescription;
			this.findNowCommand.Name = this.toolStripButtonFindNow.Name;
			this.toolStripButtonFindNow.Command = this.findNowCommand;
			this.findNowCommand.Execute += delegate(object param0, EventArgs param1)
			{
				this.FindNow();
			};
			this.clearCommand = new Command();
			this.clearCommand.Text = Strings.ObjectPickerClear;
			this.clearCommand.Description = Strings.ObjectPickerClearDescription;
			this.clearCommand.Name = this.toolStripButtonClearOrStop.Name;
			this.clearCommand.Execute += delegate(object param0, EventArgs param1)
			{
				this.OnClear();
			};
			this.stopCommand = new Command();
			this.stopCommand.Text = Strings.ObjectPickerStop;
			this.stopCommand.Description = Strings.ObjectPickerStopDescription;
			this.stopCommand.Name = this.toolStripButtonClearOrStop.Name;
			this.stopCommand.Execute += this.stopCommand_Execute;
			this.enableColumnFiltferingToolStripMenuItem.Text = Strings.ObjectPickerEnableColumnFiltering;
			this.enableColumnFiltferingToolStripMenuItem.ToolTipText = string.Empty;
			this.resultListViewRefreshCommand = new Command();
			this.resultListViewRefreshCommand.Name = "resultListViewRefreshCommand";
			this.resultListViewRefreshCommand.Execute += delegate(object param0, EventArgs param1)
			{
				this.OnResultListViewRefresh();
			};
			this.UpdateControls(false);
			this.toolStripTextBoxForName.KeyPress += delegate(object sender, KeyPressEventArgs e)
			{
				if (e.KeyChar == '\r')
				{
					e.Handled = true;
					this.FindNowCommand.Invoke();
				}
			};
			this.closeToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				base.DialogResult = DialogResult.Cancel;
			};
			this.modifyRecipientPickerScopeToolStripMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				this.OnModifyRecipientPickerScopeToolStripMenuItemClicked();
			};
			this.modifyExpectedResultSizeMenuItem.Click += delegate(object param0, EventArgs param1)
			{
				this.OnModifyExpectedResultSizeMenuItemClicked();
			};
			this.helpButton.Visible = false;
		}

		protected virtual string ValidatingSearchText(string searchText)
		{
			string result = null;
			if (this.IsANRSearch() && SearchDialog.SearchTextConstraint != null)
			{
				if (searchText.Length < SearchDialog.SearchTextConstraint.MinLength)
				{
					result = Strings.SearchDialogSearchTextTooShort(SearchDialog.SearchTextConstraint.MinLength);
				}
				if (searchText.Length > SearchDialog.SearchTextConstraint.MaxLength)
				{
					result = Strings.SearchDialogSearchTextTooLong(SearchDialog.SearchTextConstraint.MaxLength);
				}
			}
			return result;
		}

		public static ValidateLengthAttribute SearchTextConstraint
		{
			get
			{
				if (SearchDialog.searchTextConstraint == null)
				{
					SearchDialog.searchTextConstraint = new ValidateLengthAttribute(3, 5120);
				}
				return SearchDialog.searchTextConstraint;
			}
		}

		protected abstract bool IsANRSearch();

		protected abstract void OnFindNow();

		protected abstract void OnClear();

		protected abstract void OnStop();

		protected abstract void OnResultListViewRefresh();

		protected void FindNow()
		{
			string text = this.ValidatingSearchText(this.SearchTextboxText.Trim());
			if (text != null)
			{
				this.ShowHintOnSearchTextBox(SearchDialog.SmallErrorIcon, Strings.SearchDialogInvalidSearchString, text);
				return;
			}
			this.OnFindNow();
		}

		protected abstract void OnModifyRecipientPickerScopeToolStripMenuItemClicked();

		protected abstract void OnModifyExpectedResultSizeMenuItemClicked();

		protected abstract void PerformQuery(object rootId, string searchText);

		protected Panel ListControlPanel
		{
			get
			{
				return this.listControlPanel;
			}
			set
			{
				this.listControlPanel = value;
			}
		}

		protected DataListView ResultListView
		{
			get
			{
				return this.resultListView;
			}
			set
			{
				if (this.ResultListView != null)
				{
					this.ResultListView.ItemsForRowsCreated -= this.resultListView_ItemsForRowsCreated;
					this.ResultListView.SelectionChanged -= this.resultListView_SelectionChanged;
					this.ResultListView.RefreshCommand = null;
				}
				this.resultListView = value;
				if (this.ResultListView != null)
				{
					this.enableColumnFiltferingToolStripMenuItem.Command = this.resultListView.ShowFilterCommand;
					this.enableColumnFiltferingToolStripMenuItem.ToolTipText = string.Empty;
					this.addRemoveColumnsToolStripMenuItem.Command = this.ResultListView.ShowColumnPickerCommand;
					this.addRemoveColumnsToolStripMenuItem.ToolTipText = string.Empty;
					this.ResultListView.ItemsForRowsCreated += this.resultListView_ItemsForRowsCreated;
					this.ResultListView.SelectionChanged += this.resultListView_SelectionChanged;
					this.ResultListView.RefreshCommand = this.resultListViewRefreshCommand;
				}
			}
		}

		protected DataTableLoader DataTableLoader
		{
			get
			{
				return this.dataTableLoader;
			}
			set
			{
				if (this.DataTableLoader != null)
				{
					this.DataTableLoader.RefreshingChanged -= this.DataTableLoader_RefreshingChanged;
					this.DataTableLoader.RefreshCompleted -= this.DataTableLoader_RefreshCompleted;
					this.DataTableLoader.ProgressChanged -= this.DataTableLoader_ProgressChanged;
				}
				this.dataTableLoader = value;
				if (this.DataTableLoader != null)
				{
					this.DataTableLoader.RefreshingChanged += this.DataTableLoader_RefreshingChanged;
					this.DataTableLoader.RefreshCompleted += this.DataTableLoader_RefreshCompleted;
					this.DataTableLoader.ProgressChanged += this.DataTableLoader_ProgressChanged;
				}
			}
		}

		[DefaultValue(null)]
		protected ScopeSettings ScopeSettings
		{
			get
			{
				if (this.scopeSettings == null)
				{
					this.scopeSettings = new ScopeSettings(ADServerSettingsSingleton.GetInstance().ADServerSettings);
				}
				return this.scopeSettings;
			}
			set
			{
				this.scopeSettings = value;
			}
		}

		protected string Caption
		{
			get
			{
				return this.caption;
			}
			set
			{
				this.caption = value;
			}
		}

		private protected string SearchString
		{
			protected get
			{
				return this.searchString;
			}
			private set
			{
				this.searchString = value;
			}
		}

		protected string SearchTextboxText
		{
			get
			{
				return this.toolStripTextBoxForName.Text;
			}
			set
			{
				this.toolStripTextBoxForName.Text = value;
			}
		}

		protected Command FindNowCommand
		{
			get
			{
				return this.findNowCommand;
			}
		}

		protected bool SupportSearch
		{
			get
			{
				return this.toolStrip.Visible;
			}
			set
			{
				this.toolStrip.Visible = value;
			}
		}

		protected bool SupportModifyScope
		{
			get
			{
				return this.scopeToolStripMenuItem.Visible;
			}
			set
			{
				this.scopeToolStripMenuItem.Visible = value;
			}
		}

		protected bool SupportModifyResultSize
		{
			get
			{
				return this.viewToolStripMenuItem.DropDownItems.Contains(this.modifyExpectedResultSizeMenuItem);
			}
			set
			{
				if (this.SupportModifyResultSize != value)
				{
					if (value)
					{
						this.viewToolStripMenuItem.DropDownItems.Add(this.modifyExpectedResultSizeMenuItem);
						return;
					}
					this.viewToolStripMenuItem.DropDownItems.Remove(this.modifyExpectedResultSizeMenuItem);
				}
			}
		}

		protected bool ShowDialogButtons
		{
			get
			{
				return this.dialogButtonsPanel.Visible;
			}
			set
			{
				this.dialogButtonsPanel.Visible = value;
			}
		}

		protected bool ShowStatus
		{
			get
			{
				return this.loadStatusLabel.Visible;
			}
			set
			{
				this.loadStatusLabel.Visible = value;
				this.selectedCountLabel.Visible = value;
			}
		}

		protected string ModifyScopeMenuText
		{
			get
			{
				return this.modifyRecipientPickerScopeToolStripMenuItem.Text;
			}
			set
			{
				this.modifyRecipientPickerScopeToolStripMenuItem.Text = value;
			}
		}

		public bool DisableClearButtonForEmptySearchText
		{
			get
			{
				return this.disableClearButtonForEmptySearchText;
			}
			set
			{
				this.disableClearButtonForEmptySearchText = value;
			}
		}

		[DefaultValue(false)]
		public bool HelpVisible
		{
			get
			{
				return this.helpButton.Visible;
			}
			set
			{
				this.helpButton.Visible = value;
			}
		}

		protected void PerformQueryForCurrentSearchString()
		{
			this.lastSearchIsCancelled = false;
			object rootId = (this.SupportModifyScope && this.ScopeSettings.DomainViewEnabled) ? this.ScopeSettings.OrganizationalUnit : null;
			this.PerformQuery(rootId, this.SearchString);
		}

		protected void StartNewSearch()
		{
			this.SearchString = this.toolStripTextBoxForName.Text.Trim();
			if (this.ResultListView != null)
			{
				this.ResultListView.Focus();
			}
			this.PerformQueryForCurrentSearchString();
		}

		protected bool PromptToModifyRecipientScope(IUIService uiService, ExchangePropertyPageControl scopeControl)
		{
			if (scopeControl == null)
			{
				throw new ArgumentNullException("scopeControl");
			}
			using (PropertyPageDialog propertyPageDialog = new PropertyPageDialog(scopeControl))
			{
				ScopeSettings scopeSettings = new ScopeSettings();
				scopeSettings.CopyFrom(this.ScopeSettings);
				scopeControl.Context = new DataContext(new ExchangeDataHandler());
				scopeControl.Context.DataHandler.DataSource = scopeSettings;
				if (uiService.ShowDialog(propertyPageDialog) == DialogResult.OK && scopeSettings.ObjectState == ObjectState.Changed)
				{
					this.ScopeSettings.CopyChangesFrom(scopeSettings);
					this.UpdateText();
					return true;
				}
			}
			return false;
		}

		protected void ShowHintOnSearchTextBox(Icon icon, string title, string text)
		{
			NativeMethods.EDITBALLOONTIP editballoontip = new NativeMethods.EDITBALLOONTIP(icon, title, text);
			Control control = this.toolStripTextBoxForName.Control;
			UnsafeNativeMethods.SendMessage(new HandleRef(control, control.Handle), 5379, (IntPtr)0, ref editballoontip);
			GC.KeepAlive(editballoontip);
		}

		protected void UpdateStatusLabelText()
		{
			string text = Strings.QueryCanceled;
			if (this.lastSearchIsCancelled && this.DataTableLoader.Table.Rows.Count == 0)
			{
				this.loadStatusLabel.Text = text;
				return;
			}
			this.loadStatusLabel.Text = (string.IsNullOrEmpty(this.ResultListView.BindingListViewFilter) ? Strings.ObjectsFound(this.DataTableLoader.Table.Rows.Count) : Strings.ObjectsFoundAndFiltered(this.ResultListView.Items.Count, this.DataTableLoader.Table.Rows.Count - this.ResultListView.Items.Count));
			if (this.lastSearchIsCancelled)
			{
				ToolStripStatusLabel toolStripStatusLabel = this.loadStatusLabel;
				toolStripStatusLabel.Text = toolStripStatusLabel.Text + " " + text;
			}
		}

		protected void ActiveSearchTextbox()
		{
			this.toolStripTextBoxForName.Focus();
		}

		protected void RemoveColumnFilteringMenu()
		{
			this.viewToolStripMenuItem.DropDownItems.Remove(this.enableColumnFiltferingToolStripMenuItem);
		}

		private void stopCommand_Execute(object sender, EventArgs e)
		{
			this.OnStop();
		}

		private void DataTableLoader_ProgressChanged(object sender, RefreshProgressChangedEventArgs e)
		{
			this.UpdateStatusLabelText();
		}

		private void DataTableLoader_RefreshingChanged(object sender, EventArgs e)
		{
			this.UpdateControls(this.DataTableLoader.Refreshing);
		}

		private void DataTableLoader_RefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!this.DataTableLoader.Refreshing)
			{
				if (e.Error == null && e.Cancelled)
				{
					this.lastSearchIsCancelled = true;
					this.UpdateStatusLabelText();
				}
				if (this.ResultListView.SelectedIndices.Count == 0 && this.ResultListView.Items.Count > 0)
				{
					this.ResultListView.Items[0].Selected = true;
					this.ResultListView.Items[0].Focused = true;
				}
			}
		}

		private void UpdateControls(bool isSearching)
		{
			this.loadProgressBar.Visible = isSearching;
			if (isSearching)
			{
				this.loadStatusLabel.Text = Strings.Searching;
				this.toolStripButtonClearOrStop.Command = this.stopCommand;
				return;
			}
			this.toolStripButtonClearOrStop.Command = this.clearCommand;
			if (this.DisableClearButtonForEmptySearchText)
			{
				this.clearCommand.Enabled = !string.IsNullOrEmpty(this.toolStripTextBoxForName.Text);
			}
		}

		private void UpdateText()
		{
			this.Text = (this.SupportModifyScope ? Strings.ObjectPickerFormTextWithScope(this.Caption, this.ScopeSettings.ScopingDescription) : this.Caption);
		}

		private void resultListView_SelectionChanged(object sender, EventArgs e)
		{
			this.okButton.Enabled = (this.ResultListView.SelectedIndices.Count > 0);
			this.selectedCountLabel.Text = Strings.ObjectsSelected(this.ResultListView.SelectedIndices.Count);
		}

		private void resultListView_ItemsForRowsCreated(object sender, EventArgs e)
		{
			this.UpdateStatusLabelText();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			this.UpdateText();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			this.stopCommand.Invoke();
			base.OnClosing(e);
		}

		private Command findNowCommand;

		private Command clearCommand;

		private Command stopCommand;

		private Command resultListViewRefreshCommand;

		private bool lastSearchIsCancelled;

		private static ValidateLengthAttribute searchTextConstraint;

		private static readonly Icon SmallErrorIcon = new Icon(Icons.Error, new Size(16, 16));

		private DataListView resultListView;

		private DataTableLoader dataTableLoader;

		private ScopeSettings scopeSettings;

		private string caption;

		private string searchString;

		private bool disableClearButtonForEmptySearchText;
	}
}
