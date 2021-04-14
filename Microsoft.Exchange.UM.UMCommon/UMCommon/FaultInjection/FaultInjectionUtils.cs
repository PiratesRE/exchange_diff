using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FaultInjectionUtils
	{
		private static void RegisterCallback()
		{
			FaultInjectionUtils.createExceptions = new List<CreateException>();
			FaultInjectionUtils.createExceptions.Add(new CreateException(RMSFaultInjection.TryCreateException));
			FaultInjectionUtils.createExceptions.Add(new CreateException(UMLicensingFaultInjection.TryCreateException));
			FaultInjectionUtils.createExceptions.Add(new CreateException(DiagnosticFaultInjection.TryCreateException));
			FaultInjectionUtils.createExceptions.Add(new CreateException(UMReportingFaultInjection.TryCreateException));
			FaultInjectionUtils.createExceptions.Add(new CreateException(UMGrammarGeneratorFaultInjection.TryCreateException));
		}

		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			foreach (CreateException ex in FaultInjectionUtils.createExceptions)
			{
				if (ex(exceptionType, ref result))
				{
					return result;
				}
			}
			return null;
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (FaultInjectionUtils.faultInjectionTracer == null)
				{
					lock (FaultInjectionUtils.lockObject)
					{
						if (FaultInjectionUtils.faultInjectionTracer == null)
						{
							FaultInjectionTrace faultInjectionTrace = ExTraceGlobals.FaultInjectionTracer;
							FaultInjectionUtils.RegisterCallback();
							faultInjectionTrace.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjectionUtils.Callback));
							FaultInjectionUtils.faultInjectionTracer = faultInjectionTrace;
						}
					}
				}
				return FaultInjectionUtils.faultInjectionTracer;
			}
		}

		public static void FaultInjectException(uint lid)
		{
			if (Utils.RunningInTestMode)
			{
				FaultInjectionUtils.FaultInjectionTracer.TraceTest(lid);
			}
		}

		public static void FaultInjectChangeValue<T>(uint lid, ref T objectToChange)
		{
			if (Utils.RunningInTestMode)
			{
				FaultInjectionUtils.FaultInjectionTracer.TraceTest<T>(lid, ref objectToChange);
			}
		}

		public static void FaultInjectCompare<T>(uint lid, T objectToCompare)
		{
			if (Utils.RunningInTestMode)
			{
				FaultInjectionUtils.FaultInjectionTracer.TraceTest<T>(lid, objectToCompare);
			}
		}

		private static List<CreateException> createExceptions;

		private static object lockObject = new object();

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
