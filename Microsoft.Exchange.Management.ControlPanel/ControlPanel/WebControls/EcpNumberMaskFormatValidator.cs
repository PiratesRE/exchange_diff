using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpNumberMaskFormatValidator : EcpRegularExpressionValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ValidationExpression = "^\\*$|^\\d+\\*$|^\\d+x+$|^x+$|^\\d+$";
		}

		public override string DefaultErrorMessage
		{
			get
			{
				return Strings.NumberMaskFormatValidatorMessage;
			}
		}

		public override string TypeId
		{
			get
			{
				return "NumberMaskFormat";
			}
		}
	}
}
