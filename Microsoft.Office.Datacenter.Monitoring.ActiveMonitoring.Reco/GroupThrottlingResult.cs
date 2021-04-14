using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal class GroupThrottlingResult
	{
		internal bool IsPassed { get; set; }

		internal int TotalRequestsSent { get; set; }

		internal int TotalRequestsSucceeded { get; set; }

		internal RecoveryActionHelper.RecoveryActionEntrySerializable MostRecentEntry { get; set; }

		internal int MinimumMinutes { get; set; }

		internal int TotalInOneDay { get; set; }

		internal int MaxAllowedInOneDay { get; set; }

		internal string[] ThrottlingInProgressServers { get; set; }

		internal string[] RecoveryInProgressServers { get; set; }

		internal string ChecksFailed { get; set; }

		internal DateTime TimeToRetryAfter { get; set; }

		internal string Comment { get; set; }

		internal Dictionary<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics> GroupStats { get; set; }

		internal ConcurrentDictionary<string, Exception> ExceptionsByServer { get; set; }

		internal string ToXml(bool isForce = false)
		{
			if (!isForce && this.xml != null)
			{
				return this.xml;
			}
			string value = string.Empty;
			if (this.ThrottlingInProgressServers != null)
			{
				value = string.Join(",", this.ThrottlingInProgressServers);
			}
			string value2 = string.Empty;
			if (this.RecoveryInProgressServers != null)
			{
				value2 = string.Join(",", this.RecoveryInProgressServers);
			}
			XElement xelement = new XElement("GroupThrottlingResult", new object[]
			{
				new XAttribute("IsPassed", this.IsPassed),
				new XAttribute("TotalRequestsSent", this.TotalRequestsSent),
				new XAttribute("TotalRequestsSucceeded", this.TotalRequestsSucceeded),
				new XAttribute("MinimumMinutes", this.MinimumMinutes),
				new XAttribute("TotalInOneDay", this.TotalInOneDay),
				new XAttribute("MaxAllowedInOneDay", this.MaxAllowedInOneDay),
				new XAttribute("ThrottlingInProgressServers", value),
				new XAttribute("RecoveryInProgressServers", value2),
				new XAttribute("ChecksFailed", (this.ChecksFailed != null) ? this.ChecksFailed : string.Empty),
				new XAttribute("TimeToRetryAfter", this.TimeToRetryAfter.ToString("o")),
				new XAttribute("Comment", (this.Comment != null) ? this.Comment : string.Empty)
			});
			if (this.MostRecentEntry != null)
			{
				xelement.Add(new XElement("MostRecentEntry", new object[]
				{
					new XAttribute("Requester", this.MostRecentEntry.RequestorName),
					new XAttribute("StartTime", this.MostRecentEntry.StartTime),
					new XAttribute("EndTime", this.MostRecentEntry.EndTime),
					new XAttribute("State", this.MostRecentEntry.State),
					new XAttribute("Result", this.MostRecentEntry.Result)
				}));
			}
			XElement xelement2 = new XElement("ServerStats");
			if (this.GroupStats != null)
			{
				foreach (KeyValuePair<string, RpcGetThrottlingStatisticsImpl.ThrottlingStatistics> keyValuePair in this.GroupStats)
				{
					string key = keyValuePair.Key;
					RpcGetThrottlingStatisticsImpl.ThrottlingStatistics value3 = keyValuePair.Value;
					string text = null;
					Exception ex = null;
					if (this.ExceptionsByServer.TryGetValue(key, out ex) && ex != null)
					{
						text = ex.ToString().Replace("Microsoft.Exchange.Monitoring.ActiveMonitoring", "M.E.M.A");
						text = text.Substring(0, Math.Min(text.Length, 500));
					}
					XElement xelement3 = new XElement(key);
					if (!string.IsNullOrEmpty(text))
					{
						xelement3.Add(new XAttribute("Error", text));
					}
					RpcGetThrottlingStatisticsImpl.ThrottlingStatistics throttlingStatistics;
					if (this.GroupStats.TryGetValue(key, out throttlingStatistics) && throttlingStatistics != null)
					{
						xelement3.Add(new XAttribute("TotalSearched", throttlingStatistics.TotalEntriesSearched));
						xelement3.Add(new XAttribute("MostRecentEntryStartTimeUtc", (throttlingStatistics.MostRecentEntry != null) ? throttlingStatistics.MostRecentEntry.StartTimeUtc : DateTime.MinValue));
						xelement3.Add(new XAttribute("MostRecentEntryEndTimeUtc", (throttlingStatistics.MostRecentEntry != null) ? throttlingStatistics.MostRecentEntry.EndTimeUtc : DateTime.MinValue));
						xelement3.Add(new XAttribute("TotalActionsInADay", throttlingStatistics.NumberOfActionsInOneDay));
						xelement3.Add(new XAttribute("IsThrottlingInProgress", throttlingStatistics.IsThrottlingInProgress));
						xelement3.Add(new XAttribute("IsRecoveryInProgress", throttlingStatistics.IsRecoveryInProgress));
						xelement3.Add(new XAttribute("HostProcessStartTimeUtc", throttlingStatistics.HostProcessStartTimeUtc));
						xelement3.Add(new XAttribute("SystemBootTimeUtc", throttlingStatistics.SystemBootTimeUtc));
					}
					xelement2.Add(xelement3);
				}
			}
			xelement.Add(xelement2);
			this.xml = xelement.ToString();
			return this.xml;
		}

		private string xml;
	}
}
