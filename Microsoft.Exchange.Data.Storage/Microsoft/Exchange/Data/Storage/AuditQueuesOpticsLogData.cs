using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditQueuesOpticsLogData
	{
		public OrganizationId OrganizationId { get; set; }

		public AuditQueueType QueueType { get; set; }

		public QueueEventType EventType { get; set; }

		public string CorrelationId { get; set; }

		public int QueueLength { get; set; }

		public void Log()
		{
			AuditingOpticsLogger.LogAuditOpticsEntry(AuditingOpticsLoggerType.AuditQueues, AuditingOpticsLogger.GetLogColumns<AuditQueuesOpticsLogData>(this, AuditQueuesOpticsLogData.LogSchema));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AuditQueuesOpticsLogData()
		{
			LogTableSchema<AuditQueuesOpticsLogData>[] array = new LogTableSchema<AuditQueuesOpticsLogData>[5];
			array[0] = new LogTableSchema<AuditQueuesOpticsLogData>("Tenant", delegate(AuditQueuesOpticsLogData data)
			{
				if (!(data.OrganizationId == null))
				{
					return data.OrganizationId.ToString();
				}
				return "First Org";
			});
			array[1] = new LogTableSchema<AuditQueuesOpticsLogData>("QueueType", (AuditQueuesOpticsLogData data) => data.QueueType.ToString());
			array[2] = new LogTableSchema<AuditQueuesOpticsLogData>("EventType", (AuditQueuesOpticsLogData data) => data.EventType.ToString());
			array[3] = new LogTableSchema<AuditQueuesOpticsLogData>("QueueLength", (AuditQueuesOpticsLogData data) => data.QueueLength.ToString(CultureInfo.InvariantCulture));
			array[4] = new LogTableSchema<AuditQueuesOpticsLogData>("CorrelationId", (AuditQueuesOpticsLogData data) => data.CorrelationId);
			AuditQueuesOpticsLogData.LogSchema = array;
		}

		internal static LogTableSchema<AuditQueuesOpticsLogData>[] LogSchema;
	}
}
