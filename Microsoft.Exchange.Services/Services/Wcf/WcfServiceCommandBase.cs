using System;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class WcfServiceCommandBase
	{
		internal CallContext CallContext { get; private set; }

		private protected IdConverter IdConverter { protected get; private set; }

		private protected bool IsRequestTracingEnabled { protected get; private set; }

		protected WcfServiceCommandBase(CallContext callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(callContext, "callContext", "ServiceCommand::ServiceCommand");
			this.CallContext = callContext;
			this.IdConverter = new IdConverter(callContext);
			this.IsRequestTracingEnabled = this.CallContext.IsRequestTracingEnabled;
		}

		protected MailboxSession MailboxIdentityMailboxSession
		{
			get
			{
				return this.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
			}
		}

		protected void PrepareBudgetAndActivityScope()
		{
			WcfServiceCommandBase.ThrowIfNull(this.CallContext.Budget, "budget", "ServiceCommand::Execute");
			this.CallContext.Budget.StartLocal("ServiceCommand.Execute[" + this.CallContext.MethodName + "]", default(TimeSpan));
			this.scope = ActivityContext.GetCurrentActivityScope();
			if (this.scope != null)
			{
				this.scope.SetProperty(ServiceLatencyMetadata.PreExecutionLatency, this.scope.TotalMilliseconds.ToString(NumberFormatInfo.InvariantInfo));
			}
		}

		protected void PreExecute()
		{
			if (string.IsNullOrEmpty(this.scope.Component) && this.CallContext != null)
			{
				this.scope.Component = this.CallContext.WorkloadType.ToString();
			}
			if (string.IsNullOrEmpty(this.scope.Action))
			{
				this.scope.Action = base.GetType().Name;
			}
			this.preExecuteADSnapshot = this.scope.TakeStatisticsSnapshot(AggregatedOperationType.ADCalls);
			this.preExecuteRPCSnapshot = this.scope.TakeStatisticsSnapshot(AggregatedOperationType.StoreRPCs);
			this.executeStopWatch = Stopwatch.StartNew();
		}

		protected void PostExecute()
		{
			this.executeStopWatch.Stop();
			this.scope.SetProperty(ServiceLatencyMetadata.CoreExecutionLatency, this.executeStopWatch.ElapsedMilliseconds.ToString(NumberFormatInfo.InvariantInfo));
			AggregatedOperationStatistics aggregatedOperationStatistics = this.scope.TakeStatisticsSnapshot(AggregatedOperationType.ADCalls) - this.preExecuteADSnapshot;
			if (aggregatedOperationStatistics.Count > 0L)
			{
				this.scope.SetProperty(ServiceTaskMetadata.ADCount, aggregatedOperationStatistics.Count.ToString(NumberFormatInfo.InvariantInfo));
				this.scope.SetProperty(ServiceTaskMetadata.ADLatency, aggregatedOperationStatistics.TotalMilliseconds.ToString(NumberFormatInfo.InvariantInfo));
			}
			AggregatedOperationStatistics aggregatedOperationStatistics2 = this.scope.TakeStatisticsSnapshot(AggregatedOperationType.StoreRPCs) - this.preExecuteRPCSnapshot;
			if (aggregatedOperationStatistics2.Count > 0L)
			{
				this.scope.SetProperty(ServiceTaskMetadata.RpcCount, aggregatedOperationStatistics2.Count.ToString(NumberFormatInfo.InvariantInfo));
				this.scope.SetProperty(ServiceTaskMetadata.RpcLatency, aggregatedOperationStatistics2.TotalMilliseconds.ToString(NumberFormatInfo.InvariantInfo));
			}
		}

		protected static void ThrowIfNull(object objectToCheck, string parameterName, string methodName)
		{
			if (objectToCheck == null)
			{
				string message = WcfServiceCommandBase.BuildExceptionMessage(methodName, parameterName, "is null.");
				throw new FaultException(new ArgumentNullException(parameterName, message).Message);
			}
		}

		protected virtual void LogTracesForCurrentRequest()
		{
		}

		private static string BuildExceptionMessage(string methodName, string parameterName, string message)
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0}] {1} {2}", new object[]
			{
				methodName,
				parameterName,
				message
			});
		}

		protected void LogRequestTraces()
		{
			if (this.IsRequestTracingEnabled)
			{
				this.LogTracesForCurrentRequest();
			}
		}

		protected static readonly TraceToHeadersLoggerFactory TraceLoggerFactory = new TraceToHeadersLoggerFactory(VariantConfiguration.InvariantNoFlightingSnapshot.Diagnostics.TraceToHeadersLogger.Enabled);

		protected IActivityScope scope;

		private Stopwatch executeStopWatch;

		private AggregatedOperationStatistics preExecuteADSnapshot;

		private AggregatedOperationStatistics preExecuteRPCSnapshot;
	}
}
