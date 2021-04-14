using System;
using Microsoft.Exchange.Data.Storage.Auditing;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class AsyncAuditLogger : IAuditLog
	{
		public AsyncAuditLogger(IAuditLog auditLogger)
		{
			this.realAuditLogger = auditLogger;
		}

		public DateTime EstimatedLogStartTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		public DateTime EstimatedLogEndTime
		{
			get
			{
				return DateTime.MaxValue;
			}
		}

		public bool IsAsynchronous
		{
			get
			{
				return true;
			}
		}

		public IAuditQueryContext<TFilter> CreateAuditQueryContext<TFilter>()
		{
			throw new InvalidOperationException();
		}

		public int WriteAuditRecord(IAuditLogRecord auditRecord)
		{
			IQueue<AuditData> queue = QueueFactory.GetQueue<AuditData>(Queues.AdminAuditingMainQueue);
			AuditData data = new AuditData
			{
				AuditRecord = auditRecord,
				AuditLogger = this.realAuditLogger
			};
			queue.Send(data);
			return 0;
		}

		private readonly IAuditLog realAuditLogger;
	}
}
