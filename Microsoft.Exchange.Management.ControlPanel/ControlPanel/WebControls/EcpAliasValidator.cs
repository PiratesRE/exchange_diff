using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpAliasValidator : EcpRegularExpressionValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ValidationExpression = "^[!#%&'=`~\\$\\*\\+\\-\\/\\?\\^\\{\\|\\}a-zA-Z0-9_\\u00A1-\\u00FF]+(\\.[!#%&'=`~\\$\\*\\+\\-\\/\\?\\^\\{\\|\\}a-zA-Z0-9_\\u00A1-\\u00FF]+)*$";
		}

		public override string TypeId
		{
			get
			{
				return "Alias";
			}
		}
	}
}
