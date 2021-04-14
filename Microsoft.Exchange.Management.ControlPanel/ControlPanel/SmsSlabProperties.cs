using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Wizard.js")]
	public class SmsSlabProperties : Properties
	{
		public SmsSlabProperties(string disableMethodName, string editSettingPage)
		{
			this.disableMethodName = disableMethodName;
			this.editSettingPage = editSettingPage;
		}

		public WebServiceMethod DisableWebServiceMethod { get; private set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.disableMethodName != null)
			{
				this.DisableWebServiceMethod = new WebServiceMethod();
				this.DisableWebServiceMethod.ID = "Disable";
				this.DisableWebServiceMethod.Method = this.disableMethodName;
				this.DisableWebServiceMethod.ServiceUrl = base.ServiceUrl;
				this.DisableWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
				this.Controls.Add(this.DisableWebServiceMethod);
			}
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "SmsSlabProperties";
			if (this.DisableWebServiceMethod != null)
			{
				scriptDescriptor.AddComponentProperty("DisableWebServiceMethod", this.DisableWebServiceMethod.ClientID);
			}
			scriptDescriptor.AddProperty("EditSettingPage", this.editSettingPage);
			return scriptDescriptor;
		}

		private string disableMethodName;

		private string editSettingPage;
	}
}
