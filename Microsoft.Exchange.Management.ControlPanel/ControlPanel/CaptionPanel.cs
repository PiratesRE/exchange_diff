using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ToolboxData("<{0}:CaptionPanel runat=\"server\" />")]
	public class CaptionPanel : Panel, INamingContainer
	{
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string Text
		{
			get
			{
				return this.lblText.Text;
			}
			set
			{
				this.lblText.Text = value;
			}
		}

		public WebControl TextLabel
		{
			get
			{
				return this.lblText.TextContainer;
			}
		}

		[Category("Behavior")]
		[DefaultValue(EACHelpId.Default)]
		[Bindable(true)]
		public string HelpId
		{
			get
			{
				return this.helpControl.HelpId;
			}
			set
			{
				this.helpControl.HelpId = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(true)]
		[Category("Appearance")]
		public bool ShowHelp
		{
			get
			{
				return this.helpControl.ShowHelp;
			}
			set
			{
				this.helpControl.ShowHelp = value;
			}
		}

		public bool AddHelpButton { get; set; }

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(true)]
		public bool ShowCaption { get; set; }

		public CaptionPanel()
		{
			this.lblText = new EllipsisLabel();
			this.helpControl = new HelpControl();
			this.lblText.CssClass = "hdrTxt";
			this.lblText.ID = "lblText";
			this.lblText.TextContainer.Attributes["role"] = "header";
			this.helpControl.NeedPublishHelpLinkWhenHidden = true;
			this.AddHelpButton = true;
			this.ID = "CaptionPanel";
			this.CssClass = "capPane";
			this.ShowCaption = true;
		}

		protected override void CreateChildControls()
		{
			this.Controls.Clear();
			HtmlTable htmlTable = new HtmlTable();
			htmlTable.CellPadding = 0;
			htmlTable.CellSpacing = 0;
			htmlTable.Attributes.Add("class", "tb ");
			this.Controls.Add(htmlTable);
			HtmlTableRow htmlTableRow = new HtmlTableRow();
			htmlTable.Rows.Add(htmlTableRow);
			HtmlTableCell htmlTableCell = new HtmlTableCell();
			if (this.ShowCaption)
			{
				htmlTableCell.Controls.Add(this.lblText);
			}
			htmlTableCell.Attributes.Add("class", "captionTd");
			htmlTableRow.Cells.Add(htmlTableCell);
			if (this.AddHelpButton)
			{
				htmlTableCell = new HtmlTableCell();
				htmlTableCell.Controls.Add(this.helpControl);
				htmlTableCell.Attributes.Add("class", "helpTd");
				htmlTableRow.Cells.Add(htmlTableCell);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (string.IsNullOrEmpty(this.Text))
			{
				this.lblText.Attributes.Add("data-value", "{DefaultCaptionText, Mode=OneWay}");
				return;
			}
			if (this.Text.IsBindingExpression())
			{
				this.lblText.Attributes.Add("data-value", this.Text);
				this.Text = string.Empty;
			}
		}

		private EllipsisLabel lblText;

		private HelpControl helpControl;
	}
}
