using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging
{
	internal class DatabaseRequestLog : ObjectLog<RequestDiagnosticData>
	{
		private DatabaseRequestLog() : base(new DatabaseRequestLog.RequestLogSchema(), new LoadBalanceLoggingConfig("Requests"))
		{
		}

		public static void Write(IRequest request)
		{
			DatabaseRequestLog.Instance.LogObject(request.GetDiagnosticData(false));
		}

		private static readonly DatabaseRequestLog Instance = new DatabaseRequestLog();

		private class RequestLogData : ConfigurableObject
		{
			public RequestLogData(PropertyBag propertyBag) : base(propertyBag)
			{
			}

			internal override ObjectSchema ObjectSchema
			{
				get
				{
					return new DummyObjectSchema();
				}
			}
		}

		private class RequestLogSchema : ConfigurableObjectLogSchema<DatabaseRequestLog.RequestLogData, DummyObjectSchema>
		{
			public override string LogType
			{
				get
				{
					return "Database Requests";
				}
			}

			public override string Software
			{
				get
				{
					return "Mailbox Load Balancing";
				}
			}

			private static string ComputeFailureHash(Exception failureException)
			{
				string result;
				WatsonExceptionReport.TryStringHashFromStackTrace(failureException, false, out result);
				return result;
			}

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> BatchName = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("BatchName", (RequestDiagnosticData r) => r.BatchName ?? string.Empty);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> Exception = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("Exception", delegate(RequestDiagnosticData r)
			{
				if (r.Exception != null)
				{
					return r.Exception.ToString();
				}
				return string.Empty;
			});

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> ExceptionType = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("ExceptionType", delegate(RequestDiagnosticData r)
			{
				if (r.Exception != null)
				{
					return r.Exception.GetType().Name;
				}
				return string.Empty;
			});

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> ExecutionDuration = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("ExecutionDurationMs", (RequestDiagnosticData r) => r.ExecutionDuration.TotalMilliseconds);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> ExecutionFinishedTimestamp = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("FinishTS", (RequestDiagnosticData r) => r.ExecutionFinishedTimestamp);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> ExecutionStartTimestamp = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("StartTS", (RequestDiagnosticData r) => r.ExecutionStartedTimestamp);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> MailboxGuid = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("MailboxGuid", (RequestDiagnosticData r) => r.MovedMailboxGuid);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> QueueDuration = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("QueueDurationMs", (RequestDiagnosticData r) => r.QueueDuration.TotalMilliseconds);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> QueuedTimestamp = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("QueueTS", (RequestDiagnosticData r) => r.QueuedTimestamp);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> RealApplicationVersion = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("RealApplicationVersion", (RequestDiagnosticData r) => ExWatson.RealApplicationVersion.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> RequestQueue = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("Queue", (RequestDiagnosticData r) => r.Queue);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> RequestType = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("Type", (RequestDiagnosticData r) => r.RequestKind);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> Result = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("Result", (RequestDiagnosticData r) => r.Result);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> SourceDatabaseName = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("SourceDatabaseName", (RequestDiagnosticData r) => r.SourceDatabaseName);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> TargetDatabaseName = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("TargetDatabaseName", (RequestDiagnosticData r) => r.TargetDatabaseName);

			public static readonly ObjectLogSimplePropertyDefinition<RequestDiagnosticData> WatsonHash = new ObjectLogSimplePropertyDefinition<RequestDiagnosticData>("WatsonHash", delegate(RequestDiagnosticData r)
			{
				if (r.Exception != null)
				{
					return DatabaseRequestLog.RequestLogSchema.ComputeFailureHash(r.Exception);
				}
				return string.Empty;
			});
		}
	}
}
