using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[RequiredScript(typeof(WizardForm))]
	[ClientScriptResource("ReleaseQuarantine", "Microsoft.Exchange.Management.ControlPanel.Client.Quarantine.js")]
	public class ReleaseQuarantine : BaseForm
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			RadioButton radioButton = (RadioButton)this.properties.Controls[0].FindControl("rbReleaseSelected");
			RadioButton radioButton2 = (RadioButton)this.properties.Controls[0].FindControl("rbReleaseAll");
			PickerContent pickerContent = (PickerContent)this.properties.Controls[0].FindControl("pickerContent");
			descriptor.AddComponentProperty("PickerContent", pickerContent.ClientID, true);
			descriptor.AddElementProperty("ReleaseAllRadioButton", radioButton2.ClientID, true);
			descriptor.AddElementProperty("ReleaseSelectedRadioButton", radioButton.ClientID, true);
			base.BuildScriptDescriptor(descriptor);
		}

		protected PropertyPageSheet properties;
	}
}
