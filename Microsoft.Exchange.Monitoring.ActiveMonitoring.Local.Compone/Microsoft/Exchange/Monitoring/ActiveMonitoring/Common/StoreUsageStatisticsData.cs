using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class StoreUsageStatisticsData
	{
		public StoreUsageStatisticsData(string digestCategory, int sampleId, DateTime sampleTime, string mailboxGuid, string displayName, int timeInServer, int ropCount, int logRecordBytes, int logRecordCount, int timeInCpu, int pageRead, int pagePreRead, int ldapReads, int ldapSearches, bool isMailboxQuarantined)
		{
			this.DigestCategory = digestCategory;
			this.SampleId = sampleId;
			this.SampleTime = sampleTime;
			this.MailboxGuid = mailboxGuid;
			this.DisplayName = displayName;
			this.TimeInServer = timeInServer;
			this.RopCount = ropCount;
			this.LogRecordBytes = logRecordBytes;
			this.LogRecordCount = logRecordCount;
			this.TimeInCPU = timeInCpu;
			this.PageRead = pageRead;
			this.PagePreread = pagePreRead;
			this.LdapReads = ldapReads;
			this.LdapSearches = ldapSearches;
			this.IsMailboxQuarantined = isMailboxQuarantined;
		}

		public string DigestCategory { get; private set; }

		public int SampleId { get; private set; }

		public DateTime SampleTime { get; private set; }

		public string MailboxGuid { get; private set; }

		public string DisplayName { get; private set; }

		public int TimeInServer { get; private set; }

		public int RopCount { get; private set; }

		public int LogRecordBytes { get; private set; }

		public int LogRecordCount { get; private set; }

		public int TimeInCPU { get; private set; }

		public int PageRead { get; private set; }

		public int PagePreread { get; private set; }

		public int LdapReads { get; private set; }

		public int LdapSearches { get; private set; }

		public bool IsMailboxQuarantined { get; private set; }
	}
}
