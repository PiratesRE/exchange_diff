using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ManagePolicyFromISV : PostBackForm
	{
		public ManagePolicyFromISV()
		{
			base.NextButtonText = Strings.DLPUploadFileBUttonText;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Control control = base.ContentPanel.FindControl("isvpropertypage").FindControl("contentContainer");
			control.PreRender += this.Ctrl_PreRender;
			if (base.IsPostBack && base.SenderButtonId == base.NextButton.ClientID)
			{
				this.ExecuteUpload();
			}
		}

		private void Ctrl_PreRender(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			this.wizardPage1 = (Panel)control.FindControl("step1");
			this.wizardPage1.Visible = true;
			this.wizardPage1.Enabled = !base.IsInError;
			base.FooterPanel.State = ButtonsPanelState.SaveCancel;
			base.SetButtonVisibility(false, true, false);
			base.SetControlVisibility(base.CancelButton, true);
			this.policyState.SelectedValue = "Enabled";
		}

		private void ExecuteUpload()
		{
			try
			{
				if (base.Request.Files.Count == 0 || string.IsNullOrEmpty(base.Request.Files[0].FileName))
				{
					ErrorHandlingUtil.ShowServerError(Strings.ISVNoFileUploaded, string.Empty, this.Page);
				}
				else
				{
					DLPISVService dlpisvservice = new DLPISVService();
					HttpPostedFile httpPostedFile = base.Request.Files[0];
					byte[] array = new byte[httpPostedFile.ContentLength];
					httpPostedFile.InputStream.Read(array, 0, array.Length);
					PowerShellResults powerShellResults = dlpisvservice.ProcessUpload(new DLPNewPolicyUploadParameters
					{
						Mode = this.policyMode.SelectedValue,
						State = RuleState.Enabled.ToString(),
						Name = this.name.Text,
						Description = this.description.Text,
						TemplateData = array
					});
					if (powerShellResults.Failed)
					{
						ErrorHandlingUtil.ShowServerErrors(powerShellResults.ErrorRecords, this.Page);
					}
					else
					{
						this.Page.RegisterStartupScript("windowclose", string.Format("<script>{0}</script>", "window.opener.RefreshPolicyListView();window.close();"));
					}
				}
			}
			catch (Exception ex)
			{
				ErrorHandlingUtil.ShowServerError(ex.Message, string.Empty, this.Page);
			}
		}

		private const string CloseWindowScript = "window.opener.RefreshPolicyListView();window.close();";

		protected PlaceHolder uploadPanel;

		protected TextBox name;

		protected TextArea description;

		protected RadioButtonList policyMode;

		protected RadioButtonList policyState;

		protected Panel wizardPage1;
	}
}
