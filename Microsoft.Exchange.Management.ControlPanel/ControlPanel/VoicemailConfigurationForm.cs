using System;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class VoicemailConfigurationForm : WizardForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			CommonMaster commonMaster = (CommonMaster)base.Master;
			commonMaster.AddCssFiles("VoicemailSprite.css");
			this.txtPhoneNumber_Validator2.ValidationExpression = "^(?:\\([2-9]\\d{2}\\)\\ ?|[2-9]\\d{2}(?:\\-?|\\ ?))[2-9]\\d{2}[- ]?\\d{4}$";
		}

		private const string PhoneNumberValidationRegRex = "^(?:\\([2-9]\\d{2}\\)\\ ?|[2-9]\\d{2}(?:\\-?|\\ ?))[2-9]\\d{2}[- ]?\\d{4}$";

		protected EcpRegularExpressionValidator txtPhoneNumber_Validator2;
	}
}
