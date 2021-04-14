using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationAggregationDiagnosticsFrame : IConversationAggregationDiagnosticsFrame
	{
		public ConversationAggregationDiagnosticsFrame(IMailboxSession session)
		{
			this.session = session;
		}

		public ConversationAggregationResult TrackAggregation(string operationName, AggregationDelegate aggregation)
		{
			ConversationAggregationResult result;
			using (this.CreateDiagnosticsFrame("ConversationAggregation", operationName))
			{
				try
				{
					result = aggregation(this.Logger);
				}
				catch (Exception value)
				{
					this.Logger.LogEvent(new SchemaBasedLogEvent<ConversationAggregationLogSchema.Error>
					{
						{
							ConversationAggregationLogSchema.Error.Context,
							base.GetType().Name
						},
						{
							ConversationAggregationLogSchema.Error.Exception,
							value
						}
					});
					throw;
				}
			}
			return result;
		}

		private IDiagnosticsFrame CreateDiagnosticsFrame(string operationContext, string operationName)
		{
			return new DiagnosticsFrame(operationContext, operationName, ConversationAggregationDiagnosticsFrame.Tracer, this.Logger, this.CreatePerformanceTracker());
		}

		private IConversationAggregationLogger CreateLogger()
		{
			return new ConversationAggregationLogger(this.session.MailboxGuid, this.session.OrganizationId);
		}

		private IMailboxPerformanceTracker CreatePerformanceTracker()
		{
			return new ConversationAggregationPerformanceTracker(this.session);
		}

		private IConversationAggregationLogger Logger
		{
			get
			{
				if (this.logger == null)
				{
					this.logger = this.CreateLogger();
				}
				return this.logger;
			}
		}

		private const string OperationContext = "ConversationAggregation";

		private static readonly Trace Tracer = ExTraceGlobals.ConversationTracer;

		private readonly IMailboxSession session;

		private IConversationAggregationLogger logger;
	}
}
