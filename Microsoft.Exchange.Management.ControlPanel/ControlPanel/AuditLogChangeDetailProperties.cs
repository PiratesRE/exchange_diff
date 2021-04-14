using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class AuditLogChangeDetailProperties : Properties
	{
		protected abstract string DetailsPaneId { get; }

		protected abstract string HeaderLabelId { get; }

		protected abstract string GetDetailsPaneHeader();

		protected abstract void RenderChanges();

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.detailsPane == null)
			{
				this.FindDetailsPane();
			}
			if (base.Results != null && base.Results.Succeeded)
			{
				this.RenderDetailsHeader();
				this.RenderChanges();
				return;
			}
			base.ContentContainer.Visible = false;
		}

		private void RenderDetailsHeader()
		{
			Control control = this.detailsPane.FindControl(this.HeaderLabelId);
			if (control != null)
			{
				((Label)control).Text = this.GetDetailsPaneHeader();
			}
		}

		protected TableRow GetDetailRowForTable(string typeOfInformation, string information)
		{
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.Text = typeOfInformation;
			tableCell.CssClass = "breakWord";
			tableCell.VerticalAlign = VerticalAlign.Bottom;
			TableCell tableCell2 = new TableCell();
			tableCell2.Width = 5;
			TableCell tableCell3 = new TableCell();
			tableCell3.Text = information;
			tableCell3.VerticalAlign = VerticalAlign.Bottom;
			tableCell3.CssClass = "breakWord";
			tableRow.Cells.Add(tableCell);
			tableRow.Cells.Add(tableCell2);
			tableRow.Cells.Add(tableCell3);
			return tableRow;
		}

		protected TableRow GetDetailRowForTable(string textInRow)
		{
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.Text = textInRow;
			tableCell.CssClass = "breakWord";
			tableCell.VerticalAlign = VerticalAlign.Bottom;
			tableRow.Cells.Add(tableCell);
			return tableRow;
		}

		protected TableRow GetEmptyRowForTable()
		{
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.Text = "&nbsp;";
			tableRow.Cells.Add(tableCell);
			return tableRow;
		}

		private void FindDetailsPane()
		{
			this.detailsPane = (HtmlGenericControl)base.ContentContainer.FindControl(this.DetailsPaneId);
		}

		protected HtmlGenericControl detailsPane;
	}
}
