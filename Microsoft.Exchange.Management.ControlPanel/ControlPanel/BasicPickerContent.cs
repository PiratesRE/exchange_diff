using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ParseChildren(true)]
	[ControlValueProperty("Values")]
	[ClientScriptResource("BasicPickerContent", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	[PersistChildren(true)]
	[ToolboxData("<{0}:BasicPickerContent runat=server></{0}:BasicPickerContent>")]
	public class BasicPickerContent : ScriptControlBase
	{
		public BasicPickerContent()
		{
			this.CssClass = "pickerContainer";
			this.pickerTextBox = new TextBox();
			this.pickerTextBox.ID = "pickerTextBox";
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.CreateContentPanel();
			this.CreateBottomPanel();
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl(HtmlTextWriterTag.Div.ToString());
			htmlGenericControl.Attributes["class"] = "PropertyDiv";
			htmlGenericControl.Controls.Add(this.contentPanel);
			htmlGenericControl.Controls.Add(this.bottomPanel);
			this.Controls.Add(htmlGenericControl);
		}

		private void CreateBottomPanel()
		{
			this.btnAddItem = new HtmlButton();
			this.btnAddItem.ID = "btnAddItem";
			this.btnAddItem.CausesValidation = false;
			this.btnAddItem.Attributes["type"] = "button";
			this.btnAddItem.Attributes["onClick"] = "javascript:return false;";
			this.btnAddItem.Attributes.Add("class", "selectbutton");
			this.btnAddItem.InnerText = Strings.PickerFormItemsButtonText;
			TableCell tableCell = new TableCell();
			tableCell.CssClass = "selectButtonCell";
			tableCell.Controls.Add(this.btnAddItem);
			this.wellControl = new WellControl();
			this.wellControl.ID = "wellControl";
			this.wellControl.DisplayProperty = this.NameProperty;
			this.wellControl.IdentityProperty = this.IdentityProperty;
			this.wellControl.CssClass = "wellControl";
			TableCell tableCell2 = new TableCell();
			tableCell2.Controls.Add(this.wellControl);
			TableRow tableRow = new TableRow();
			tableRow.Cells.Add(tableCell2);
			Table table = new Table();
			table.CssClass = "wellWrapperTable";
			table.CellSpacing = 0;
			table.CellPadding = 0;
			table.Rows.Add(tableRow);
			TableCell tableCell3 = new TableCell();
			tableCell3.CssClass = "wellContainerCell";
			tableCell3.Controls.Add(table);
			TableRow tableRow2 = new TableRow();
			tableRow2.Cells.Add(tableCell);
			tableRow2.Cells.Add(tableCell3);
			Table table2 = new Table();
			table2.Width = Unit.Percentage(100.0);
			table2.CellSpacing = 0;
			table2.CellPadding = 0;
			table2.Rows.Add(tableRow2);
			this.selectionPanel = new Panel();
			this.selectionPanel.ID = "selectionPanel";
			this.selectionPanel.CssClass = "selectionPanel";
			this.selectionPanel.Controls.Add(table2);
			this.bottomPanel = new Panel();
			this.bottomPanel.ID = "bottomPanel";
			this.bottomPanel.CssClass = "bottom";
			this.bottomPanel.Controls.Add(this.selectionPanel);
		}

		private void CreateContentPanel()
		{
			this.pickerTextBox.CssClass = "pickerTextBox";
			this.pickerTextBox.Style.Add(HtmlTextWriterStyle.MarginTop, "0px");
			TableCell tableCell = new TableCell();
			tableCell.CssClass = "pickerTextBoxTd";
			tableCell.Controls.Add(this.pickerTextBox);
			EncodingLabel child = Util.CreateHiddenForSRLabel(string.Empty, this.pickerTextBox.ID);
			tableCell.Controls.Add(child);
			TableRow tableRow = new TableRow();
			tableRow.Cells.Add(tableCell);
			Table table = new Table();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.CssClass = "pickerTextBoxContainer";
			table.Rows.Add(tableRow);
			this.contentPanel = new Panel();
			this.contentPanel.ID = "contentPanel";
			this.contentPanel.CssClass = "contentPanel";
			if (!string.IsNullOrWhiteSpace(this.Caption))
			{
				Label label = new Label();
				label.Text = this.Caption;
				label.CssClass = "detailsLabel";
				this.contentPanel.Controls.Add(label);
			}
			this.contentPanel.Controls.Add(table);
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		[DefaultValue("Error")]
		public string ErrorProperty
		{
			get
			{
				return this.errorProperty;
			}
			set
			{
				this.errorProperty = value;
			}
		}

		[DefaultValue("CanRetry")]
		public string CanRetryProperty
		{
			get
			{
				return this.canRetryProperty;
			}
			set
			{
				this.canRetryProperty = value;
			}
		}

		[DefaultValue("Warning")]
		public string WarningProperty
		{
			get
			{
				return this.warningProperty;
			}
			set
			{
				this.warningProperty = value;
			}
		}

		[DefaultValue("DisplayName")]
		public string NameProperty
		{
			get
			{
				return this.nameProperty;
			}
			set
			{
				this.nameProperty = value;
			}
		}

		[DefaultValue("Identity")]
		public string IdentityProperty
		{
			get
			{
				return this.identityProperty;
			}
			set
			{
				this.identityProperty = value;
			}
		}

		[DefaultValue("")]
		public string Caption
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

		public WebServiceReference ServiceUrl { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			BaseForm baseForm = this.Page as BaseForm;
			if (baseForm != null)
			{
				descriptor.AddComponentProperty("Form", "aspnetForm");
			}
			descriptor.AddElementProperty("PickerTextBox", this.pickerTextBox.ClientID, this);
			descriptor.AddElementProperty("AddButton", this.btnAddItem.ClientID, this);
			descriptor.AddElementProperty("BottomPanel", this.bottomPanel.ClientID, this);
			descriptor.AddComponentProperty("WellControl", this.wellControl.ClientID, this);
			descriptor.AddProperty("ServiceUrl", EcpUrl.ProcessUrl(this.ServiceUrl.ServiceUrl), true);
			descriptor.AddProperty("ErrorProperty", this.ErrorProperty, true);
			descriptor.AddProperty("CanRetryProperty", this.CanRetryProperty, true);
			descriptor.AddProperty("WarningProperty", this.WarningProperty, true);
			descriptor.AddProperty("SpriteSrc", Util.GetSpriteImageSrc(this));
		}

		public string WrapperControlID { get; set; }

		private Panel contentPanel;

		private TextBox pickerTextBox;

		private Panel bottomPanel;

		private Panel selectionPanel;

		private HtmlButton btnAddItem;

		private WellControl wellControl;

		private string nameProperty;

		private string identityProperty;

		private string caption;

		private string errorProperty = "Error";

		private string canRetryProperty = "CanRetry";

		private string warningProperty = "Warning";
	}
}
