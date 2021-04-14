using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("CalendarDiagnosticLogProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Calendar.js")]
	public sealed class CalendarDiagnosticLogProperties : Properties, IScriptControl
	{
		public CalendarDiagnosticLogProperties()
		{
			Util.RequireUpdateProgressPopUp(this);
		}

		public WebServiceMethod SendLogWebServiceMethod { get; private set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.SendLogWebServiceMethod = new WebServiceMethod();
			this.SendLogWebServiceMethod.ID = "SendCalendarDiagnosticLog";
			this.SendLogWebServiceMethod.ServiceUrl = base.ServiceUrl;
			this.SendLogWebServiceMethod.Method = "SendLog";
			this.SendLogWebServiceMethod.ParameterNames = WebServiceParameterNames.Custom;
			base.ContentContainer.Controls.Add(this.SendLogWebServiceMethod);
		}

		protected override void OnPreRender(EventArgs e)
		{
			InvokeWebService invokeWebService = new InvokeWebService();
			invokeWebService.ID = "webServiceBehavior";
			invokeWebService.TargetControlID = "btnTroubleShoot";
			invokeWebService.EnableProgressPopup = true;
			invokeWebService.CloseAfterSuccess = true;
			invokeWebService.WebServiceMethods.Add(this.SendLogWebServiceMethod);
			invokeWebService.ProgressDescription = OwaOptionStrings.Processing;
			base.ContentContainer.Controls.Add(invokeWebService);
			base.OnPreRender(e);
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptDescriptor = this.GetScriptDescriptor();
			scriptDescriptor.Type = "CalendarDiagnosticLogProperties";
			if (this.SendLogWebServiceMethod != null)
			{
				scriptDescriptor.AddComponentProperty("SendLogWebServiceMethod", this.SendLogWebServiceMethod.ClientID);
			}
			return new ScriptControlDescriptor[]
			{
				scriptDescriptor
			};
		}
	}
}
