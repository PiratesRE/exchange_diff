using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("DownloadReport", "Microsoft.Exchange.Management.ControlPanel.Client.Users.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class DownloadReport : EcpContentPage
	{
		protected override void OnLoad(EventArgs e)
		{
			string value = this.Context.Request.QueryString["Identity"];
			if (string.IsNullOrEmpty(value))
			{
				throw new BadQueryParameterException("Identity");
			}
			if (string.IsNullOrEmpty(this.Context.Request.QueryString["Name"]))
			{
				throw new BadQueryParameterException("Name");
			}
			if (string.IsNullOrEmpty(this.Context.Request.QueryString["HandlerClass"]))
			{
				throw new BadQueryParameterException("HandlerClass");
			}
			Identity identity = Identity.FromIdParameter(value);
			WebServiceReference webServiceReference = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=MigrationReport");
			PowerShellResults<JsonDictionary<object>> powerShellResults = (PowerShellResults<JsonDictionary<object>>)webServiceReference.GetObject(identity);
			if (!powerShellResults.SucceededWithValue)
			{
				throw new BadQueryParameterException("Identity");
			}
			if ((MigrationType)powerShellResults.Output[0]["MigrationType"] == MigrationType.BulkProvisioning)
			{
				this.OverrideStringsForBulkProvisioning();
			}
			if (this.linkShowReport != null)
			{
				this.linkShowReport.NavigateUrl = this.Context.Request.RawUrl.Replace("DownloadReport.aspx?", "Download.aspx?");
			}
			base.OnLoad(e);
		}

		private void OverrideStringsForBulkProvisioning()
		{
			base.Title = Strings.DownloadErrorReportTitle;
			if (this.lblReportTitle != null)
			{
				this.lblReportTitle.Text = Strings.DownloadErrorReportTitle;
			}
			if (this.lblReportMsg != null)
			{
				this.lblReportMsg.Text = Strings.DownloadErrorReportText;
			}
		}

		protected Label lblReportTitle;

		protected Label lblReportMsg;

		protected HyperLink linkShowReport;
	}
}
