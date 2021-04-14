using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class BindingCollection : Collection<Binding>
	{
		public string ToJavaScript(IControlResolver resolver)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			stringBuilder.Append(string.Join(",", (from o in this
			select o.ToJavaScript(resolver)).ToArray<string>()));
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}
