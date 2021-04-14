using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ComboBoxBinding : ClientControlBinding
	{
		public ComboBoxBinding(Control control, string clientPropertyName) : base(control, clientPropertyName)
		{
		}

		protected override string ToJavaScriptWhenVisible(IControlResolver resolver)
		{
			return string.Format("new ComboBoxBinding('{0}','{1}')", this.ClientID, base.ClientPropertyName);
		}
	}
}
