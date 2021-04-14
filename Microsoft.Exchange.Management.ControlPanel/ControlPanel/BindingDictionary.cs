using System;
using System.Collections.Generic;
using System.Text;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class BindingDictionary : Dictionary<string, Binding>
	{
		public string ToJavaScript(IControlResolver resolver)
		{
			StringBuilder stringBuilder = new StringBuilder("{");
			foreach (KeyValuePair<string, Binding> keyValuePair in this)
			{
				stringBuilder.Append('"');
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append("\":");
				stringBuilder.Append(keyValuePair.Value.ToJavaScript(resolver));
				stringBuilder.Append(",");
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
