using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpCompareValidator : CompareValidator, IEcpValidator
	{
		public string DefaultErrorMessage
		{
			get
			{
				return this.defaultErrorMessage;
			}
			set
			{
				this.defaultErrorMessage = value;
			}
		}

		public string TypeId
		{
			get
			{
				return "CompVal";
			}
		}

		private string defaultErrorMessage = Strings.CompareValidatorMessage;
	}
}
