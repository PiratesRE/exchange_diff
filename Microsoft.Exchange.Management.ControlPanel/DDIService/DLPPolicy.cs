using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.FfoReporting;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class DLPPolicy
	{
		private static void AggregateData(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DLPPolicyReportingService dlppolicyReportingService = new DLPPolicyReportingService();
			PowerShellResults<MailTrafficPolicyReport> dlptrafficData = dlppolicyReportingService.GetDLPTrafficData(new DLPPolicyTrafficReportParameters
			{
				StartDate = new DateTime?((DateTime)ExDateTime.GetNow(EcpDateTimeHelper.GetCurrentUserTimeZone()).Subtract(new TimeSpan(48, 0, 0)).ToUtc()),
				EndDate = new DateTime?((DateTime)ExDateTime.GetNow(EcpDateTimeHelper.GetCurrentUserTimeZone()).ToUtc()),
				EventType = "DlpPolicyHits,DlpPolicyOverride,DlpPolicyFalsePositive",
				Direction = "Outbound"
			});
			if (dlptrafficData != null && dlptrafficData.Output != null)
			{
				IEnumerable<MailTrafficPolicyReport> source = from d in dlptrafficData.Output
				orderby d.Date.Date descending, d.Date.Hour descending
				select d;
				IEnumerable<MailTrafficPolicyReport> reportData = source.Take(24);
				IEnumerable<MailTrafficPolicyReport> trendData = source.Skip(24);
				foreach (object obj in dataTable.Rows)
				{
					DataRow row = (DataRow)obj;
					DLPPolicy.CalcuatePolicyData(row, "Name", "Hits", "HitsTrend", "DlpPolicyHits", reportData, trendData);
					DLPPolicy.CalcuatePolicyData(row, "Name", "Overrides", "OverridesTrend", "DlpPolicyOverride", reportData, trendData);
					DLPPolicy.CalcuatePolicyData(row, "Name", "FalsePositives", "FalsePositivesTrend", "DlpPolicyFalsePositive", reportData, trendData);
				}
			}
		}

		private static void CalcuatePolicyData(DataRow row, string policyNameField, string countField, string trendField, string eventType, IEnumerable<MailTrafficPolicyReport> reportData, IEnumerable<MailTrafficPolicyReport> trendData)
		{
			IEnumerable<MailTrafficPolicyReport> source = from d in reportData
			where d.DlpPolicy.Equals(row[policyNameField].ToString(), StringComparison.OrdinalIgnoreCase) && string.Compare(d.EventType, eventType, true) == 0
			select d;
			IEnumerable<MailTrafficPolicyReport> source2 = from d in trendData
			where d.DlpPolicy.Equals(row[policyNameField].ToString(), StringComparison.OrdinalIgnoreCase) && string.Compare(d.EventType, eventType, true) == 0
			select d;
			long num = (long)source.Sum((MailTrafficPolicyReport d) => d.MessageCount);
			long num2 = (long)source2.Sum((MailTrafficPolicyReport d) => d.MessageCount);
			row[countField] = num;
			row[trendField] = ((num == num2) ? 0 : ((num < num2) ? -1 : 1));
		}

		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (RbacPrincipal.Current.IsInRole("LiveID"))
			{
				DLPPolicy.AggregateData(inputRow, dataTable, store);
			}
		}

		public static string GetDisplayMode(string mode)
		{
			switch ((RuleMode)Enum.Parse(typeof(RuleMode), mode, true))
			{
			case RuleMode.Audit:
				return Strings.DLPTestPolicy;
			case RuleMode.Enforce:
				return Strings.DLPEnforcedPolicy;
			}
			return Strings.DLPTestAndAuditPolicy;
		}

		public static void NewPolicyPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			Hashtable hashtable = new Hashtable();
			if (!DDIHelper.IsEmptyValue(inputRow["ExceptionGroup"]))
			{
				hashtable["exceptionDL"] = ((Identity)inputRow["ExceptionGroup"]).DisplayName;
			}
			if (!DDIHelper.IsEmptyValue(inputRow["IncidentMailbox"]))
			{
				hashtable["IncidentManagementMailBox"] = ((Identity)inputRow["IncidentMailbox"]).DisplayName;
			}
			if (hashtable.Count > 0)
			{
				DataRow dataRow = dataTable.Rows[0];
				dataRow["Parameters"] = hashtable;
				store.ModifiedColumns.Add("Parameters");
			}
		}

		public static void GetTemplateListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (!DBNull.Value.Equals(dataRow["RuleParameters"]))
				{
					MultiValuedProperty<string> source = (MultiValuedProperty<string>)dataRow["RuleParameters"];
					dataRow["SupportsDistributionGroup"] = source.Any((string rp) => rp.Contains("%%exceptionDL%%"));
					dataRow["SupportsIncidentManagementMailbox"] = source.Any((string rp) => rp.Contains("%%IncidentManagementMailBox%%"));
				}
				else
				{
					dataRow["SupportsDistributionGroup"] = false;
					dataRow["SupportsIncidentManagementMailbox"] = false;
				}
			}
		}

		public static string GetCountryDisplayName(string locale)
		{
			if (string.IsNullOrEmpty(locale))
			{
				return string.Empty;
			}
			string result;
			try
			{
				LanguageList languageList = new LanguageList();
				result = RtlUtil.ConvertToDecodedBidiString(languageList.GetDisplayValue(locale), RtlUtil.IsRtl);
			}
			catch (Exception exception)
			{
				DDIHelper.Trace("Failed to locate locale {0}, Error: {1}", new object[]
				{
					locale,
					exception.GetTraceFormatter()
				});
				result = locale;
			}
			return result;
		}

		public static string ConvertToBidiString(string value)
		{
			return RtlUtil.ConvertToDecodedBidiString(value, RtlUtil.IsRtl);
		}

		private const int DLPPolicyReportingPeriod = 24;
	}
}
