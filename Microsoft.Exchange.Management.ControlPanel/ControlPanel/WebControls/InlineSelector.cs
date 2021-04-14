using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("InlineSelector", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:InlineSelector runat=server></{0}:InlineSelector>")]
	public class InlineSelector : ScriptControlBase
	{
		public InlineSelector() : base(HtmlTextWriterTag.Div)
		{
			this.browseButton = new IconButton();
			this.browseButton.ID = "browseButton";
			this.cancelButton = new HyperLink();
			this.cancelButton.ID = "clearButton";
			this.cancelImage = new CommandSprite();
			this.cancelImage.ID = "ImageClearButton";
			this.pickerTextBox = new TextBox();
			this.pickerTextBox.ID = "pickerTextBox";
			this.pickerTextBox.ReadOnly = true;
			this.BrowseButtonText = Strings.Browse;
		}

		[Localizable(true)]
		[DefaultValue(null)]
		[Editor("System.ComponentModel.Design.MultilineStringEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[Bindable(BindableSupport.Yes)]
		public string Text
		{
			get
			{
				if (!this.pickerTextBox.Text.IsNullOrBlank())
				{
					return this.pickerTextBox.Text;
				}
				return null;
			}
			set
			{
				if (!value.IsNullOrBlank())
				{
					this.pickerTextBox.Text = value;
					return;
				}
				this.pickerTextBox.Text = null;
			}
		}

		[Browsable(false)]
		public CommandSprite.SpriteId BrowseButtonImageId
		{
			get
			{
				return this.browseButton.ImageId;
			}
			set
			{
				this.browseButton.ImageId = value;
			}
		}

		[Browsable(false)]
		[Localizable(true)]
		public string BrowseButtonText
		{
			get
			{
				return this.browseButton.Text;
			}
			set
			{
				this.browseButton.Text = value;
			}
		}

		[DefaultValue("false")]
		public bool HideClearButton { get; set; }

		public void SetIndicatorVisible(bool showIndicator)
		{
			this.cancelButton.CssClass = (showIndicator ? string.Empty : "hidden");
			this.indCell.CssClass = this.indCell.CssClass + (showIndicator ? " EnabledPickerIndicatorTd" : string.Empty);
		}

		protected override void CreateChildControls()
		{
			Table table = new Table();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.CssClass = "pickerTextBoxContainer";
			table.Attributes.Add("role", "presentation");
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.CssClass = "pickerTextBoxTd";
			this.pickerTextBox.CssClass = "pickerTextBox";
			this.pickerTextBox.TabIndex = 0;
			this.pickerTextBox.ReadOnly = true;
			tableCell.Controls.Add(this.pickerTextBox);
			this.cancelButton.NavigateUrl = "#";
			this.cancelButton.Attributes.Add("onclick", "javascript:return false;");
			this.cancelImage.ImageId = CommandSprite.SpriteId.ClearDefault;
			this.cancelImage.AlternateText = Strings.ClearSelectionTooltip;
			this.cancelButton.ToolTip = Strings.ClearSelectionTooltip;
			this.cancelButton.Controls.Add(this.cancelImage);
			EncodingLabel encodingLabel = new EncodingLabel();
			encodingLabel.Text = "×";
			encodingLabel.CssClass = "pickerTextBoxImageAlter";
			this.cancelButton.Controls.Add(encodingLabel);
			this.indCell = new TableCell();
			this.indCell.ID = "indCell";
			this.indCell.CssClass = (this.HideClearButton ? "hidden" : "pickerTextBoxIndicatorTd");
			this.indCell.Controls.Add(this.cancelButton);
			tableRow.Cells.Add(tableCell);
			tableRow.Cells.Add(this.indCell);
			table.Rows.Add(tableRow);
			Table table2 = new Table();
			table2.CellPadding = 0;
			table2.CellSpacing = 1;
			table2.CssClass = "singleSelect";
			table2.Attributes.Add("role", "presentation");
			TableRow tableRow2 = new TableRow();
			TableCell tableCell2 = new TableCell();
			tableCell2.Controls.Add(table);
			tableRow2.Cells.Add(tableCell2);
			this.browseButton.CssClass = "pickerBrowseButton" + (Util.IsIE() ? " pickerBrowseButton-IE" : string.Empty);
			TableCell tableCell3 = new TableCell();
			tableCell3.Controls.Add(this.browseButton);
			tableRow2.Cells.Add(tableCell3);
			table2.Rows.Add(tableRow2);
			this.Controls.Add(table2);
			EncodingLabel child = Util.CreateHiddenForSRLabel(string.Empty, this.browseButton.ID);
			EncodingLabel child2 = Util.CreateHiddenForSRLabel(string.Empty, this.cancelButton.ID);
			this.browseButton.Controls.Add(child);
			this.cancelButton.Controls.Add(child2);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (!this.HideClearButton)
			{
				this.SetIndicatorVisible(!string.IsNullOrEmpty(this.Text) && this.Enabled);
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.pickerTextBox.Enabled = this.Enabled;
			this.browseButton.Disabled = !this.Enabled;
			base.Render(writer);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("BrowseButton", this.browseButton.ClientID);
			descriptor.AddElementProperty("IndicatorTd", this.indCell.ClientID);
			descriptor.AddElementProperty("Indicator", this.cancelButton.ClientID);
			descriptor.AddElementProperty("TextBox", this.pickerTextBox.ClientID);
			descriptor.AddProperty("HideClearButton", this.HideClearButton);
		}

		private TextBox pickerTextBox;

		private HyperLink cancelButton;

		private TableCell indCell;

		private CommandSprite cancelImage;

		private IconButton browseButton;
	}
}
