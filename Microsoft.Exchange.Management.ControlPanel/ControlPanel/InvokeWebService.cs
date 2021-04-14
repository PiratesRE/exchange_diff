using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[TargetControlType(typeof(Control))]
	[ClientScriptResource("InvokeWebService", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class InvokeWebService : ExtenderControlBase
	{
		public InvokeWebService()
		{
			this.Trigger = "click";
			this.WebServiceMethods = new List<WebServiceMethod>();
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.Trigger != "click")
			{
				descriptor.AddProperty("Trigger", this.Trigger);
			}
			if (this.EnableConfirmation)
			{
				descriptor.AddProperty("EnableConfirmation", true);
			}
			if (this.IsSaveMethod)
			{
				descriptor.AddProperty("IsSaveMethod", true);
			}
			if (this.EnableProgressPopup)
			{
				descriptor.AddProperty("EnableProgressPopup", true);
			}
			descriptor.AddProperty("ProgressDescription", this.ProgressDescription, true);
			descriptor.AddProperty("AssociateElementID", this.AssociateElementID, true);
			this.CheckWebServiceErrorHandlers(this.WebServiceMethods);
			descriptor.AddProperty("WebServiceMethodIDs", this.WebServiceMethodIDs, true);
			if (this.CloseAfterSuccess)
			{
				descriptor.AddProperty("CloseAfterSuccess", true);
			}
		}

		public string Trigger { get; set; }

		public bool EnableConfirmation { get; set; }

		public bool IsSaveMethod { get; set; }

		public bool EnableProgressPopup { get; set; }

		public string ProgressDescription { get; set; }

		public string AssociateElementID { get; set; }

		public string WebServiceMethodIDs
		{
			get
			{
				return string.Join(",", (from saveMethod in this.WebServiceMethods
				select saveMethod.ClientID).ToArray<string>());
			}
		}

		private void CheckWebServiceErrorHandlers(List<WebServiceMethod> webServices)
		{
			int num = 0;
			foreach (WebServiceMethod webServiceMethod in webServices)
			{
				if (webServiceMethod.ExceptionHandlers.Count > 0 && ++num > 1)
				{
					throw new NotSupportedException("Current we only allow one webservice to have handler.");
				}
			}
		}

		public List<WebServiceMethod> WebServiceMethods { get; private set; }

		public bool CloseAfterSuccess { get; set; }
	}
}
