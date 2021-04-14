using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("UMEnableWizard", "Microsoft.Exchange.Management.ControlPanel.Client.UnifiedMessaging.js")]
	public sealed class UMEnableWizard : Properties, IScriptControl
	{
		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptDescriptor = this.GetScriptDescriptor();
			scriptDescriptor.Type = "UMEnableWizard";
			scriptDescriptor.AddElementProperty("Extension", base.ContentContainer.FindControl("txtExtension").ClientID);
			scriptDescriptor.AddElementProperty("SipResourceIdentifier", base.ContentContainer.FindControl("txtSip").ClientID);
			scriptDescriptor.AddElementProperty("E164ResourceIdentifier", base.ContentContainer.FindControl("txtE164").ClientID);
			scriptDescriptor.AddComponentProperty("PolicySelector", base.ContentContainer.FindControl("pickerUMMailboxPolicy").ClientID);
			scriptDescriptor.AddElementProperty("E164Section", base.ContentContainer.FindControl("e164Div").ClientID);
			scriptDescriptor.AddElementProperty("SipSection", base.ContentContainer.FindControl("sipDiv").ClientID);
			scriptDescriptor.AddElementProperty("ExtensionDigitsLabel", base.ContentContainer.FindControl("txtExtension_label").ClientID);
			scriptDescriptor.AddElementProperty("ExtensionStepActionSummaryLabel", base.ContentContainer.FindControl("lblStep2ActionSummary").ClientID);
			scriptDescriptor.AddElementProperty("ExtensionDescriptionLabel", base.ContentContainer.FindControl("txtExtension_label2").ClientID);
			scriptDescriptor.AddProperty("ManualPinForLabel", base.ContentContainer.FindControl("rbPinSettings").ClientID + "_1");
			scriptDescriptor.AddElementProperty("SipAutoLabel", base.ContentContainer.FindControl("txtSip_label2").ClientID);
			scriptDescriptor.AddElementProperty("ManualPin", base.ContentContainer.FindControl("txtPin").ClientID);
			return new ScriptControlDescriptor[]
			{
				scriptDescriptor
			};
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			StaticBinding staticBinding = new StaticBinding();
			staticBinding.Name = "Identity";
			staticBinding.Value = base.ObjectIdentity;
			base.Bindings.Bindings.Add("Identity", staticBinding);
		}
	}
}
