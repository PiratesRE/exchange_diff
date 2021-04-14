using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ToolboxData("<{0}:DeliveryReportSummary runat=server></{0}:DeliveryReportSummary>")]
	[ClientScriptResource("DeliveryReportSummary", "Microsoft.Exchange.Management.ControlPanel.Client.DeliveryReports.js")]
	public class DeliveryReportSummary : ScriptControlBase, INamingContainer
	{
		public DeliveryReportSummary() : base(HtmlTextWriterTag.Div)
		{
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("ListView", this.ListViewID, this);
			if (this.RecipientCounts != null)
			{
				descriptor.AddProperty("RecipientCounts", this.RecipientCounts);
			}
			descriptor.AddElementProperty("RecipientsStatusLinksCell", this.recipientsStatusLinksCell.ClientID, this);
			descriptor.AddElementProperty("LargeRecipientsTextRow", this.largeRecipientsTextRow.ClientID, this);
			descriptor.AddComponentProperty("ListViewDataSource", this.ListViewDataSourceID, this);
			descriptor.AddProperty("IsOWA", this.IsOWA);
			descriptor.AddProperty("DeliveryReportUrl", this.DeliveryReportUrl);
			descriptor.AddComponentProperty("NewMailMessageWebServiceMethod", this.newMailMessageWSMethod.ClientID, this);
			descriptor.AddComponentProperty("DeliveryStatusDataSourceRefreshMethod", this.DeliveryStatusDataSourceRefreshMethod.ClientID, this);
		}

		public string ListViewID { get; set; }

		public RecipientCounts RecipientCounts { get; set; }

		public string ListViewDataSourceID { get; set; }

		public bool IsOWA { get; set; }

		public string DeliveryReportUrl { get; set; }

		public WebServiceReference MailMessageServiceUrl { get; set; }

		public WebServiceMethod DeliveryStatusDataSourceRefreshMethod { get; internal set; }

		protected override void CreateChildControls()
		{
			this.summaryTable = new Table();
			this.summaryTable.ID = "summaryTable";
			TableRow tableRow = new TableRow();
			EncodingLabel encodingLabel = new EncodingLabel();
			encodingLabel.Text = OwaOptionStrings.SummaryToDate;
			this.recipientsStatusLinksCell = new TableCell();
			this.recipientsStatusLinksCell.Controls.Add(encodingLabel);
			this.recipientsStatusLinksCell.ID = "recipientStatusLinks";
			tableRow.Cells.Add(this.recipientsStatusLinksCell);
			this.largeRecipientsTextRow = new TableRow();
			this.largeRecipientsTextRow.ID = "largeRecipTxtRow";
			TableCell tableCell = new TableCell();
			EncodingLabel encodingLabel2 = new EncodingLabel();
			encodingLabel2.Text = OwaOptionStrings.LargeRecipientList(30);
			tableCell.Controls.Add(encodingLabel2);
			this.largeRecipientsTextRow.Cells.Add(tableCell);
			this.summaryTable.Rows.Add(tableRow);
			this.summaryTable.Rows.Add(this.largeRecipientsTextRow);
			this.Controls.Add(this.summaryTable);
			this.AddNewMailMessageWebServiceMthod();
			base.CreateChildControls();
		}

		private void AddNewMailMessageWebServiceMthod()
		{
			this.newMailMessageWSMethod = new WebServiceMethod();
			this.newMailMessageWSMethod.ServiceUrl = this.MailMessageServiceUrl;
			this.newMailMessageWSMethod.Method = "NewObject";
			this.newMailMessageWSMethod.ID = "NewMailMessage";
			this.newMailMessageWSMethod.ParameterNames = WebServiceParameterNames.NewObject;
			this.Controls.Add(this.newMailMessageWSMethod);
		}

		private TableRow largeRecipientsTextRow;

		private TableCell recipientsStatusLinksCell;

		private WebServiceMethod newMailMessageWSMethod;

		private Table summaryTable;
	}
}
