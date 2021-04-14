using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("EcpWizardStep", "Microsoft.Exchange.Management.ControlPanel.Client.Wizard.js")]
	public class EcpWizardStep : WizardStepBase
	{
		public EcpWizardStep()
		{
			this.ViewModel = "WizardStepViewModel";
			base.ClientClassName = "EcpWizardStep";
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<EcpWizardStep>(this);
			this.AddAttribute("data-type", this.ViewModel);
			this.AddAttribute("vm-ViewModelID", this.ID);
			this.AddAttribute("vm-NextViewModelID", base.NextStepID);
		}

		private void AddAttribute(string name, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				base.Attributes.Add(name, value);
			}
		}

		public string ViewModel { get; set; }
	}
}
