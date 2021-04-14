using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ClientControlBinding : ControlBinding
	{
		public ClientControlBinding(Control control, string clientPropertyName) : base(control)
		{
			this.ClientPropertyName = clientPropertyName;
		}

		private protected string ClientPropertyName { protected get; private set; }

		protected override string ToJavaScriptWhenVisible(IControlResolver resolver)
		{
			return string.Format("new ClientControlBinding('{0}','{1}')", this.ClientID, this.ClientPropertyName);
		}
	}
}
