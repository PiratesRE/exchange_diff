using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class PickerContentWithDetails : PickerContent
	{
		public string DetailsUrl { get; set; }

		protected override void AddDetails()
		{
			if (!string.IsNullOrEmpty(this.DetailsUrl))
			{
				DetailsPane detailsPane = new DetailsPane();
				detailsPane.SuppressFrameCache = true;
				detailsPane.CssClass = "rolePickerDetailsPane";
				detailsPane.ID = "DetailedContent";
				detailsPane.SourceID = base.ListView.ID;
				detailsPane.BaseUrl = this.DetailsUrl;
				detailsPane.FrameTitle = Strings.ViewDetails;
				base.IsMasterDetailed = true;
				Table table = new Table();
				table.CssClass = "masterDetailsTable";
				table.BorderWidth = Unit.Pixel(0);
				table.CellPadding = 0;
				table.CellSpacing = 0;
				TableRow tableRow = new TableRow();
				TableCell tableCell = new TableCell();
				tableCell.CssClass = "rolePickerListCell";
				tableCell.VerticalAlign = VerticalAlign.Top;
				tableCell.Controls.Add(base.ListView);
				tableRow.Cells.Add(tableCell);
				TableCell tableCell2 = new TableCell();
				tableCell2.CssClass = "rolePickerDetailsCell";
				tableCell2.Controls.Add(detailsPane);
				tableRow.Cells.Add(tableCell2);
				table.Rows.Add(tableRow);
				base.ContentPanel.Controls.Add(table);
				return;
			}
			base.AddDetails();
		}
	}
}
