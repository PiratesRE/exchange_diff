using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.FfoReporting;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Management.ReportingTask;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	internal static class FaultInjection
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (FaultInjection.faultInjectionTracer == null)
				{
					lock (FaultInjection.lockObject)
					{
						if (FaultInjection.faultInjectionTracer == null)
						{
							FaultInjectionTrace faultInjectionTrace = ExTraceGlobals.FaultInjectionTracer;
							faultInjectionTrace.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjection.Callback));
							FaultInjection.faultInjectionTracer = faultInjectionTrace;
						}
					}
				}
				return FaultInjection.faultInjectionTracer;
			}
		}

		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if (typeof(InvalidExpressionException).FullName.Contains(exceptionType))
				{
					return new InvalidExpressionException(new LocalizedString("Fault Injection"));
				}
				if (typeof(Exception).FullName.Contains(exceptionType))
				{
					return new Exception("Fault Injection");
				}
			}
			return result;
		}

		internal const uint InternalValidateLid = 3355847997U;

		internal const uint InternalProcessRecordLid = 3070635325U;

		internal const uint RetrieveDalObjectsLid = 4270206269U;

		internal const uint InternalValidateAddMailboxErrors = 3783667005U;

		private const string ExceptionMessage = "Fault Injection";

		private static object lockObject = new object();

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
