using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class FailureObjectLog
	{
		public FailureObjectLog(ObjectLogConfiguration logConfig)
		{
			this.failureLogger = new ObjectLog<FailureObjectLog.FailureObjectLogEntry>(new FailureObjectLog.FailureObjectLogSchema(), logConfig);
		}

		public static LogSchema GetLogSchema()
		{
			return ObjectLog<FailureObjectLog.FailureObjectLogEntry>.GetLogSchema(new FailureObjectLog.FailureObjectLogSchema());
		}

		public void LogFailureEvent(IFailureObjectLoggable failureObject, Exception failureException)
		{
			if (failureException != null && failureObject != null)
			{
				FailureObjectLog.FailureObjectLogEntry failureObjectLogEntry = new FailureObjectLog.FailureObjectLogEntry(failureObject, this.ExtractExceptionString(failureException), this.ComputeFailureHash(failureException), failureException.GetType().Name, default(Guid), 0);
				this.failureLogger.LogObject(failureObjectLogEntry);
				while (failureException.InnerException != null)
				{
					failureException = failureException.InnerException;
					failureObjectLogEntry = new FailureObjectLog.FailureObjectLogEntry(failureObject, this.ExtractExceptionString(failureException), this.ComputeFailureHash(failureException), failureException.GetType().Name, failureObjectLogEntry.FailureGuid, failureObjectLogEntry.FailureLevel + 1);
					this.failureLogger.LogObject(failureObjectLogEntry);
				}
			}
		}

		public virtual string ComputeFailureHash(Exception failureException)
		{
			string result;
			WatsonExceptionReport.TryStringHashFromStackTrace(failureException, false, out result);
			return result;
		}

		public virtual string ExtractExceptionString(Exception failureException)
		{
			return failureException.ToString();
		}

		private ObjectLog<FailureObjectLog.FailureObjectLogEntry> failureLogger;

		private class FailureObjectLogEntry
		{
			public FailureObjectLogEntry(IFailureObjectLoggable failureObject, string ex, string failureHash, string exType, Guid failureGuid = default(Guid), int failureLevel = 0)
			{
				this.FailureObject = failureObject;
				this.FailureException = ex;
				this.FailureExceptionHash = failureHash;
				this.FailureExceptionType = exType;
				this.FailureGuid = ((failureGuid == default(Guid)) ? Guid.NewGuid() : failureGuid);
				this.FailureLevel = failureLevel;
			}

			public readonly IFailureObjectLoggable FailureObject;

			public readonly Guid FailureGuid;

			public readonly int FailureLevel;

			public readonly string FailureException;

			public readonly string FailureExceptionHash;

			public readonly string FailureExceptionType;
		}

		private class FailureObjectLogSchema : ObjectLogSchema
		{
			public override string LogType
			{
				get
				{
					return "Failure Object Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> FailureGuid = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("FailureGuid", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureGuid);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> FailureLevel = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("FailureLevel", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureLevel);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> ApplicationVersion = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("ApplicationVersion", (FailureObjectLog.FailureObjectLogEntry d) => ExWatson.RealApplicationVersion.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> ObjectGuid = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("ObjectGuid", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureObject.ObjectGuid);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> ObjectType = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("ObjectType", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureObject.ObjectType);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> Flags = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("Flags", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureObject.Flags);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> FailureContext = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("FailureContext", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureObject.FailureContext);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> ExceptionFailureType = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("ExceptionFailureType", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureExceptionType);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> ExceptionHash = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("ExceptionHash", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureExceptionHash);

			public static readonly ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry> ExceptionMessage = new ObjectLogSimplePropertyDefinition<FailureObjectLog.FailureObjectLogEntry>("ExceptionMessage", (FailureObjectLog.FailureObjectLogEntry d) => d.FailureException);
		}
	}
}
