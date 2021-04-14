using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpRangeValidator : RangeValidator, IEcpValidator
	{
		public string DefaultErrorMessage
		{
			get
			{
				return string.Format(Strings.RangeValidatorErrorMessage, base.MinimumValue, base.MaximumValue);
			}
		}

		public virtual string TypeId
		{
			get
			{
				return "Range";
			}
		}
	}
}
