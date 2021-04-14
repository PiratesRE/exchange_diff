using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpEmailAddressValidator : EcpRegularExpressionValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ValidationExpression = "^[a-zA-Z0-9-_.!#$%&*+-/=?^_|~]+@[a-zA-Z0-9-_.]+$";
		}

		public override string TypeId
		{
			get
			{
				return "EmailAddress";
			}
		}
	}
}
