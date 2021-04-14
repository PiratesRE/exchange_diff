using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics;
using Microsoft.Exchange.MailboxLoadBalance.Logging.SoftDeletedMailboxRemoval;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ObjectLogCollector
	{
		public ObjectLogCollector()
		{
			this.loggers[typeof(MailboxStatisticsLogEntry)] = MailboxStatisticsLog.CreateWithConfig(this.GetLogConfig("MailboxStatistics"));
			this.loggers[typeof(SoftDeletedMailboxRemovalLogEntry)] = SoftDeletedMailboxRemovalLog.CreateWithConfig(this.GetLogConfig("SoftDeletedRemoval"));
		}

		public virtual void LogObject<TObject>(TObject obj) where TObject : ConfigurableObject
		{
			ObjectLog<TObject> loggerInstance = this.GetLoggerInstance<TObject>();
			loggerInstance.LogObject(obj);
		}

		protected virtual ObjectLogConfiguration GetLogConfig(string logName)
		{
			return new LoadBalanceLoggingConfig(logName);
		}

		protected virtual ObjectLog<TObject> GetLoggerInstance<TObject>()
		{
			object obj;
			if (!this.loggers.TryGetValue(typeof(TObject), out obj))
			{
				throw new NotSupportedException("No known logger for objects of type " + typeof(TObject));
			}
			return (ObjectLog<TObject>)obj;
		}

		private readonly ConcurrentDictionary<Type, object> loggers = new ConcurrentDictionary<Type, object>();
	}
}
