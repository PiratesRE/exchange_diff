using System;
using System.Collections;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EcpTraceHelper
	{
		public static string GetTraceString(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			string result;
			if (obj is Exception)
			{
				result = (obj as Exception).ToTraceString();
			}
			else if (obj is PSCommand)
			{
				result = (obj as PSCommand).ToTraceString();
			}
			else if (obj is ErrorRecord[])
			{
				result = (obj as ErrorRecord[]).ToTraceString();
			}
			else if (obj is PowerShellResults<JsonDictionary<object>>)
			{
				result = (obj as PowerShellResults<JsonDictionary<object>>).ToTraceString();
			}
			else if (obj is IDictionary)
			{
				result = (obj as IDictionary).ToTraceString();
			}
			else if (obj is IEnumerable)
			{
				result = (obj as IEnumerable).ToTraceString();
			}
			else
			{
				result = obj.ToString();
			}
			return result;
		}
	}
}
