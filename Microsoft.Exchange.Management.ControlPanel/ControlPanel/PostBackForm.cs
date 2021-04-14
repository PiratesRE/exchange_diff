using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("PostBackForm", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class PostBackForm : BaseForm
	{
		public PostBackForm()
		{
			base.FooterPanel.State = ButtonsPanelState.Wizard;
			base.FooterPanel.CloseWindowOnCancel = true;
			this.currentObjectIdField = new HiddenField();
			this.currentObjectIdField.ID = "currentRequestIdField";
			base.ContentPanel.Controls.Add(this.currentObjectIdField);
			this.senderBtn = new HiddenField();
			this.senderBtn.ID = "senderBtn";
			base.ContentPanel.Controls.Add(this.senderBtn);
			Util.RequireUpdateProgressPopUp(this);
		}

		protected string CurrentObjectId
		{
			get
			{
				return this.currentObjectIdField.Value;
			}
			set
			{
				this.currentObjectIdField.Value = value;
			}
		}

		protected string SenderButtonId
		{
			get
			{
				return this.senderBtn.Value;
			}
		}

		protected string CommitButtonProgressDescription { get; set; }

		protected string BackButtonProgressDescription { get; set; }

		protected string NextButtonProgressDescription { get; set; }

		protected bool IsInError { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			this.EnableViewState = true;
			base.OnLoad(e);
			if (base.IsPostBack)
			{
				if (string.Compare(this.SenderButtonId, base.CommitButtonClientID) != 0 && string.Compare(this.SenderButtonId, base.BackButton.ClientID) != 0 && string.Compare(this.SenderButtonId, base.NextButton.ClientID) != 0)
				{
					this.ShowDisabledWizard(Strings.WebServiceErrorMessage);
					return;
				}
			}
			else
			{
				Exception lastError = base.Server.GetLastError();
				if (lastError != null)
				{
					InfoCore infoCore = lastError.ToErrorInformationBase().ToInfo();
					if (lastError.IsMaxRequestLengthExceededException())
					{
						infoCore.Message = Strings.MigrationFileTooBig;
						infoCore.Details = string.Empty;
						base.Server.ClearError();
					}
					ErrorHandlingUtil.ShowServerError(infoCore, this.Page);
				}
			}
		}

		protected void SetButtonVisibility(bool backButtonVisible, bool nextButtonVisible, bool commitButtonVisible)
		{
			this.SetControlVisibility(base.BackButton, backButtonVisible);
			this.SetControlVisibility(base.NextButton, nextButtonVisible);
			this.SetControlVisibility(base.CommitButton, commitButtonVisible);
		}

		protected void ShowDisabledWizard(string reason)
		{
			this.IsInError = true;
			base.NextButton.Disabled = true;
			base.BackButton.Disabled = true;
			base.CommitButton.Disabled = true;
			ErrorHandlingUtil.ShowServerError(reason, string.Empty, this.Page);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("CommitButtonProgressDescription", this.CommitButtonProgressDescription, true);
			descriptor.AddProperty("NextButtonProgressDescription", this.NextButtonProgressDescription, true);
			descriptor.AddProperty("BackButtonProgressDescription", this.BackButtonProgressDescription, true);
			descriptor.AddElementProperty("NextButton", base.NextButton.ClientID);
			descriptor.AddElementProperty("BackButton", base.BackButton.ClientID);
			descriptor.AddElementProperty("SenderButtonField", this.senderBtn.ClientID);
		}

		protected void SetControlVisibility(HtmlControl ctrl, bool visible)
		{
			ctrl.Visible = visible;
			if (visible)
			{
				ctrl.Style.Remove(HtmlTextWriterStyle.Display);
				return;
			}
			ctrl.Style[HtmlTextWriterStyle.Display] = "none";
		}

		private HiddenField currentObjectIdField;

		private HiddenField senderBtn;
	}
}
