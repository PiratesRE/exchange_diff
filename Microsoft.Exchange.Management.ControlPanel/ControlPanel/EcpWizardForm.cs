using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("EcpWizardForm", "Microsoft.Exchange.Management.ControlPanel.Client.Wizard.js")]
	public class EcpWizardForm : WizardFormBase
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.NextButton.Attributes.Add("data-visible", "{CanNext, Mode=OneWay}");
			base.BackButton.Attributes.Add("data-visible", "{CanBack, Mode=OneWay}");
			base.CommitButton.Attributes.Add("data-visible", "{IsCommitStep, Mode=OneWay}");
			base.OnPreRender(e);
		}
	}
}
