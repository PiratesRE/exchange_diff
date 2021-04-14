using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpCustomValidator : CustomValidator, IEcpValidator
	{
		public string DefaultErrorMessage { get; set; }

		public virtual string TypeId
		{
			get
			{
				return "custom";
			}
		}
	}
}
