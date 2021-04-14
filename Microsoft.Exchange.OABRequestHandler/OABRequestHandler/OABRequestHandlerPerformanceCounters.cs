using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OABRequestHandler
{
	internal static class OABRequestHandlerPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (OABRequestHandlerPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in OABRequestHandlerPerformanceCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchangeOABRequestHandler";

		public static readonly ExPerformanceCounter RequestHandlingAverageTime = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Request Handling Average Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RequestHandlingAverageTimeBase = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Request Handling Average Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AccessDeniedFailuresPerSecond = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Access Denied Failures/Sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AccessDeniedFailures = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Access Denied Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			OABRequestHandlerPerformanceCounters.AccessDeniedFailuresPerSecond
		});

		private static readonly ExPerformanceCounter InvalidRequestFailuresPerSecond = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Invalid Request Failures/Sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter InvalidRequestFailures = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Invalid Request Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			OABRequestHandlerPerformanceCounters.InvalidRequestFailuresPerSecond
		});

		private static readonly ExPerformanceCounter FileNotAvailableFailuresPerSecond = new ExPerformanceCounter("MSExchangeOABRequestHandler", "File Not Available Failures/Sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FileNotAvailableFailures = new ExPerformanceCounter("MSExchangeOABRequestHandler", "File Not Available Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			OABRequestHandlerPerformanceCounters.FileNotAvailableFailuresPerSecond
		});

		public static readonly ExPerformanceCounter CurrentNumberRequestsInCache = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Current Number Requests In Cache", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter DirectoryFailuresPerSecond = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Directory Failures/Sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DirectoryFailures = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Directory Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			OABRequestHandlerPerformanceCounters.DirectoryFailuresPerSecond
		});

		private static readonly ExPerformanceCounter UnknownFailuresPerSecond = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Unknown Failures/Sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UnknownFailures = new ExPerformanceCounter("MSExchangeOABRequestHandler", "Unknown Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			OABRequestHandlerPerformanceCounters.UnknownFailuresPerSecond
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			OABRequestHandlerPerformanceCounters.RequestHandlingAverageTime,
			OABRequestHandlerPerformanceCounters.RequestHandlingAverageTimeBase,
			OABRequestHandlerPerformanceCounters.AccessDeniedFailures,
			OABRequestHandlerPerformanceCounters.InvalidRequestFailures,
			OABRequestHandlerPerformanceCounters.FileNotAvailableFailures,
			OABRequestHandlerPerformanceCounters.CurrentNumberRequestsInCache,
			OABRequestHandlerPerformanceCounters.DirectoryFailures,
			OABRequestHandlerPerformanceCounters.UnknownFailures
		};
	}
}
