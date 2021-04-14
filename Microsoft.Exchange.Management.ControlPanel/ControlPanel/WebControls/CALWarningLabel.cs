using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class CALWarningLabel : WebControl
	{
		public CALWarningLabel() : base(HtmlTextWriterTag.Table)
		{
		}

		public string EnterpriseText { get; set; }

		public string DatacenterText { get; set; }

		protected override void CreateChildControls()
		{
			string text = null;
			string helpId = EACHelpId.LearnMoreCAL.ToString();
			if (RbacPrincipal.Current.IsInRole("Enterprise"))
			{
				text = this.EnterpriseText;
			}
			else if (RbacPrincipal.Current.IsInRole("LiveID"))
			{
				text = this.DatacenterText;
			}
			if (text == null)
			{
				this.Visible = false;
				base.CreateChildControls();
				return;
			}
			TableRow tableRow = new TableRow();
			tableRow.VerticalAlign = VerticalAlign.Top;
			this.Controls.Add(tableRow);
			TableCell tableCell = new TableCell();
			tableRow.Controls.Add(tableCell);
			CommonSprite commonSprite = new CommonSprite();
			commonSprite.ImageId = CommonSprite.SpriteId.Information;
			tableCell.Controls.Add(commonSprite);
			TableCell tableCell2 = new TableCell();
			tableRow.Controls.Add(tableCell2);
			HelpLink helpLink = new HelpLink();
			helpLink.Text = text;
			helpLink.TextIsFormatString = true;
			helpLink.HelpId = helpId;
			tableCell2.Controls.Add(helpLink);
			base.CreateChildControls();
		}
	}
}
