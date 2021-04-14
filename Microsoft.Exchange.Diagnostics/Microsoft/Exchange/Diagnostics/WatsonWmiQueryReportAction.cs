using System;
using System.Management;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	internal class WatsonWmiQueryReportAction : WatsonReportAction
	{
		public WatsonWmiQueryReportAction(string query) : base(query, false)
		{
		}

		public override string ActionName
		{
			get
			{
				return "WMI Query";
			}
		}

		public override string Evaluate(WatsonReport watsonReport)
		{
			string text = string.Concat(new string[]
			{
				"--- ",
				base.Expression,
				" ---\r\n",
				WatsonWmiQueryReportAction.EvaluateWmiQuery(base.Expression),
				"--- END ---"
			});
			watsonReport.LogExtraData(text);
			return text;
		}

		private static string EvaluateWmiQuery(string query)
		{
			string result;
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							stringBuilder.AppendLine(managementObject.ToString());
						}
					}
					result = stringBuilder.ToString();
				}
			}
			catch (Exception ex)
			{
				result = string.Concat(new string[]
				{
					"Error evaluating WMI query: ",
					ex.GetType().Name,
					" (",
					ex.Message,
					")\r\n"
				});
			}
			return result;
		}
	}
}
