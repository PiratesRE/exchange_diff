using System;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class AvailabilityCommandBase<RequestType, SingleItemType> : SingleStepServiceCommand<RequestType, SingleItemType> where RequestType : BaseRequest
	{
		public AvailabilityCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		internal override void InternalCancelStep(LocalizedException exception, out bool isBatchStopResponse)
		{
			if (exception is OverBudgetException || exception is ResourceUnhealthyException)
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(new ServerBusyException(exception), FaultParty.Receiver);
			}
			throw FaultExceptionUtilities.CreateAvailabilityFault(exception, FaultParty.Receiver);
		}

		protected static void CheckRequestStreamSize(HttpRequest currentRequest)
		{
			int maximumRequestStreamSize = Configuration.MaximumRequestStreamSize;
			if (currentRequest.ContentLength > maximumRequestStreamSize)
			{
				throw new RequestStreamTooBigException((long)maximumRequestStreamSize, (long)currentRequest.ContentLength);
			}
		}

		protected void LogLatency(PerformanceContext ldapInitialPerformanceContext, PerformanceContext rpcInitialPerformanceContext)
		{
			PerformanceContext performanceContext;
			if (NativeMethods.GetTLSPerformanceContext(out performanceContext))
			{
				uint num = performanceContext.rpcCount - rpcInitialPerformanceContext.rpcCount;
				ulong num2 = (performanceContext.rpcLatency - rpcInitialPerformanceContext.rpcLatency) / 10000UL;
				base.CallContext.HttpContext.Items["TotalRpcRequestCount"] = num;
				base.CallContext.HttpContext.Items["TotalRpcRequestLatency"] = num2;
			}
			PerformanceContext performanceContext2 = PerformanceContext.Current;
			uint num3 = performanceContext2.RequestCount - ldapInitialPerformanceContext.RequestCount;
			int num4 = performanceContext2.RequestLatency - ldapInitialPerformanceContext.RequestLatency;
			base.CallContext.HttpContext.Items["TotalLdapRequestCount"] = num3;
			base.CallContext.HttpContext.Items["TotalLdapRequestLatency"] = num4;
		}

		protected static readonly Trace ConfigurationTracer = ExTraceGlobals.ConfigurationTracer;

		protected static readonly Trace CalendarViewTracer = ExTraceGlobals.CalendarViewTracer;

		protected static readonly Trace SecurityTracer = ExTraceGlobals.SecurityTracer;
	}
}
