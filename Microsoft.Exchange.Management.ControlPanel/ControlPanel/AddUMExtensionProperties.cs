using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("AddUMExtensionProperties", "Microsoft.Exchange.Management.ControlPanel.Client.UnifiedMessaging.js")]
	public sealed class AddUMExtensionProperties : Properties
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (string.IsNullOrEmpty(this.Page.Request["dialPlanId"]))
			{
				throw new BadQueryParameterException("dialPlanId");
			}
			EcpSingleSelect ecpSingleSelect = (EcpSingleSelect)base.ContentContainer.FindControl("pickerUMDialPlan");
			ecpSingleSelect.PickerFormUrl = EcpUrl.AppendQueryParameter(ecpSingleSelect.PickerFormUrl, "dialPlanId", this.Page.Request["dialPlanId"]);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.Type = "AddUMExtensionProperties";
			scriptDescriptor.AddElementProperty("Extension", base.ContentContainer.FindControl("txtExtension").ClientID);
			scriptDescriptor.AddElementProperty("PhoneContext", base.ContentContainer.FindControl("txtPhoneContext").ClientID);
			scriptDescriptor.AddElementProperty("DialPlanName", base.ContentContainer.FindControl("txtDialPlanName").ClientID);
			scriptDescriptor.AddElementProperty("ExtensionLabel", base.ContentContainer.FindControl("txtExtension_label").ClientID);
			scriptDescriptor.AddComponentProperty("DialPlanSelector", base.ContentContainer.FindControl("pickerUMDialPlan").ClientID);
			return scriptDescriptor;
		}
	}
}
