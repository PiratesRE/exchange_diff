using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.SMSProperties.js")]
	public class SmsWizardProperties : Properties
	{
		public WebServiceMethod SendVerificationCodeWebServiceMethod { get; private set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.SendVerificationCodeWebServiceMethod = new WebServiceMethod();
			this.SendVerificationCodeWebServiceMethod.ID = "SendVerificationCode";
			this.SendVerificationCodeWebServiceMethod.Method = "SendVerificationCode";
			this.SendVerificationCodeWebServiceMethod.ServiceUrl = base.ServiceUrl;
			this.SendVerificationCodeWebServiceMethod.ParameterNames = WebServiceParameterNames.SetObject;
			this.Controls.Add(this.SendVerificationCodeWebServiceMethod);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "SmsWizardProperties";
			scriptDescriptor.AddProperty("ServiceProviders", SmsServiceProviders.Instance);
			if (this.IsNotificationWizard)
			{
				scriptDescriptor.AddProperty("IsNotificationWizard", true);
			}
			scriptDescriptor.AddComponentProperty("SendVerificationCodeWebServiceMethod", this.SendVerificationCodeWebServiceMethod.ClientID);
			return scriptDescriptor;
		}

		public bool IsNotificationWizard { get; set; }
	}
}
