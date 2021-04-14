using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("CalendarWorkflowProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Calendar.js")]
	public sealed class CalendarWorkflowProperties : Properties
	{
		public WebServiceMethod UpdateWebServiceMethod { get; private set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.UpdateWebServiceMethod = new WebServiceMethod();
			this.UpdateWebServiceMethod.ID = "EnableAutomateProcessing";
			this.UpdateWebServiceMethod.ServiceUrl = base.ServiceUrl;
			this.UpdateWebServiceMethod.Method = "UpdateObject";
			this.UpdateWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
			this.Controls.Add(this.UpdateWebServiceMethod);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "CalendarWorkflowProperties";
			if (this.UpdateWebServiceMethod != null)
			{
				scriptDescriptor.AddComponentProperty("UpdateWebServiceMethod", this.UpdateWebServiceMethod.ClientID);
			}
			return scriptDescriptor;
		}
	}
}
