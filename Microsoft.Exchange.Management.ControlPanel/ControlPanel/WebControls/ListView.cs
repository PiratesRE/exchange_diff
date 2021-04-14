using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("ListView", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class ListView : ScriptControlBase
	{
		public ListView()
		{
			this.Height = Unit.Percentage(100.0);
			this.Width = Unit.Percentage(100.0);
			this.toolbar = new ToolBar();
			this.toolbar.ID = "ToolBar";
			this.toolbar.CssClass = "ListViewToolBar";
			this.listSource = new WebServiceListSource();
			this.listSource.ID = "listSource";
			this.AllowSorting = true;
			this.EmptyDataText = Strings.ListViewEmptyDataText;
			this.MultiSelect = true;
			this.ShowHeader = true;
			this.ShowTitle = true;
			this.ShowToolBar = true;
			this.EnableColumnResize = true;
			this.IdentityProperty = "Identity";
			this.NameProperty = "Name";
			this.IsEditable = false;
			this.ProgressDelay = 0;
		}

		[DefaultValue(true)]
		public bool AllowSorting { get; set; }

		[DefaultValue("")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[MergableProperty(false)]
		public List<ListItem> Views
		{
			get
			{
				return this.views;
			}
			set
			{
				if (value != null)
				{
					this.views = (from x in value
					where x.Enabled && x.IsAccessibleToUser(this.Context.User)
					select x).ToList<ListItem>();
				}
			}
		}

		public string LocalSearchViewModel { get; set; }

		[DefaultValue(false)]
		public bool ShowSearchBarOnToolBar { get; set; }

		[DefaultValue(false)]
		public bool PreLoad { get; set; }

		[DefaultValue("")]
		[MergableProperty(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public List<ColumnHeader> Columns
		{
			get
			{
				return this.columns;
			}
		}

		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue(null)]
		public CommandCollection Commands
		{
			get
			{
				return this.toolbar.Commands;
			}
		}

		public string DataSourceID { get; set; }

		[DefaultValue(null)]
		public string RefreshCookieName
		{
			get
			{
				return this.listSource.RefreshCookieName;
			}
			set
			{
				this.listSource.RefreshCookieName = value;
			}
		}

		[DefaultValue(null)]
		protected BindingCollection FilterParameters
		{
			get
			{
				return this.listSource.FilterParameters;
			}
		}

		public WebServiceReference ServiceUrl
		{
			get
			{
				return this.listSource.ServiceUrl;
			}
			set
			{
				this.listSource.ServiceUrl = value;
				this.DataSourceID = this.listSource.ID;
			}
		}

		public bool SupportAsyncGetList
		{
			get
			{
				return this.listSource.SupportAsyncGetList;
			}
			set
			{
				this.listSource.SupportAsyncGetList = value;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] RefreshAfter
		{
			get
			{
				return this.listSource.RefreshAfter;
			}
			set
			{
				this.listSource.RefreshAfter = value;
			}
		}

		[Localizable(true)]
		public string EmptyDataText { get; set; }

		public int ProgressDelay { get; set; }

		public string OnClientItemActivated { get; set; }

		public string OnClientItemUpdated { get; set; }

		public string OnClientSelectionChanged { get; set; }

		[DefaultValue(true)]
		public bool MultiSelect { get; set; }

		[DefaultValue(false)]
		public virtual bool ShowHeader { get; set; }

		public int Features
		{
			get
			{
				ListViewFlags listViewFlags = (ListViewFlags)0;
				if (this.ShowTitle)
				{
					listViewFlags |= ListViewFlags.Title;
				}
				if (this.Views.Count > 0)
				{
					listViewFlags |= ListViewFlags.ViewsPanel;
				}
				if (this.ShowSearchBar)
				{
					listViewFlags |= ListViewFlags.SearchBar;
				}
				if (this.ShowStatus)
				{
					listViewFlags |= ListViewFlags.Status;
				}
				if (this.IsEditable)
				{
					listViewFlags |= ListViewFlags.IsEditable;
				}
				if (this.AllowSorting)
				{
					listViewFlags |= ListViewFlags.AllowSorting;
				}
				if (this.ShowHeader)
				{
					listViewFlags |= ListViewFlags.ShowHeader;
				}
				if (this.MultiSelect)
				{
					listViewFlags |= ListViewFlags.MultiSelect;
				}
				if (this.EnableColumnResize)
				{
					listViewFlags |= ListViewFlags.EnableColumnResize;
				}
				return (int)listViewFlags;
			}
		}

		[DefaultValue(true)]
		public bool ShowToolBar { get; set; }

		public virtual bool ShowStatus
		{
			get
			{
				if (this.showStatus == null)
				{
					return this.showStatusDefault;
				}
				return this.showStatus.Value;
			}
			set
			{
				this.showStatus = new bool?(value);
			}
		}

		[DefaultValue(true)]
		public bool ShowTitle { get; set; }

		[DefaultValue(false)]
		public bool ShowSearchBar { get; set; }

		[DefaultValue(256)]
		public int SearchBarMaxLength
		{
			get
			{
				return this.SearchTextBox.MaxLength;
			}
			set
			{
				this.SearchTextBox.MaxLength = value;
			}
		}

		[Localizable(true)]
		public string SearchButtonToolTip
		{
			get
			{
				return this.SearchTextBox.SearchButtonToolTip;
			}
			set
			{
				this.SearchTextBox.SearchButtonToolTip = value;
			}
		}

		[Localizable(true)]
		public string SearchTextBoxWatermarkText
		{
			get
			{
				return this.SearchTextBox.WatermarkText;
			}
			set
			{
				this.SearchTextBox.WatermarkText = value;
			}
		}

		[DefaultValue(false)]
		public bool EnableAutoSuggestion
		{
			get
			{
				return this.SearchTextBox.EnableAutoSuggestion;
			}
			set
			{
				this.SearchTextBox.EnableAutoSuggestion = value;
			}
		}

		[DefaultValue("GetSuggestion")]
		public string AutoSuggestionServiceWorkflow
		{
			get
			{
				return this.SearchTextBox.SuggestionServiceWorkFlow;
			}
			set
			{
				this.SearchTextBox.SuggestionServiceWorkFlow = value;
			}
		}

		[DefaultValue("GetList")]
		public string AutoSuggestionServiceMethod
		{
			get
			{
				return this.SearchTextBox.SuggestionServiceMethod;
			}
			set
			{
				this.SearchTextBox.SuggestionServiceMethod = value;
			}
		}

		public string AutoSuggestionPropertyNames
		{
			get
			{
				return this.SearchTextBox.AutoSuggestionPropertyNames;
			}
			set
			{
				this.SearchTextBox.AutoSuggestionPropertyNames = value;
			}
		}

		public string AutoSuggestionPropertyValues
		{
			get
			{
				return this.SearchTextBox.AutoSuggestionPropertyValues;
			}
			set
			{
				this.SearchTextBox.AutoSuggestionPropertyValues = value;
			}
		}

		public SortDirection SortDirection { get; set; }

		public string SortProperty { get; set; }

		public bool ClientSort
		{
			get
			{
				return this.listSource.ClientSort;
			}
			set
			{
				this.listSource.ClientSort = value;
			}
		}

		[DefaultValue(null)]
		public string CaptionText { get; set; }

		[DefaultValue(typeof(Unit), "100%")]
		public override Unit Height
		{
			get
			{
				return base.Height;
			}
			set
			{
				base.Height = value;
			}
		}

		public string IdentityProperty { get; set; }

		[DefaultValue(null)]
		public string DefaultSprite { get; set; }

		[DefaultValue(null)]
		public string SpriteProperty { get; set; }

		[DefaultValue(null)]
		public string SpriteAltTextProperty { get; set; }

		public string NameProperty { get; set; }

		[DefaultValue(false)]
		[Browsable(false)]
		public bool IsEditable { get; set; }

		[DefaultValue(false)]
		public bool WarningAsError { get; set; }

		[Browsable(false)]
		public int InlineEditMaxLength { get; set; }

		public string InputWaterMarkText { get; set; }

		[DefaultValue(typeof(Unit), "100%")]
		public override Unit Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
			}
		}

		[DefaultValue(true)]
		public bool EnableColumnResize { get; set; }

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		public string SearchTextBoxToolTip
		{
			get
			{
				return this.SearchTextBox.ToolTip;
			}
			set
			{
				this.SearchTextBox.ToolTip = value;
			}
		}

		internal FilterTextBox SearchTextBox
		{
			get
			{
				if (this.searchTextBox == null)
				{
					this.searchTextBox = new FilterTextBox();
					this.searchTextBox.ID = "SearchBox";
				}
				return this.searchTextBox;
			}
		}

		private FilterDropDown ViewFilterDropDown
		{
			get
			{
				if (this.viewFilterDropDown == null)
				{
					this.viewFilterDropDown = new FilterDropDown();
					this.viewFilterDropDown.ID = "ViewFilterDropDown";
				}
				return this.viewFilterDropDown;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.showStatus == null && this.Page != null)
			{
				EcpContentPage ecpContentPage = this.Page as EcpContentPage;
				if (ecpContentPage != null && ecpContentPage.FeatureSet == FeatureSet.Options)
				{
					this.showStatusDefault = false;
				}
			}
			if (!string.IsNullOrEmpty(this.DataSourceID))
			{
				WebServiceListSource webServiceListSource = this.FindControl(this.DataSourceID) as WebServiceListSource;
				if (webServiceListSource != null)
				{
					this.listSource = webServiceListSource;
					this.Commands.Add(new RefreshCommand());
				}
			}
			this.toolbar.ApplyRolesFilter();
			if (this.Commands.Count == 0 && this.Controls.Contains(this.toolbar))
			{
				this.Controls.Remove(this.toolbar);
			}
			this.UpdateColumns();
			if (this.Views.Count > 0)
			{
				ComponentBinding componentBinding = new ComponentBinding(this.viewFilterDropDown, "filterValue");
				componentBinding.Name = "SelectedView";
				this.listSource.FilterParameters.Add(componentBinding);
			}
			if (this.ShowSearchBar)
			{
				if (string.IsNullOrEmpty(this.LocalSearchViewModel))
				{
					ComponentBinding componentBinding2 = new ComponentBinding(this.searchTextBox, "filterText");
					componentBinding2.Name = "SearchText";
					this.listSource.FilterParameters.Add(componentBinding2);
				}
				else
				{
					ComponentBinding componentBinding3 = new ComponentBinding(this.searchTextBox, "advancedSearch");
					componentBinding3.Name = "SearchText";
					this.listSource.FilterParameters.Add(componentBinding3);
				}
			}
			this.ClientSort |= this.SupportAsyncGetList;
			if (this.ShowHeader && !this.ClientSort)
			{
				ComponentBinding componentBinding4 = new ComponentBinding(this, "SortDirection");
				componentBinding4.Name = "Direction";
				this.listSource.SortParameters.Add(componentBinding4);
				ComponentBinding componentBinding5 = new ComponentBinding(this, "SortProperty");
				componentBinding5.Name = "PropertyName";
				this.listSource.SortParameters.Add(componentBinding5);
			}
			this.listSource.UpdateParameters();
		}

		private void UpdateColumns()
		{
			if (!string.IsNullOrEmpty(this.DefaultSprite) || !string.IsNullOrEmpty(this.SpriteProperty))
			{
				this.Columns.Insert(0, new SpriteColumnHeader
				{
					DefaultSprite = this.DefaultSprite,
					Name = this.SpriteProperty,
					AlternateTextProperty = this.SpriteAltTextProperty
				});
			}
			if (this.AllowSorting && this.SortProperty == null)
			{
				foreach (ColumnHeader columnHeader in this.Columns)
				{
					if (!string.IsNullOrEmpty(columnHeader.SortExpression) && !(columnHeader is SpriteColumnHeader))
					{
						this.SortProperty = columnHeader.SortExpression;
						break;
					}
				}
			}
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			this.Columns.RemoveAll((ColumnHeader x) => !string.IsNullOrEmpty(x.Role) && !rbacPrincipal.IsInRole(x.Role));
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.ServiceUrl != null)
			{
				this.SearchTextBox.SuggestionServicePath = this.ServiceUrl.ServiceUrl;
			}
			if (!string.IsNullOrEmpty(this.LocalSearchViewModel))
			{
				this.SearchTextBox.Attributes.Add("data-type", this.LocalSearchViewModel);
				this.SearchTextBox.Attributes.Add("data-control", "FilterTextBox");
				this.SearchTextBox.Attributes.Add("data-text", "{Text}");
				this.SearchTextBox.Attributes.Add("data-advancedSearch", "{AdvancedSearch, Mode=OneWay}");
				this.SearchTextBox.Attributes.Add("vm-SimpleSearchFields", base.Attributes["vm-SimpleSearchFields"]);
			}
			if (this.PreLoad)
			{
				HttpContext httpContext = HttpContext.Current;
				httpContext.Items.Add("getlistasync", "1");
				this.preLoadResults = this.ServiceUrl.GetList(new DDIParameters(), new SortOptions());
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.toolbarPanel != null)
			{
				this.toolbarPanel.Style[HtmlTextWriterStyle.Display] = ((this.ShowToolBar && this.Commands.ContainsVisibleCommands()) ? string.Empty : "none");
			}
			base.Render(writer);
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "hidden");
			string cssClass = this.CssClass;
			if (string.IsNullOrEmpty(cssClass))
			{
				this.CssClass = "ListView";
			}
			else
			{
				this.CssClass += " ListView";
			}
			base.AddAttributesToRender(writer);
			this.CssClass = cssClass;
		}

		protected override void CreateChildControls()
		{
			bool hasViewsPanel = false;
			if (this.Views.Count > 0)
			{
				hasViewsPanel = true;
				WebControl child = this.CreateViewsPanel();
				this.Controls.Add(child);
			}
			WebControl webControl = null;
			if (this.ShowSearchBar)
			{
				webControl = this.CreateSearchBar();
				if (this.ShowSearchBarOnToolBar)
				{
					InlineSearchBarCommand inlineSearchBarCommand = new InlineSearchBarCommand();
					inlineSearchBarCommand.ImageAltText = Strings.SearchCommandText;
					inlineSearchBarCommand.ImageId = CommandSprite.SpriteId.SearchDefault;
					webControl.ClientIDMode = ClientIDMode.Static;
					inlineSearchBarCommand.ControlIdToMove = webControl.ClientID;
					inlineSearchBarCommand.MovedControlCss = "ToolBarSearchBar";
					this.Commands.Add(inlineSearchBarCommand);
				}
			}
			if (this.Commands.Count > 0 && this.ShowToolBar)
			{
				this.toolbarPanel = (Panel)this.CreateToolBarControl(hasViewsPanel);
				this.Controls.Add(this.toolbarPanel);
			}
			if (webControl != null)
			{
				this.Controls.Add(webControl);
			}
			if (this.IsEditable)
			{
				this.listViewInputPanel = this.CreateInputBar();
				this.Controls.Add(this.listViewInputPanel);
			}
			if (!this.AllowSorting)
			{
				foreach (ColumnHeader columnHeader in this.Columns)
				{
					columnHeader.IsSortable = false;
				}
			}
			if (this.ServiceUrl != null)
			{
				this.Controls.Add(this.listSource);
			}
			base.Attributes.Add("role", "application");
			base.CreateChildControls();
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("DataSource", this.DataSourceID, this);
			descriptor.AddEvent("itemActivated", this.OnClientItemActivated, true);
			descriptor.AddEvent("itemUpdated", this.OnClientItemUpdated, true);
			descriptor.AddEvent("selectionChanged", this.OnClientSelectionChanged, true);
			descriptor.AddProperty("EmptyDataText", this.EmptyDataText);
			descriptor.AddProperty("CaptionText", this.CaptionText, true);
			if (this.ProgressDelay != 0)
			{
				descriptor.AddProperty("ProgressDelay", this.ProgressDelay);
			}
			if (this.Features != 0)
			{
				descriptor.AddProperty("Features", this.Features);
			}
			if (this.SortDirection != SortDirection.Ascending)
			{
				descriptor.AddProperty("SortDirection", this.SortDirection);
			}
			descriptor.AddProperty("SortProperty", this.SortProperty, true);
			if (this.IdentityProperty != "Identity")
			{
				descriptor.AddProperty("IdentityProperty", this.IdentityProperty);
			}
			if (this.PreLoad)
			{
				descriptor.AddProperty("PreLoadResultsString", this.preLoadResults.ToJsonString(null));
			}
			if (this.NameProperty != "Name")
			{
				descriptor.AddProperty("NameProperty", this.NameProperty);
			}
			if (this.InlineEditMaxLength != 128)
			{
				descriptor.AddProperty("InlineEditMaxLength", this.InlineEditMaxLength);
			}
			if (this.SupportAsyncGetList)
			{
				descriptor.AddProperty("PageSize", ListView.pageSize, 500);
			}
			else
			{
				descriptor.AddProperty("PageSize", ListView.pageSizeWithNoPaging, 3000);
			}
			descriptor.AddProperty("PageSizes", ListView.pageSizes, true);
			StringBuilder stringBuilder = new StringBuilder("[");
			stringBuilder.Append(string.Join(",", from o in this.Columns
			select o.ToJavaScript()));
			stringBuilder.Append("]");
			descriptor.AddScriptProperty("Columns", stringBuilder.ToString());
			if (this.toolbarPanel != null && this.ShowToolBar)
			{
				descriptor.AddComponentProperty("ToolBar", this.toolbar.ClientID);
			}
			if (this.viewFilterDropDown != null)
			{
				descriptor.AddComponentProperty("ViewFilterDropDown", this.viewFilterDropDown.ClientID);
			}
			if (this.searchTextBox != null && this.ShowSearchBar)
			{
				descriptor.AddComponentProperty("SearchTextBox", this.searchTextBox.ClientID);
			}
			if (this.listViewInputPanel != null && this.IsEditable)
			{
				descriptor.AddComponentProperty("InputTextBox", this.listViewInputPanel.ClientID);
			}
		}

		private WebControl CreateToolBarControl(bool hasViewsPanel)
		{
			return new Panel
			{
				CssClass = (hasViewsPanel ? "ToolBarContainer WithViewsPanel" : "ToolBarContainer"),
				Controls = 
				{
					this.toolbar
				}
			};
		}

		private WebControl CreateViewsPanel()
		{
			Panel panel = new Panel();
			panel.CssClass = "ViewsPanel";
			panel.ID = "ViewsPanel";
			this.ViewFilterDropDown.Items.AddRange(this.Views.ToArray());
			string value = this.Page.Request.QueryString["vw"];
			if (!string.IsNullOrEmpty(value))
			{
				ListItem listItem = this.ViewFilterDropDown.Items.FindByValue(value);
				if (listItem != null)
				{
					this.ViewFilterDropDown.ClearSelection();
					listItem.Selected = true;
				}
			}
			this.ViewFilterDropDown.Width = Unit.Percentage(100.0);
			Panel panel2 = new Panel();
			panel2.Controls.Add(this.ViewFilterDropDown);
			panel.Controls.Add(panel2);
			return panel;
		}

		private WebControl CreateSearchBar()
		{
			Panel panel = new Panel();
			panel.CssClass = "SearchBar";
			panel.ID = "SearchBar";
			panel.Attributes.Add("role", "search");
			this.SearchTextBox.Width = Unit.Percentage(100.0);
			Panel panel2 = new Panel();
			panel2.Controls.Add(this.SearchTextBox);
			panel.Controls.Add(panel2);
			if (this.ShowSearchBarOnToolBar)
			{
				panel.Style.Add(HtmlTextWriterStyle.Display, "none");
			}
			return panel;
		}

		private Panel CreateInputBar()
		{
			return new ListViewInputPanel
			{
				ID = "InputBar",
				MaxLength = this.InlineEditMaxLength,
				WatermarkText = this.InputWaterMarkText
			};
		}

		public const string RefreshCommandName = "Refresh";

		private const int DefaultPageSize = 500;

		private const int DefaultSizeForNoPagingListView = 3000;

		private static readonly object EventSelectionChanged = new object();

		private static readonly int pageSize = ConfigUtil.ReadInt("ListViewPageSize", 500);

		private static readonly string pageSizes = ConfigurationManager.AppSettings["ListViewPageSizes"];

		private static readonly int pageSizeWithNoPaging = ConfigUtil.ReadInt("ListViewSizeForNoPaging", 3000);

		private bool? showStatus;

		private bool showStatusDefault = true;

		private ToolBar toolbar;

		private Panel toolbarPanel;

		private Panel listViewInputPanel;

		private List<ListItem> views = new List<ListItem>();

		private List<ColumnHeader> columns = new List<ColumnHeader>();

		private WebServiceListSource listSource;

		private FilterTextBox searchTextBox;

		private FilterDropDown viewFilterDropDown;

		private PowerShellResults preLoadResults;
	}
}
