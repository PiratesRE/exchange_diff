using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal static class RwsPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (RwsPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in RwsPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange ReportingWebService";

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchange ReportingWebService", "PID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RbacPrincipals = new ExPerformanceCounter("MSExchange ReportingWebService", "RBAC Principals", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RbacPrincipalsPerSecond = new ExPerformanceCounter("MSExchange ReportingWebService", "RBAC Principals/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RbacPrincipalsTotal = new ExPerformanceCounter("MSExchange ReportingWebService", "RBAC Principals - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			RwsPerfCounters.RbacPrincipalsPerSecond
		});

		public static readonly ExPerformanceCounter RbacPrincipalsPeak = new ExPerformanceCounter("MSExchange ReportingWebService", "RBAC Principals - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRbacPrincipalCreation = new ExPerformanceCounter("MSExchange ReportingWebService", "RBAC Principals - Average Creation Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRbacPrincipalCreationBase = new ExPerformanceCounter("MSExchange ReportingWebService", "RBAC Principals - Average Creation Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PowerShellRunspace = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter PowerShellRunspacePerSecond = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PowerShellRunspaceTotal = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			RwsPerfCounters.PowerShellRunspacePerSecond
		});

		public static readonly ExPerformanceCounter PowerShellRunspacePeak = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AveragePowerShellRunspaceCreation = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Average Creation Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AveragePowerShellRunspaceCreationBase = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Average Creation Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveRunspaces = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Active", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ActiveRunspacesPerSecond = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Activations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveRunspacesTotal = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Activations Total", string.Empty, null, new ExPerformanceCounter[]
		{
			RwsPerfCounters.ActiveRunspacesPerSecond
		});

		public static readonly ExPerformanceCounter ActiveRunspacesPeak = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Peak Active", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageActiveRunspace = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Average Active Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageActiveRunspaceBase = new ExPerformanceCounter("MSExchange ReportingWebService", "PowerShell Runspaces - Average Active Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RequestErrorsPerSecond = new ExPerformanceCounter("MSExchange ReportingWebService", "Request Failures/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReportCmdletErrors = new ExPerformanceCounter("MSExchange ReportingWebService", "Report Cmdlet Failures", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ReportCmdletErrorsPerSecond = new ExPerformanceCounter("MSExchange ReportingWebService", "Report Cmdlet Failures/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RequestErrors = new ExPerformanceCounter("MSExchange ReportingWebService", "Request Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			RwsPerfCounters.RequestErrorsPerSecond,
			RwsPerfCounters.ReportCmdletErrorsPerSecond
		});

		public static readonly ExPerformanceCounter SendWatson = new ExPerformanceCounter("MSExchange ReportingWebService", "Watson Reports", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveRequests = new ExPerformanceCounter("MSExchange ReportingWebService", "Requests - Active", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ActiveRequestsPerSecond = new ExPerformanceCounter("MSExchange ReportingWebService", "Requests - Activations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveRequestsTotal = new ExPerformanceCounter("MSExchange ReportingWebService", "Requests - Activations Total", string.Empty, null, new ExPerformanceCounter[]
		{
			RwsPerfCounters.ActiveRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ActiveRequestsPeak = new ExPerformanceCounter("MSExchange ReportingWebService", "Requests - Peak Active", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRequestResponseTime = new ExPerformanceCounter("MSExchange ReportingWebService", "Requests - Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRequestResponseTimeBase = new ExPerformanceCounter("MSExchange ReportingWebService", "Requests - Average Response Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageReportCmdletResponseTime = new ExPerformanceCounter("MSExchange ReportingWebService", "Report Cmdlet - Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageReportCmdletResponseTimeBase = new ExPerformanceCounter("MSExchange ReportingWebService", "Report Cmdlet - Average Response Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageReportRow = new ExPerformanceCounter("MSExchange ReportingWebService", "Average Report Row", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageReportRowBase = new ExPerformanceCounter("MSExchange ReportingWebService", "Average Report Row Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			RwsPerfCounters.PID,
			RwsPerfCounters.RbacPrincipals,
			RwsPerfCounters.RbacPrincipalsTotal,
			RwsPerfCounters.RbacPrincipalsPeak,
			RwsPerfCounters.AverageRbacPrincipalCreation,
			RwsPerfCounters.AverageRbacPrincipalCreationBase,
			RwsPerfCounters.PowerShellRunspace,
			RwsPerfCounters.PowerShellRunspaceTotal,
			RwsPerfCounters.PowerShellRunspacePeak,
			RwsPerfCounters.AveragePowerShellRunspaceCreation,
			RwsPerfCounters.AveragePowerShellRunspaceCreationBase,
			RwsPerfCounters.ActiveRunspaces,
			RwsPerfCounters.ActiveRunspacesTotal,
			RwsPerfCounters.ActiveRunspacesPeak,
			RwsPerfCounters.AverageActiveRunspace,
			RwsPerfCounters.AverageActiveRunspaceBase,
			RwsPerfCounters.RequestErrors,
			RwsPerfCounters.ReportCmdletErrors,
			RwsPerfCounters.SendWatson,
			RwsPerfCounters.ActiveRequests,
			RwsPerfCounters.ActiveRequestsTotal,
			RwsPerfCounters.ActiveRequestsPeak,
			RwsPerfCounters.AverageRequestResponseTime,
			RwsPerfCounters.AverageRequestResponseTimeBase,
			RwsPerfCounters.AverageReportCmdletResponseTime,
			RwsPerfCounters.AverageReportCmdletResponseTimeBase,
			RwsPerfCounters.AverageReportRow,
			RwsPerfCounters.AverageReportRowBase
		};
	}
}
