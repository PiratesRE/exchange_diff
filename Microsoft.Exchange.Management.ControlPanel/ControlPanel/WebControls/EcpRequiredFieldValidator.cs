using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpRequiredFieldValidator : RequiredFieldValidator, IEcpValidator
	{
		public string DefaultErrorMessage
		{
			get
			{
				return Strings.RequiredFieldValidatorErrorMessage;
			}
		}

		public string TypeId
		{
			get
			{
				return "ReqFld";
			}
		}
	}
}
