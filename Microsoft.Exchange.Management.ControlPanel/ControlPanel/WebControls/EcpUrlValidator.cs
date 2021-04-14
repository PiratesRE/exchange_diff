using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpUrlValidator : EcpRegularExpressionValidator
	{
		public override string DefaultErrorMessage
		{
			get
			{
				return Strings.UrlValidatorErrorMessage;
			}
		}

		public override string TypeId
		{
			get
			{
				return "Url";
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ValidationExpression = CommonRegex.Url.ToString();
		}
	}
}
