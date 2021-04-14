using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("CalendarAppearanceProperties", "Microsoft.Exchange.Management.ControlPanel.Client.Calendar.js")]
	public sealed class CalendarAppearanceProperties : Properties
	{
		public WebServiceMethod UpdateWebServiceMethod { get; private set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.UpdateWebServiceMethod = new WebServiceMethod();
			this.UpdateWebServiceMethod.ID = "UpdateTimeZone";
			this.UpdateWebServiceMethod.ServiceUrl = base.ServiceUrl;
			this.UpdateWebServiceMethod.Method = "UpdateObject";
			this.UpdateWebServiceMethod.ParameterNames = WebServiceParameterNames.Identity;
			this.Controls.Add(this.UpdateWebServiceMethod);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "CalendarAppearanceProperties";
			if (this.UpdateWebServiceMethod != null)
			{
				scriptDescriptor.AddComponentProperty("UpdateWebServiceMethod", this.UpdateWebServiceMethod.ClientID);
			}
			return scriptDescriptor;
		}
	}
}
