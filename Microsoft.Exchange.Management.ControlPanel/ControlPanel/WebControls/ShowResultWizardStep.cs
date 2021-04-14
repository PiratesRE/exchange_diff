using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ShowResultWizardStep : WizardStep
	{
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.messageDiv = new Panel();
			this.messageDiv.ID = "msgDiv";
			this.messageDiv.CssClass = "PropertyDiv WizardTxtDiv";
			this.Controls.Add(this.messageDiv);
		}

		public string SucceededText { get; set; }

		[IDReferenceProperty(typeof(WebServiceWizardStep))]
		public string WebServiceStepID { get; set; }

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "ShowResultWizardStep";
			Control control = this.NamingContainer.FindControl(this.WebServiceStepID);
			scriptDescriptor.AddComponentProperty("WebServiceStep", control.ClientID);
			scriptDescriptor.AddElementProperty("MessageDiv", this.messageDiv.ClientID);
			scriptDescriptor.AddProperty("SucceededText", this.SucceededText);
			return scriptDescriptor;
		}

		private Panel messageDiv;
	}
}
