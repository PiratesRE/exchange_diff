using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("MicroEditPanel", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	public class MicroEditPanel : DataContextProvider
	{
		public MicroEditPanel()
		{
			base.ViewModel = "MicroEditViewModel";
		}

		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(MicroEditPanel))]
		public ITemplate Content { get; set; }

		public string Title { get; set; }

		public string LinkText { get; set; }

		public string Roles { get; set; }

		public string LocStringsResource { get; set; }

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void CreateChildControls()
		{
			this.ID = "EditPanel";
			this.CssClass = "MicroEdit";
			base.Attributes.Add("title", string.Empty);
			this.CreateTitleRow();
			this.CreateBodyRow();
			base.CreateChildControls();
		}

		private void CreateButtonRow(Panel parent)
		{
			Panel panel = new Panel();
			panel.CssClass = "btnPane";
			parent.Controls.Add(panel);
			HtmlButton htmlButton = new HtmlButton();
			htmlButton.ID = "OkButton";
			htmlButton.InnerText = Strings.CommitButtonText;
			htmlButton.Attributes.Add("data-control", "Button");
			htmlButton.Attributes.Add("data-Command", "{SaveCommand, Mode=OneWay}");
			panel.Controls.Add(htmlButton);
			HtmlButton htmlButton2 = new HtmlButton();
			htmlButton2.ID = "CancelButton";
			htmlButton2.InnerText = Strings.CancelButtonText;
			htmlButton2.Attributes.Add("data-control", "Button");
			htmlButton2.Attributes.Add("data-Command", "{CancelCommand, Mode=OneWay}");
			panel.Controls.Add(htmlButton2);
		}

		private void CreateBodyRow()
		{
			Panel panel = new Panel();
			panel.ID = "divBody";
			this.Controls.Add(panel);
			panel.CssClass = "MicroEditReservedSpaceForFVA";
			if (this.Content != null)
			{
				this.Content.InstantiateIn(panel);
			}
			if (this.LocStringsResource != null)
			{
				this.fvaExtender = new FieldValidationAssistantExtender();
				panel.Controls.Add(this.fvaExtender);
				this.fvaExtender.LocStringsResource = this.LocStringsResource;
				this.fvaExtender.TargetControlID = panel.UniqueID;
				this.fvaExtender.Canvas = panel.ClientID;
				this.fvaExtender.IndentCssClass = "baseFrmFvaIndent";
				((ToolkitScriptManager)ScriptManager.GetCurrent(this.Page)).CombineScript(this.LocStringsResource);
			}
			this.CreateButtonRow(panel);
		}

		private void CreateTitleRow()
		{
			Panel panel = new Panel();
			this.Controls.Add(panel);
			Table table = new Table();
			panel.Controls.Add(table);
			TableRow tableRow = new TableRow();
			table.Rows.Add(tableRow);
			TableCell tableCell = new TableCell();
			tableCell.Width = Unit.Percentage(100.0);
			tableRow.Cells.Add(tableCell);
			Label label = new Label();
			label.CssClass = "MicroEditTitle";
			label.Text = this.Title;
			tableCell.Controls.Add(label);
			if (this.LinkText != null)
			{
				TableCell tableCell2 = new TableCell();
				tableRow.Cells.Add(tableCell2);
				LinkButton linkButton = new LinkButton();
				linkButton.ID = "Link";
				linkButton.Text = HttpUtility.HtmlEncode(this.LinkText);
				linkButton.CssClass = "MicroEditLink";
				tableCell2.Controls.Add(linkButton);
			}
		}

		private FieldValidationAssistantExtender fvaExtender;
	}
}
