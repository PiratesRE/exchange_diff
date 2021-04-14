using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.Optics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class StorageLoggerBase : IExtensibleLogger, IWorkloadLogger
	{
		internal StorageLoggerBase(IExtensibleLogger logger)
		{
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.logger = logger;
			this.ActivityId = (ActivityContext.ActivityId ?? Guid.NewGuid());
		}

		protected abstract string TenantName { get; }

		protected abstract Guid MailboxGuid { get; }

		private protected Guid ActivityId { protected get; private set; }

		public void LogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			this.logger.LogActivityEvent(activityScope, eventType);
		}

		public virtual void LogEvent(ILogEvent logEvent)
		{
			ArgumentValidator.ThrowIfNull("logEvent", logEvent);
			this.AppendEventData(logEvent.GetEventData());
			this.logger.LogEvent(logEvent);
		}

		public void LogEvent(IEnumerable<ILogEvent> logEvents)
		{
			ArgumentValidator.ThrowIfNull("logEvents", logEvents);
			foreach (ILogEvent logEvent in logEvents)
			{
				this.LogEvent(logEvent);
			}
		}

		protected virtual void AppendEventData(ICollection<KeyValuePair<string, object>> eventData)
		{
			if (!string.IsNullOrEmpty(this.TenantName))
			{
				eventData.Add(new KeyValuePair<string, object>(StorageLoggerBase.TenantNameFieldName, this.TenantName));
			}
			eventData.Add(new KeyValuePair<string, object>(StorageLoggerBase.MailboxGuidFieldName, this.MailboxGuid));
			eventData.Add(new KeyValuePair<string, object>(StorageLoggerBase.ActivityIdFieldName, this.ActivityId));
		}

		internal static readonly string TenantNameFieldName = "TenantName";

		internal static readonly string MailboxGuidFieldName = "MailboxGuid";

		internal static readonly string ActivityIdFieldName = "ActivityId";

		private readonly IExtensibleLogger logger;
	}
}
