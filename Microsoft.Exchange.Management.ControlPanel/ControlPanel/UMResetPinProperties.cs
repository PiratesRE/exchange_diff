using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class UMResetPinProperties : Properties
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.SetDynamicLabels();
		}

		private void SetDynamicLabels()
		{
			PowerShellResults<SetUMMailboxPinConfiguration> powerShellResults = (PowerShellResults<SetUMMailboxPinConfiguration>)base.Results;
			if (powerShellResults != null && powerShellResults.SucceededWithValue)
			{
				Section section = base.Sections["ResetUMPinSection"];
				RadioButtonList radioButtonList = (RadioButtonList)section.FindControl("rbResetPin");
				radioButtonList.Items[1].Text = Strings.UMMailboxPinDigitLabel(powerShellResults.Value.MinPinLength.ToString());
				EcpRegularExpressionValidator ecpRegularExpressionValidator = (EcpRegularExpressionValidator)section.FindControl("txtPin_Validator2");
				ecpRegularExpressionValidator.ValidationExpression = CommonRegex.NumbersOfSpecificLength(powerShellResults.Value.MinPinLength, 24).ToString();
				ecpRegularExpressionValidator.DefaultErrorMessage = Strings.UMResetPinManualPinError(powerShellResults.Value.MinPinLength);
			}
		}
	}
}
