using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Optics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DiagnosticsFrame : DisposableFrame, IDiagnosticsFrame, IForceReportDisposeTrackable, IDisposeTrackable, IDisposable
	{
		public DiagnosticsFrame(string operationContext, string operationName, ITracer tracer, IExtensibleLogger logger, IMailboxPerformanceTracker performanceTracker) : base(null)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("operationContext", operationContext);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("operationName", operationName);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfNull("logger", logger);
			ArgumentValidator.ThrowIfNull("performanceTracker", performanceTracker);
			this.operationContext = operationContext;
			this.operationName = operationName;
			this.tracer = tracer;
			this.logger = logger;
			this.performanceTracker = performanceTracker;
			this.Start();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DiagnosticsFrame>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Finish();
			}
			base.InternalDispose(disposing);
		}

		private void Start()
		{
			this.tracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: Starting {1}.", this.operationContext, this.operationName);
			this.logger.LogEvent(new SchemaBasedLogEvent<DiagnosticsFrame.OperationStart>
			{
				{
					DiagnosticsFrame.OperationStart.OperationName,
					this.operationName
				}
			});
			this.performanceTracker.Start();
		}

		private void Finish()
		{
			this.performanceTracker.Stop();
			this.logger.LogEvent(this.performanceTracker.GetLogEvent(this.operationName));
			this.tracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: Finishing {1}.", this.operationContext, this.operationName);
		}

		private readonly string operationContext;

		private readonly string operationName;

		private readonly ITracer tracer;

		private readonly IExtensibleLogger logger;

		private readonly IMailboxPerformanceTracker performanceTracker;

		private enum OperationStart
		{
			OperationName
		}
	}
}
