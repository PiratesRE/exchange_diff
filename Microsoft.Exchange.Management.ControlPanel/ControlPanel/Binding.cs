using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class Binding
	{
		public string Name { get; set; }

		public abstract string ToJavaScript(IControlResolver resolver);
	}
}
