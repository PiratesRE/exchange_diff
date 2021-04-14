using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ImportContactListForm : PostBackForm
	{
		public ImportContactListForm()
		{
			base.NextButtonProgressDescription = OwaOptionStrings.ImportContactListProgress;
			base.CommitButton.Attributes["onclick"] = "\r\n            try\r\n            {\r\n                if (window.opener != null &&\r\n                    window.opener.RefreshContactList != null)\r\n                {\r\n                    window.opener.RefreshContactList();\r\n                }\r\n            }\r\n            catch(e) {} // Catch all\r\n              \r\n            window.close();";
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Control control = base.ContentPanel.FindControl("ImportContactListProperties").FindControl("contentContainer");
			control.PreRender += this.Ctrl_PreRender;
			if (base.IsPostBack)
			{
				if (base.SenderButtonId == base.NextButton.ClientID)
				{
					this.ImportContactsRequest();
					return;
				}
			}
			else
			{
				this.currentPage = ImportContactListForm.ImportContactListPageState.UploadCsvFilePage;
			}
		}

		private void ImportContactsRequest()
		{
			if (base.Request.Files.Count == 0 || string.IsNullOrEmpty(base.Request.Files[0].FileName))
			{
				this.currentPage = ImportContactListForm.ImportContactListPageState.UploadCsvFilePage;
				ErrorHandlingUtil.ShowServerError(OwaOptionStrings.ImportContactListNoFileUploaded, string.Empty, this.Page);
				return;
			}
			HttpPostedFile httpPostedFile = base.Request.Files[0];
			this.filename = string.Empty;
			try
			{
				this.filename = Path.GetFileName(httpPostedFile.FileName);
			}
			catch (ArgumentException)
			{
				this.filename = null;
			}
			if (string.IsNullOrEmpty(this.filename))
			{
				this.currentPage = ImportContactListForm.ImportContactListPageState.UploadCsvFilePage;
				ErrorHandlingUtil.ShowServerError(OwaOptionClientStrings.FileUploadFailed, string.Empty, this.Page);
				return;
			}
			ImportContactListParameters importContactListParameters = new ImportContactListParameters();
			importContactListParameters.CSVStream = httpPostedFile.InputStream;
			ImportContactList importContactList = new ImportContactList();
			PowerShellResults<ImportContactsResult> powerShellResults = importContactList.ImportObject(Identity.FromExecutingUserId(), importContactListParameters);
			if (!powerShellResults.Failed)
			{
				this.importResult = powerShellResults.Output[0];
				this.currentPage = ImportContactListForm.ImportContactListPageState.ImportContactListResultPage;
				return;
			}
			this.currentPage = ImportContactListForm.ImportContactListPageState.UploadCsvFilePage;
			if (powerShellResults.ErrorRecords[0].Exception is ImportContactsException)
			{
				ErrorHandlingUtil.ShowServerError(powerShellResults.ErrorRecords[0].Message, string.Empty, this.Page);
				return;
			}
			ErrorHandlingUtil.ShowServerErrors(powerShellResults.ErrorRecords, this.Page);
		}

		private void Ctrl_PreRender(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			this.wizardPage1 = (Panel)control.FindControl("ImportContactListPage1");
			this.wizardPage2 = (Panel)control.FindControl("ImportContactListPage2");
			if (this.currentPage == ImportContactListForm.ImportContactListPageState.UploadCsvFilePage)
			{
				this.wizardPage1.Visible = true;
				this.wizardPage1.Enabled = !base.IsInError;
				this.wizardPage2.Visible = false;
				base.FooterPanel.State = ButtonsPanelState.SaveCancel;
				base.SetButtonVisibility(false, true, false);
				base.SetControlVisibility(base.CancelButton, true);
				return;
			}
			this.wizardPage1.Visible = false;
			this.wizardPage2.Visible = true;
			this.wizardPage2.Enabled = !base.IsInError;
			this.InitializePage2();
			base.FooterPanel.State = ButtonsPanelState.Save;
			base.SetButtonVisibility(false, false, true);
			base.SetControlVisibility(base.CancelButton, false);
		}

		private void InitializePage2()
		{
			if (this.importResult != null)
			{
				Label label = (Label)this.wizardPage2.FindControl("lblImportResult");
				label.Text = OwaOptionStrings.ImportContactListPage2Result(this.filename);
				Label label2 = (Label)this.wizardPage2.FindControl("lblImportResultNumber");
				label2.Text = OwaOptionStrings.ImportContactListPage2ResultNumber(this.importResult.ContactsImported);
			}
		}

		private const string CloseWindowScript = "\r\n            try\r\n            {\r\n                if (window.opener != null &&\r\n                    window.opener.RefreshContactList != null)\r\n                {\r\n                    window.opener.RefreshContactList();\r\n                }\r\n            }\r\n            catch(e) {} // Catch all\r\n              \r\n            window.close();";

		private ImportContactsResult importResult;

		private string filename;

		private Panel wizardPage1;

		private Panel wizardPage2;

		private ImportContactListForm.ImportContactListPageState currentPage;

		private enum ImportContactListPageState
		{
			UploadCsvFilePage,
			ImportContactListResultPage
		}
	}
}
