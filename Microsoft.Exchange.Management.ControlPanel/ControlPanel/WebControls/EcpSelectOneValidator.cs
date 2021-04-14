using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpSelectOneValidator : RequiredFieldValidator, IEcpValidator
	{
		public string DefaultErrorMessage
		{
			get
			{
				return Strings.SelectOneValidatorErrorMessage;
			}
		}

		public string TypeId
		{
			get
			{
				return "SelOne";
			}
		}
	}
}
