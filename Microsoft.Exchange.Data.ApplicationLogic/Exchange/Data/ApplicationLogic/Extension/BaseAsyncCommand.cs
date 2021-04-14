using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal abstract class BaseAsyncCommand
	{
		public BaseAsyncCommand(string scenario)
		{
			this.scenario = scenario;
			this.ResetRequestID();
		}

		protected void ExecuteWithExceptionHandler(GrayException.UserCodeDelegate callback)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(callback);
			}
			catch (GrayException exception)
			{
				this.InternalFailureCallback(exception, null);
			}
		}

		protected virtual void InternalFailureCallback(Exception exception = null, string traceMessage = null)
		{
			if (exception == null && traceMessage == null)
			{
				throw new ArgumentNullException("exception", "exception or traceMessage must be non-null");
			}
			if (exception != null)
			{
				BaseAsyncCommand.Tracer.TraceError<Uri, Exception>(0L, "BaseAsyncOmexCommand.InternalFailureCallback: Request: {0} Exception: {1}", this.uri, exception);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_RequestFailed, this.periodicKey, new object[]
				{
					this.scenario,
					this.requestId,
					this.GetLoggedMailboxIdentifier(),
					this.uri,
					ExtensionDiagnostics.GetLoggedExceptionString(exception)
				});
			}
			else
			{
				BaseAsyncCommand.Tracer.TraceError<Uri, string>(0L, "BaseAsyncOmexCommand.InternalFailureCallback: Request: {0} Message: {1}", this.uri, traceMessage);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_RequestFailed, this.periodicKey, new object[]
				{
					this.scenario,
					this.requestId,
					this.GetLoggedMailboxIdentifier(),
					this.uri,
					traceMessage
				});
			}
			this.failureCallback(exception);
		}

		protected virtual void LogResponseParseFailureEvent(ExEventLog.EventTuple eventTuple, string periodicKey, object messageArg)
		{
			ExtensionDiagnostics.Logger.LogEvent(eventTuple, this.periodicKey, new object[]
			{
				this.scenario,
				this,
				this.requestId,
				this.GetLoggedMailboxIdentifier(),
				this.uri,
				messageArg
			});
		}

		protected string GetLoggedMailboxIdentifier()
		{
			string result = null;
			if (this.getLoggedMailboxIdentifierCallback != null)
			{
				result = this.getLoggedMailboxIdentifierCallback();
			}
			return result;
		}

		public void ResetRequestID()
		{
			this.requestId = Guid.NewGuid().ToString();
		}

		protected static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		protected string periodicKey;

		protected Uri uri;

		protected string scenario;

		protected string requestId;

		protected BaseAsyncCommand.FailureCallback failureCallback;

		protected BaseAsyncCommand.GetLoggedMailboxIdentifierCallback getLoggedMailboxIdentifierCallback;

		internal delegate void LogResponseParseFailureEventCallback(ExEventLog.EventTuple eventTuple, string periodicKey, object messageArg);

		internal delegate string GetLoggedMailboxIdentifierCallback();

		internal delegate void FailureCallback(Exception exception);
	}
}
