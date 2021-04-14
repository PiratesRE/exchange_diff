using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpAliasListValidator : EcpRegularExpressionValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ValidationExpression = "^[^=)(;\"><\\*]*$";
		}

		public override string DefaultErrorMessage
		{
			get
			{
				return Strings.AliasListValidatorMessage;
			}
		}

		public override string TypeId
		{
			get
			{
				return "AliasList";
			}
		}
	}
}
