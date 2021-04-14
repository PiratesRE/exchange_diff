using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpNoLeadingOrTrailingSpacesValidator : EcpRegularExpressionValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ValidationExpression = "^[^\\s]+(.*[^\\s]+)?$";
		}

		public override string DefaultErrorMessage
		{
			get
			{
				return Strings.NoLeadingOrTrailingSpacesValidatorMessage;
			}
		}

		public override string TypeId
		{
			get
			{
				return "Trlspc";
			}
		}
	}
}
