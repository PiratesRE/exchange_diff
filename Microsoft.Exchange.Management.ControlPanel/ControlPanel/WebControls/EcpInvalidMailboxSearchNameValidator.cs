using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpInvalidMailboxSearchNameValidator : RegularExpressionValidator, IEcpValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.ValidationExpression = "^[^\\?\\*]+$";
		}

		public string DefaultErrorMessage
		{
			get
			{
				return Strings.MailboxSearchNameValidatorErrorMessage;
			}
		}

		public string TypeId
		{
			get
			{
				return "MailboxSearchName";
			}
		}
	}
}
