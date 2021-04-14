using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SessionStatisticsLogData
	{
		internal SessionStatisticsLogData(Guid requestGuid, SessionStatistics sessionStatistics, SessionStatistics archiveSessionStatistics)
		{
			this.RequestGuid = requestGuid;
			this.SessionStatistics = sessionStatistics;
			this.ArchiveSessionStatistics = archiveSessionStatistics;
		}

		public readonly SessionStatistics SessionStatistics;

		public readonly SessionStatistics ArchiveSessionStatistics;

		public readonly Guid RequestGuid;
	}
}
