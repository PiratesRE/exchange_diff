using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class FaultInjection
	{
		public static Exception Callback(string exceptionType)
		{
			if (exceptionType != null)
			{
				foreach (FaultInjection.NameExceptionPair nameExceptionPair in FaultInjection.exceptions)
				{
					if (nameExceptionPair.Name.Contains(exceptionType))
					{
						return nameExceptionPair.Exception;
					}
				}
			}
			return null;
		}

		public static T TraceTest<T>(FaultInjection.LIDs faultLid)
		{
			T result = default(T);
			ExTraceGlobals.FaultInjectionTracer.TraceTest<T>((uint)faultLid, ref result);
			return result;
		}

		private static FaultInjection.NameExceptionPair[] exceptions = new FaultInjection.NameExceptionPair[]
		{
			new FaultInjection.NameExceptionPair
			{
				Name = typeof(OverBudgetException).FullName,
				Exception = new OverBudgetException()
			},
			new FaultInjection.NameExceptionPair
			{
				Name = typeof(VirusScanInProgressException).FullName,
				Exception = new VirusScanInProgressException(Strings.descVirusDetected("test"))
			},
			new FaultInjection.NameExceptionPair
			{
				Name = typeof(ConnectionFailedTransientException).FullName,
				Exception = new ConnectionFailedTransientException(Strings.descMailboxLogonFailed)
			},
			new FaultInjection.NameExceptionPair
			{
				Name = typeof(ConnectionFailedPermanentException).FullName,
				Exception = new ConnectionFailedPermanentException(Strings.descMailboxLogonFailed)
			},
			new FaultInjection.NameExceptionPair
			{
				Name = typeof(WebException).FullName + "_" + WebExceptionStatus.Timeout,
				Exception = new WebException("test", WebExceptionStatus.Timeout)
			},
			new FaultInjection.NameExceptionPair
			{
				Name = typeof(WebException).FullName + "_" + WebExceptionStatus.Pending,
				Exception = new WebException("test", WebExceptionStatus.Pending)
			}
		};

		internal enum LIDs : uint
		{
			UseDestinationUserAddress = 2743479613U
		}

		private class NameExceptionPair
		{
			public string Name;

			public Exception Exception;
		}
	}
}
