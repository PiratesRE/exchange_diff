using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("WizardStep", "Microsoft.Exchange.Management.ControlPanel.Client.Wizard.js")]
	public class WizardStep : WizardStepBase
	{
		public WizardStep()
		{
			base.ClientClassName = "WizardStep";
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<WizardStep>(this);
		}

		protected override ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptDescriptor = base.GetScriptDescriptor();
			scriptDescriptor.AddProperty("StepID", this.ID);
			scriptDescriptor.AddProperty("NextStepID", base.NextStepID);
			return scriptDescriptor;
		}

		protected Properties FindPropertiesParent()
		{
			Properties properties = null;
			Control parent = this.Parent;
			while (parent != null && parent != this.Page)
			{
				properties = (parent as Properties);
				if (properties != null)
				{
					break;
				}
				parent = parent.Parent;
			}
			return properties;
		}
	}
}
