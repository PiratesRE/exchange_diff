using System;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class ProbeHelper
	{
		public static string GetExtensionAttribute(IHygieneLogger logger, ProbeWorkItem workItem, string key)
		{
			string text = null;
			if (workItem.Definition.Attributes.ContainsKey(key))
			{
				text = workItem.Definition.Attributes[key];
			}
			else
			{
				logger.LogError(string.Format("Cannot find value for key '{0}'", key));
			}
			logger.LogVerbose(string.Format("GET_ATTR -- key='{0}', val='{1}' \n", key, text));
			return text;
		}

		internal static void ModifyResultName(ProbeResult probeResult)
		{
			probeResult.ResultName = ProbeHelper.ModifyResultName(probeResult.ResultName);
		}

		internal static string ModifyResultName(string resultName)
		{
			if (string.IsNullOrWhiteSpace(resultName) || resultName.StartsWith("/"))
			{
				return null;
			}
			int num = resultName.IndexOf('/');
			string text = (num > 0) ? resultName.Substring(0, num) : resultName;
			char[] array = ((text.Length <= 5) ? text : text.Substring(text.Length - 5, 5)).ToCharArray();
			Array.Reverse(array);
			string newValue = "Inter" + text.Substring(0, text.Length - 5) + new string(array);
			return resultName.Replace(text, newValue);
		}
	}
}
