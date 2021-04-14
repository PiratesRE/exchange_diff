using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AjaxUploaderBinding : ComponentBinding
	{
		public AjaxUploaderBinding(Control control, string clientPropertyName) : base(control, clientPropertyName)
		{
		}

		protected override string ToJavaScriptWhenVisible(IControlResolver resolver)
		{
			return string.Format("new AjaxUploaderBinding('{0}','{1}')", base.Control.ClientID, base.ClientPropertyName);
		}
	}
}
