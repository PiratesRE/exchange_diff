using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ControlValueProperty("Values")]
	[ParseChildren(true)]
	[PersistChildren(true)]
	[ToolboxData("<{0}:PickerContent runat=server></{0}:PickerContent>")]
	[ClientScriptResource("PickerContent", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	public class PickerContent : ScriptControlBase
	{
		public PickerContent()
		{
			this.CssClass = "pickerContainer";
			this.listView = new Microsoft.Exchange.Management.ControlPanel.WebControls.ListView();
			this.listView.ID = "pickerListView";
			this.listView.ShowHeader = true;
			this.selectionPanel = new Panel();
			this.selectionPanel.ID = "selectionPanel";
			this.NameProperty = "DisplayName";
		}

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BindingCollection FilterParameters
		{
			get
			{
				return this.filterParameters;
			}
		}

		private bool AllowTyping
		{
			get
			{
				if (this.allowTyping == null)
				{
					this.allowTyping = new bool?(HttpContext.Current.Request.QueryString["allowtyping"] == "t");
				}
				return this.allowTyping.Value;
			}
		}

		protected virtual void AddDetails()
		{
			this.contentPanel.Controls.Add(this.listView);
		}

		protected Panel ContentPanel
		{
			get
			{
				return this.contentPanel;
			}
		}

		protected bool IsMasterDetailed
		{
			get
			{
				return this.isMasterDetailed;
			}
			set
			{
				this.isMasterDetailed = value;
			}
		}

		public Microsoft.Exchange.Management.ControlPanel.WebControls.ListView ListView
		{
			get
			{
				return this.listView;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.CreateListSource();
			this.searchPanel = this.CreateSearchPanel();
			this.contentPanel = new Panel();
			this.contentPanel.ID = "pickerContentPanel";
			this.contentPanel.CssClass = "contentPanel";
			this.listView.ShowTitle = false;
			this.listView.DataSourceID = this.listSource.ID;
			this.listView.CssClass = "pickerListView";
			this.AddDetails();
			this.bottomPanel = new Panel();
			this.bottomPanel.ID = "bottom";
			this.bottomPanel.CssClass = "bottom";
			this.detailsPanel = new Panel();
			this.detailsPanel.ID = "detailsPanel";
			this.detailsPanel.CssClass = "detailsPanel";
			this.detailsCaption = new Literal();
			this.detailsCaption.ID = "detailsCaption";
			this.detailsCaption.Text = Strings.PickerFormDetailsText;
			this.detailsTextBox = new TextBox();
			this.detailsTextBox.ID = "detailsTextBox";
			this.detailsTextBox.ReadOnly = true;
			this.detailsTextBox.TextMode = TextBoxMode.MultiLine;
			this.detailsTextBox.Rows = 3;
			this.detailsTextBox.CssClass = "detailsTextBox";
			this.detailsPanel.Controls.Add(this.detailsCaption);
			this.detailsPanel.Controls.Add(this.detailsTextBox);
			this.selectionPanel.CssClass = "selectionPanel";
			this.btnAddItem = new HtmlButton();
			this.btnAddItem.ID = "btnAddItem";
			this.btnAddItem.CausesValidation = false;
			this.btnAddItem.Attributes["type"] = "button";
			this.btnAddItem.Attributes["onClick"] = "javascript:return false;";
			this.btnAddItem.Attributes.Add("class", "selectbutton");
			this.btnAddItem.InnerText = Strings.PickerFormItemsButtonText;
			this.wellControl = new WellControl();
			this.wellControl.ID = "wellControl";
			this.wellControl.DisplayProperty = ((this.listView.Columns.Count == 0) ? "DisplayName" : this.listView.NameProperty);
			this.wellControl.IdentityProperty = this.listView.IdentityProperty;
			this.wellControl.CssClass = "wellControl";
			Table table = new Table();
			table.Width = Unit.Percentage(100.0);
			table.CellSpacing = 0;
			table.CellPadding = 0;
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.CssClass = "selectButtonCell";
			tableCell.Controls.Add(this.btnAddItem);
			TableCell tableCell2 = new TableCell();
			tableCell2.CssClass = "wellContainerCell";
			Table table2 = new Table();
			table2.CssClass = "wellWrapperTable";
			table2.CellSpacing = 0;
			table2.CellPadding = 0;
			TableRow tableRow2 = new TableRow();
			TableCell tableCell3 = new TableCell();
			tableCell3.Controls.Add(this.wellControl);
			tableRow2.Cells.Add(tableCell3);
			table2.Rows.Add(tableRow2);
			tableCell2.Controls.Add(table2);
			tableRow.Cells.Add(tableCell);
			tableRow.Cells.Add(tableCell2);
			table.Rows.Add(tableRow);
			this.selectionPanel.Controls.Add(table);
			this.bottomPanel.Controls.Add(this.detailsPanel);
			this.bottomPanel.Controls.Add(this.selectionPanel);
			this.Controls.Add(this.searchPanel);
			this.Controls.Add(this.contentPanel);
			this.Controls.Add(this.bottomPanel);
		}

		private void CreateListSource()
		{
			if (this.ServiceUrl != null)
			{
				this.webServiceListSource = new WebServiceListSource();
				this.webServiceListSource.ID = "webServiceListSource";
				this.webServiceListSource.ServiceUrl = this.ServiceUrl;
				this.webServiceListSource.SupportAsyncGetList = this.SupportAsyncGetList;
				this.listSource = this.webServiceListSource;
			}
			else
			{
				this.listSource = new ListSource();
				this.listSource.ID = "listSource";
			}
			this.Controls.Add(this.listSource);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!this.HideSearchPanel && this.webServiceListSource == null)
			{
				throw new ArgumentException("Please specify a ServiceUrl for PickerContent or set HideSearchPanel to true");
			}
			if (this.webServiceListSource == null && this.FilterParameters != null && this.FilterParameters.Count > 0)
			{
				throw new ArgumentException("Please specify a ServiceUrl for PickerContent otherwise FilterParameters are not supported");
			}
			base.OnPreRender(e);
			this.detailsPanel.Visible = !string.IsNullOrEmpty(this.DetailsProperty);
			this.searchPanel.Visible = !this.HideSearchPanel;
			this.selectionPanel.Visible = this.ShowWellControl;
			if (!this.HideSearchPanel)
			{
				ComponentBinding componentBinding = new ComponentBinding(this.filterTextBox, "text");
				componentBinding.Name = "SearchText";
				this.webServiceListSource.FilterParameters.Add(componentBinding);
			}
			if (this.HasCustomizedFilter)
			{
				ClientControlBinding clientControlBinding = new ComponentBinding(this, "CustomizedFilters");
				clientControlBinding.Name = "CustomizedFilters";
				this.webServiceListSource.FilterParameters.Add(clientControlBinding);
			}
			foreach (Binding binding in this.FilterParameters)
			{
				QueryStringBinding queryStringBinding = binding as QueryStringBinding;
				if (queryStringBinding == null || queryStringBinding.HasValue || !queryStringBinding.Optional)
				{
					this.webServiceListSource.FilterParameters.Add(binding);
				}
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		private Panel CreateSearchPanel()
		{
			Panel panel = new Panel();
			panel.ID = "searchPanel";
			panel.CssClass = "searchPanel";
			Table table = new Table();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Width = Unit.Percentage(100.0);
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.CssClass = "pickerFilterCell";
			this.filterTextBox = new FilterTextBox();
			this.filterTextBox.ID = "txtSearch";
			tableCell.Controls.Add(this.filterTextBox);
			tableRow.Cells.Add(tableCell);
			table.Rows.Add(tableRow);
			panel.Controls.Add(table);
			return panel;
		}

		[MergableProperty(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue("")]
		public List<ColumnHeader> Columns
		{
			get
			{
				return this.listView.Columns;
			}
		}

		public PickerSelectionType SelectionMode
		{
			get
			{
				return this.selectionMode;
			}
			set
			{
				if (this.SelectionMode != value)
				{
					this.selectionMode = value;
					this.listView.ShowStatus = (this.SelectionMode == PickerSelectionType.Multiple || this.SupportAsyncGetList);
					this.listView.MultiSelect = (this.SelectionMode == PickerSelectionType.Multiple);
				}
			}
		}

		private bool ShowWellControl
		{
			get
			{
				return this.selectionMode == PickerSelectionType.Multiple || this.AllowTyping;
			}
		}

		[DefaultValue("DisplayName")]
		public string NameProperty
		{
			get
			{
				return this.listView.NameProperty;
			}
			set
			{
				this.listView.NameProperty = value;
			}
		}

		public string SortProperty
		{
			get
			{
				return this.listView.SortProperty;
			}
			set
			{
				this.listView.SortProperty = value;
			}
		}

		public bool ClientSort
		{
			get
			{
				return this.listView.ClientSort;
			}
			set
			{
				this.listView.ClientSort = value;
			}
		}

		[DefaultValue("false")]
		public bool ShowHeader
		{
			get
			{
				return this.listView.ShowHeader;
			}
			set
			{
				this.listView.ShowHeader = value;
			}
		}

		[DefaultValue("Identity")]
		public string IdentityProperty
		{
			get
			{
				return this.listView.IdentityProperty;
			}
			set
			{
				this.listView.IdentityProperty = value;
			}
		}

		[DefaultValue(null)]
		[Themeable(true)]
		public string SpriteProperty
		{
			get
			{
				return this.listView.SpriteProperty;
			}
			set
			{
				this.listView.SpriteProperty = value;
			}
		}

		[DefaultValue(null)]
		[Themeable(true)]
		public string SpriteAltTextProperty
		{
			get
			{
				return this.listView.SpriteAltTextProperty;
			}
			set
			{
				this.listView.SpriteAltTextProperty = value;
			}
		}

		public WebServiceReference ServiceUrl { get; set; }

		public int SearchBarMaxLength
		{
			get
			{
				return this.filterTextBox.MaxLength;
			}
			set
			{
				this.filterTextBox.MaxLength = value;
			}
		}

		[DefaultValue(false)]
		public bool HasCustomizedFilter { get; set; }

		[Category("Appearance")]
		[Bindable(true)]
		[DefaultValue(false)]
		public bool HideSearchPanel { get; set; }

		[DefaultValue(false)]
		public bool ReturnIdentities { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			BaseForm baseForm = this.Page as BaseForm;
			if (baseForm != null)
			{
				descriptor.AddComponentProperty("Form", "aspnetForm");
			}
			descriptor.AddComponentProperty("ListView", this.ListViewID, this);
			descriptor.AddElementProperty("DetailsTextBox", this.DetailsTextBoxID, this);
			descriptor.AddElementProperty("DetailsPanel", this.DetailsPanelID, this);
			descriptor.AddComponentProperty("ListSource", this.ListSourceID, this);
			descriptor.AddElementProperty("AddButton", this.AddButtonID, this);
			descriptor.AddElementProperty("WrapperControl", this.WrapperControlID, this);
			descriptor.AddElementProperty("SearchPanel", this.SearchPanelID, this);
			descriptor.AddElementProperty("ContentPanel", this.ContentPanelID, this);
			descriptor.AddElementProperty("BottomPanel", this.BottomPanelID, this);
			descriptor.AddProperty("IsMasterDetailed", this.IsMasterDetailed, true);
			descriptor.AddProperty("DetailsProperty", this.DetailsProperty, true);
			descriptor.AddProperty("ReturnIdentities", this.ReturnIdentities, true);
			descriptor.AddProperty("AllowTyping", this.AllowTyping, true);
			descriptor.AddProperty("SpriteSrc", Util.GetSpriteImageSrc(this));
			if (this.ShowWellControl)
			{
				descriptor.AddComponentProperty("WellControl", this.wellControl.ClientID);
			}
			if (this.filterTextBox != null && !this.HideSearchPanel)
			{
				descriptor.AddComponentProperty("FilterTextBox", this.filterTextBox.ClientID);
			}
		}

		public string ListViewID
		{
			get
			{
				return this.listView.ClientID;
			}
		}

		public string DetailsTextBoxID
		{
			get
			{
				return this.detailsTextBox.ClientID;
			}
		}

		public string DetailsPanelID
		{
			get
			{
				return this.detailsPanel.ClientID;
			}
		}

		public string ListSourceID
		{
			get
			{
				return this.listSource.ClientID;
			}
		}

		public string AddButtonID
		{
			get
			{
				return this.btnAddItem.ClientID;
			}
		}

		public string WrapperControlID { get; set; }

		public string SearchPanelID
		{
			get
			{
				return this.searchPanel.ClientID;
			}
		}

		public string ContentPanelID
		{
			get
			{
				return this.contentPanel.ClientID;
			}
		}

		public string BottomPanelID
		{
			get
			{
				return this.bottomPanel.ClientID;
			}
		}

		public string DetailsProperty { get; set; }

		[DefaultValue(false)]
		public bool SupportAsyncGetList { get; set; }

		private ListSource listSource;

		private WebServiceListSource webServiceListSource;

		private Panel searchPanel;

		private FilterTextBox filterTextBox;

		private Panel contentPanel;

		private bool isMasterDetailed;

		private Microsoft.Exchange.Management.ControlPanel.WebControls.ListView listView;

		private Panel bottomPanel;

		private Panel detailsPanel;

		private Literal detailsCaption;

		private TextBox detailsTextBox;

		private Panel selectionPanel;

		private HtmlButton btnAddItem;

		private WellControl wellControl;

		private BindingCollection filterParameters = new BindingCollection();

		private bool? allowTyping;

		private PickerSelectionType selectionMode = PickerSelectionType.Multiple;
	}
}
