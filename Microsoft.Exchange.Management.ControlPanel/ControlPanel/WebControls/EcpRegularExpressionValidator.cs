using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpRegularExpressionValidator : RegularExpressionValidator, IEcpValidator
	{
		public EcpRegularExpressionValidator()
		{
			this.DefaultErrorMessage = Strings.RegexValidatorErrorMessage;
		}

		public virtual string DefaultErrorMessage { get; set; }

		public virtual string TypeId
		{
			get
			{
				return "Regex";
			}
		}
	}
}
