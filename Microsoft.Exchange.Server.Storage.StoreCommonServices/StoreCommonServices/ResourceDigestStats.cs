using System;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct ResourceDigestStats
	{
		public ResourceDigestStats(ref JET_THREADSTATS databaseThreadStats)
		{
			this.PageRead = databaseThreadStats.cPageRead;
			this.PagePreread = databaseThreadStats.cPagePreread;
			this.LogRecordCount = databaseThreadStats.cLogRecord;
			this.LogRecordBytes = ((databaseThreadStats.cbLogRecord >= 0) ? databaseThreadStats.cbLogRecord : int.MaxValue);
			this.MailboxGuid = default(Guid);
			this.MailboxNumber = 0;
			this.TimeInServer = default(TimeSpan);
			this.TimeInCPU = default(TimeSpan);
			this.ROPCount = 0;
			this.LdapReads = 0;
			this.LdapSearches = 0;
			this.SampleTime = default(DateTime);
		}

		public Guid MailboxGuid;

		public int MailboxNumber;

		public TimeSpan TimeInServer;

		public TimeSpan TimeInCPU;

		public int ROPCount;

		public int PageRead;

		public int PagePreread;

		public int LogRecordCount;

		public int LogRecordBytes;

		public int LdapReads;

		public int LdapSearches;

		public DateTime SampleTime;
	}
}
