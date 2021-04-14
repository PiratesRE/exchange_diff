using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	[Serializable]
	public class ResponseTrackerItem
	{
		internal bool Matches(TestId? testId, RequestTarget? requestTarget, HttpWebRequestWrapper request)
		{
			return (testId == null || this.StepId == testId.ToString()) && (requestTarget == null || this.TargetType == requestTarget.ToString()) && this.TargetHost == request.RequestUri.Host && this.PathAndQuery == request.RequestUri.PathAndQuery;
		}

		internal void AppendSummary(bool useCsvFormat, SummaryHeader[] headers, StringBuilder stringBuilder)
		{
			if (stringBuilder.Length == 0)
			{
				foreach (SummaryHeader summaryHeader in headers)
				{
					summaryHeader.Append(useCsvFormat, stringBuilder, summaryHeader.HeaderTitle);
				}
			}
			stringBuilder.Append(Environment.NewLine);
			int num = 1;
			Dictionary<SummaryHeader, string[]> dictionary = new Dictionary<SummaryHeader, string[]>();
			foreach (SummaryHeader summaryHeader2 in headers)
			{
				string[] array = summaryHeader2.ValueExtractionDelegate(this);
				if (array != null && array.Length > 0)
				{
					if (array.Length > 1)
					{
						num = ((array.Length > num) ? array.Length : num);
						dictionary.Add(summaryHeader2, array);
					}
					summaryHeader2.Append(useCsvFormat, stringBuilder, array[0]);
				}
			}
			for (int k = 1; k < num; k++)
			{
				stringBuilder.Append(Environment.NewLine);
				foreach (SummaryHeader summaryHeader3 in headers)
				{
					string itemToLog = string.Empty;
					string[] array2;
					if (dictionary.TryGetValue(summaryHeader3, out array2) && array2.Length > k)
					{
						itemToLog = array2[k];
					}
					summaryHeader3.Append(useCsvFormat, stringBuilder, itemToLog);
				}
			}
		}

		public int Index;

		public string StepId;

		public string TargetHost;

		public string TargetIpAddress;

		public string TargetType;

		public string PathAndQuery;

		public string RespondingServer;

		public string MailboxServer;

		public string DomainController;

		public string ARRServer;

		public string FailingServer;

		public string FailingTargetHostname;

		public string FailingTargetIPAddress;

		public int? FailureHttpResponseCode;

		public TimeSpan ResponseLatency;

		public TimeSpan TotalLatency;

		public TimeSpan? CasLatency;

		public TimeSpan? RpcLatency;

		public TimeSpan? LdapLatency;

		public TimeSpan? MservLatency;

		public TimeSpan DnsLatency;

		public string TargetVipName;

		public string TargetVipForestName;

		public long ContentLength;

		public string FailedIpAddresses;

		public bool? IsE14CasServer;

		[XmlIgnore]
		[NonSerialized]
		internal HttpWebResponseWrapper Response;

		[XmlIgnore]
		[NonSerialized]
		internal ConcurrentDictionary<NamedVip, Exception> IpAddressListFailureList;
	}
}
