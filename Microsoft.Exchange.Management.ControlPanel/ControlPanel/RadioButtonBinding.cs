using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RadioButtonBinding : ClientControlBinding
	{
		public RadioButtonBinding(Control control, string clientPropertyName) : base(control, clientPropertyName)
		{
		}

		protected override string ToJavaScriptWhenVisible(IControlResolver resolver)
		{
			return string.Format("new RadioButtonBinding('{0}','{1}')", this.ClientID, base.ClientPropertyName);
		}
	}
}
