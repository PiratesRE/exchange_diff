using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FaultInjectionUtils
	{
		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if ("Microsoft.Mapi.MapiExceptionNetworkError".Contains(exceptionType))
				{
					result = new MapiExceptionNetworkError("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionExiting".Contains(exceptionType))
				{
					result = new MapiExceptionExiting("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionMdbOffline".Contains(exceptionType))
				{
					result = new MapiExceptionMdbOffline("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionBackupInProgress".Contains(exceptionType))
				{
					result = new MapiExceptionBackupInProgress("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionNotFound".Contains(exceptionType))
				{
					result = new MapiExceptionNotFound("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionUnableToComplete".Contains(exceptionType))
				{
					result = new MapiExceptionUnableToComplete("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionWatermarkError".Contains(exceptionType))
				{
					result = new MapiExceptionWatermarkError("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionWrongServer".Contains(exceptionType))
				{
					result = new MapiExceptionWrongServer("FaultInjection", -1, -1, null, null);
				}
				else if ("Microsoft.Mapi.MapiExceptionNoAccess".Contains(exceptionType))
				{
					result = new MapiExceptionNoAccess("FaultInjection", -2147024891, -2147024891, null, null);
				}
			}
			return result;
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (FaultInjectionUtils.faultInjectionTracer == null)
				{
					FaultInjectionUtils.faultInjectionTracer = new FaultInjectionTrace(FaultInjectionUtils.MapiNetComponent, FaultInjectionUtils.tagFaultInjection);
					FaultInjectionUtils.faultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjectionUtils.Callback));
				}
				return FaultInjectionUtils.faultInjectionTracer;
			}
		}

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Guid MapiNetComponent = new Guid("82914ab6-016b-442c-8e49-2562a4333be0");

		private static int tagFaultInjection = 35;
	}
}
