using System;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using Microsoft.Security.Application;

namespace Microsoft.Exchange.Management.ControlPanel.Reporting
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Reporting.js")]
	public class ViewMailboxSearchErrorsProperties : Properties
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			HtmlTableCell htmlTableCell = (HtmlTableCell)base.ContentContainer.FindControl("genericErrorsLink");
			PowerShellResults<MailboxSearch> powerShellResults = base.Results as PowerShellResults<MailboxSearch>;
			if (powerShellResults != null && powerShellResults.SucceededWithValue)
			{
				MailboxSearch mailboxSearch = powerShellResults.Output[0];
				string leftPart = this.Context.GetRequestUrl().GetLeftPart(UriPartial.Authority);
				string arg = Encoder.UrlEncode(Encoder.HtmlEncode(mailboxSearch.Identity.RawIdentity));
				HtmlAnchor htmlAnchor = new HtmlAnchor();
				htmlAnchor.InnerHtml = Strings.ClickHereForDetails;
				string arg2 = string.Format("{0}/ecp/Reporting/ViewMailboxSearchGenericErrors.aspx?Id={1}", leftPart, arg);
				htmlAnchor.HRef = "#";
				htmlAnchor.Attributes.Add("onclick", string.Format("window.open(\"{0}\",\"_blank\",\"width=800,height=600,scrollbars=yes,resizable=no,toolbar=yes,directories=no,location=center,menubar=yes,status=yes\"); return false", arg2));
				if (htmlTableCell != null && mailboxSearch.TotalUndefinedErrors > 0)
				{
					htmlTableCell.Controls.Add(htmlAnchor);
				}
				htmlAnchor.Dispose();
			}
		}
	}
}
