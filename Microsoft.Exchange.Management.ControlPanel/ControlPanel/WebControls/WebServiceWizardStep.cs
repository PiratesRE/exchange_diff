using System;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("WebServiceWizardStep", "Microsoft.Exchange.Management.ControlPanel.Client.Wizard.js")]
	public class WebServiceWizardStep : WizardStep
	{
		public WebServiceWizardStep()
		{
			base.ClientClassName = "WebServiceWizardStep";
			this.ShowErrors = true;
			Util.RequireUpdateProgressPopUp(this);
		}

		public string WebServiceMethodName { get; set; }

		public string ParameterNames { get; set; }

		public string ProgressDescription { get; set; }

		[DefaultValue(false)]
		public bool NextOnCancel { get; set; }

		[DefaultValue(false)]
		public bool NextOnError { get; set; }

		[DefaultValue(true)]
		public bool ShowErrors { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.propertyPage = base.FindPropertiesParent();
			this.webServiceMethod = new WebServiceMethod();
			this.webServiceMethod.ServiceUrl = this.propertyPage.ServiceUrl;
			this.webServiceMethod.ID = this.ID + "WebServiceMethod";
			this.webServiceMethod.Method = this.WebServiceMethodName;
			if (string.IsNullOrEmpty(this.ParameterNames))
			{
				this.webServiceMethod.ParameterNames = WebServiceParameterNames.SetObject;
			}
			else
			{
				this.webServiceMethod.ParameterNames = (WebServiceParameterNames)Enum.Parse(typeof(WebServiceParameterNames), this.ParameterNames);
			}
			this.Controls.Add(this.webServiceMethod);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.AddComponentProperty("PropertyPage", this.propertyPage.ClientID);
			scriptDescriptor.AddComponentProperty("WebServiceMethod", this.webServiceMethod.ClientID);
			scriptDescriptor.AddProperty("ProgressDescription", this.ProgressDescription ?? Strings.PleaseWait);
			scriptDescriptor.AddProperty("NextOnCancel", this.NextOnCancel);
			scriptDescriptor.AddProperty("NextOnError", this.NextOnError);
			scriptDescriptor.AddProperty("ShowErrors", this.ShowErrors);
			return scriptDescriptor;
		}

		private Properties propertyPage;

		private WebServiceMethod webServiceMethod;
	}
}
