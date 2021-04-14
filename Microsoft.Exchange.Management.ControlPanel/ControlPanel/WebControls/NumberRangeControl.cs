using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:NumberRangeControl runat=server></{0}:NumberRangeControl>")]
	[ClientScriptResource("NumberRangeControl", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class NumberRangeControl : ScriptControlBase
	{
		public NumberRangeControl() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "valueRangePicker";
		}

		public string AtLeastTextboxID
		{
			get
			{
				this.EnsureChildControls();
				return this.tbxAtLeast.ClientID;
			}
		}

		public string AtMostTextboxID
		{
			get
			{
				this.EnsureChildControls();
				return this.tbxAtMost.ClientID;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.tbxAtLeast = new TextBox();
			this.tbxAtLeast.ID = "tbxAtLeast";
			this.tbxAtMost = new TextBox();
			this.tbxAtMost.ID = "tbxAtMost";
			Label label = new Label();
			Label label2 = new Label();
			label.ID = this.tbxAtLeast.ID + "_label";
			label2.ID = this.tbxAtMost.ID + "_label";
			label.Text = Strings.AtLeast;
			label2.Text = Strings.AtMost;
			HtmlTable htmlTable = new HtmlTable();
			HtmlTableRow htmlTableRow = new HtmlTableRow();
			HtmlTableRow htmlTableRow2 = new HtmlTableRow();
			HtmlTableCell htmlTableCell = new HtmlTableCell();
			HtmlTableCell htmlTableCell2 = new HtmlTableCell();
			HtmlTableCell htmlTableCell3 = new HtmlTableCell();
			HtmlTableCell htmlTableCell4 = new HtmlTableCell();
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl("div");
			HtmlGenericControl htmlGenericControl2 = new HtmlGenericControl("div");
			htmlTable.CellPadding = (htmlTable.CellSpacing = 0);
			htmlTableCell2.Attributes.Add("class", "labelCell");
			htmlTableCell.Attributes.Add("class", "labelCell");
			htmlTableCell4.Attributes.Add("class", "inputCell");
			htmlTableCell3.Attributes.Add("class", "inputCell");
			htmlGenericControl.Controls.Add(label);
			htmlTableCell.Controls.Add(htmlGenericControl);
			htmlTableCell3.Controls.Add(this.tbxAtLeast);
			htmlTableRow.Cells.Add(htmlTableCell);
			htmlTableRow.Cells.Add(htmlTableCell3);
			htmlTable.Rows.Add(htmlTableRow);
			htmlGenericControl2.Controls.Add(label2);
			htmlTableCell2.Controls.Add(htmlGenericControl2);
			htmlTableCell4.Controls.Add(this.tbxAtMost);
			htmlTableRow2.Cells.Add(htmlTableCell2);
			htmlTableRow2.Cells.Add(htmlTableCell4);
			htmlTable.Rows.Add(htmlTableRow2);
			this.Controls.Add(htmlTable);
			NumericInputExtender numericInputExtender = new NumericInputExtender();
			numericInputExtender.TargetControlID = this.tbxAtLeast.UniqueID;
			this.Controls.Add(numericInputExtender);
			NumericInputExtender numericInputExtender2 = new NumericInputExtender();
			numericInputExtender2.TargetControlID = this.tbxAtMost.UniqueID;
			this.Controls.Add(numericInputExtender2);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("AtLeastTextbox", this.AtLeastTextboxID, this);
			descriptor.AddElementProperty("AtMostTextbox", this.AtMostTextboxID, this);
		}

		private TextBox tbxAtLeast;

		private TextBox tbxAtMost;
	}
}
