using System;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("WizardForm", "Microsoft.Exchange.Management.ControlPanel.Client.Wizard.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class WizardForm : WizardFormBase
	{
		public string StartsWithStepID { get; private set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.StartsWithStepID = this.Page.Request.QueryString["StartsWith"];
			if (string.IsNullOrEmpty(this.StartsWithStepID))
			{
				Control control = this.FindFirstWizardStep(base.ContentPanel);
				this.StartsWithStepID = control.ID;
			}
		}

		private Control FindFirstWizardStep(Control control)
		{
			if (control is WizardStepBase)
			{
				return control;
			}
			if (control.HasControls())
			{
				for (int i = 0; i < control.Controls.Count; i++)
				{
					Control control2 = this.FindFirstWizardStep(control.Controls[i]);
					if (control2 != null)
					{
						return control2;
					}
				}
			}
			return null;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("StartsWithStepID", this.StartsWithStepID, true);
			base.BuildScriptDescriptor(descriptor);
		}

		private const string StartsWithQueryStringParameter = "StartsWith";
	}
}
