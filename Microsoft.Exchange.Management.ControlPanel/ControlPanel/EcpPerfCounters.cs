using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class EcpPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (EcpPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in EcpPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Control Panel";

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchange Control Panel", "PID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RbacSessions = new ExPerformanceCounter("MSExchange Control Panel", "RBAC Sessions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RbacSessionsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "RBAC Sessions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RbacSessionsTotal = new ExPerformanceCounter("MSExchange Control Panel", "RBAC Sessions - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.RbacSessionsPerSecond
		});

		public static readonly ExPerformanceCounter RbacSessionsPeak = new ExPerformanceCounter("MSExchange Control Panel", "RBAC Sessions - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRbacSessionCreation = new ExPerformanceCounter("MSExchange Control Panel", "RBAC Sessions - Average Creation Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRbacSessionCreationBase = new ExPerformanceCounter("MSExchange Control Panel", "RBAC Sessions - Average Creation Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PowerShellRunspace = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter PowerShellRunspacePerSecond = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PowerShellRunspaceTotal = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.PowerShellRunspacePerSecond
		});

		public static readonly ExPerformanceCounter PowerShellRunspacePeak = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AveragePowerShellRunspaceCreation = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Average Creation Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AveragePowerShellRunspaceCreationBase = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Average Creation Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveRunspaces = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Active", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ActiveRunspacesPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Activations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveRunspacesTotal = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Activations Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.ActiveRunspacesPerSecond
		});

		public static readonly ExPerformanceCounter ActiveRunspacesPeak = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Peak Active", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageActiveRunspace = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Average Active Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageActiveRunspaceBase = new ExPerformanceCounter("MSExchange Control Panel", "PowerShell Runspaces - Average Active Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AspNetErrorsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "ASP.Net Request Failures/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AspNetErrors = new ExPerformanceCounter("MSExchange Control Panel", "ASP.Net Request Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.AspNetErrorsPerSecond
		});

		private static readonly ExPerformanceCounter WebServiceErrorsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Web Service Request Failures/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WebServiceErrors = new ExPerformanceCounter("MSExchange Control Panel", "Web Service Request Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.WebServiceErrorsPerSecond
		});

		public static readonly ExPerformanceCounter SendWatson = new ExPerformanceCounter("MSExchange Control Panel", "Watson Reports", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RedirectToErrorPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Requests Redirected To Error Page/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RedirectToError = new ExPerformanceCounter("MSExchange Control Panel", "Requests Redirected To Error Page", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.RedirectToErrorPerSecond
		});

		private static readonly ExPerformanceCounter WebServiceGetListPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - GetList Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WebServiceGetList = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - GetList Calls", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.WebServiceGetListPerSecond
		});

		private static readonly ExPerformanceCounter WebServiceGetObjectPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - GetObject Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WebServiceGetObject = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - GetObject Calls", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.WebServiceGetObjectPerSecond
		});

		private static readonly ExPerformanceCounter WebServiceNewObjectPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - NewObject Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WebServiceNewObject = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - NewObject Calls", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.WebServiceNewObjectPerSecond
		});

		private static readonly ExPerformanceCounter WebServiceSetObjectPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - SetObject Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WebServiceSetObject = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - SetObject Calls", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.WebServiceSetObjectPerSecond
		});

		private static readonly ExPerformanceCounter WebServiceRemoveObjectPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - RemoveObject Calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WebServiceRemoveObject = new ExPerformanceCounter("MSExchange Control Panel", "Web Service - RemoveObject Calls", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.WebServiceRemoveObjectPerSecond
		});

		public static readonly ExPerformanceCounter ActiveRequests = new ExPerformanceCounter("MSExchange Control Panel", "Requests - Active", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ActiveRequestsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Requests - Activations/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveRequestsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Requests - Activations Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.ActiveRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ActiveRequestsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Requests - Peak Active", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageResponseTime = new ExPerformanceCounter("MSExchange Control Panel", "Requests - Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageResponseTimeBase = new ExPerformanceCounter("MSExchange Control Panel", "Requests - Average Response Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutboundProxySessions = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Sessions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter OutboundProxySessionsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Sessions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutboundProxySessionsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Sessions - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.OutboundProxySessionsPerSecond
		});

		public static readonly ExPerformanceCounter OutboundProxySessionsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Sessions - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutboundProxyRequests = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter OutboundProxyRequestsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutboundProxyRequestsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Requests - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.OutboundProxyRequestsPerSecond
		});

		public static readonly ExPerformanceCounter OutboundProxyRequestsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Requests - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageOutboundProxyRequestsResponseTime = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Requests - Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageOutboundProxyRequestsResponseTimeBase = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Requests - Average Response Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutboundProxyRequestBytes = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Request Bytes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OutboundProxyResponseBytes = new ExPerformanceCounter("MSExchange Control Panel", "Outbound Proxy Response Bytes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoOutboundProxySessions = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Sessions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EsoOutboundProxySessionsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Sessions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoOutboundProxySessionsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Sessions - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.EsoOutboundProxySessionsPerSecond
		});

		public static readonly ExPerformanceCounter EsoOutboundProxySessionsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Sessions - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoOutboundProxyRequests = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EsoOutboundProxyRequestsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoOutboundProxyRequestsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Requests - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.EsoOutboundProxyRequestsPerSecond
		});

		public static readonly ExPerformanceCounter EsoOutboundProxyRequestsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Requests - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageEsoOutboundProxyRequestsResponseTime = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Requests - Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageEsoOutboundProxyRequestsResponseTimeBase = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Requests - Average Response Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoOutboundProxyRequestBytes = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Request Bytes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoOutboundProxyResponseBytes = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Outbound Proxy Response Bytes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter InboundProxySessions = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Sessions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter InboundProxySessionsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Sessions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter InboundProxySessionsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Sessions - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.InboundProxySessionsPerSecond
		});

		public static readonly ExPerformanceCounter InboundProxySessionsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Sessions - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter InboundProxyRequests = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter InboundProxyRequestsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter InboundProxyRequestsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Requests - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.InboundProxyRequestsPerSecond
		});

		public static readonly ExPerformanceCounter InboundProxyRequestsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Inbound Proxy Requests - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoInboundProxySessions = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Sessions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EsoInboundProxySessionsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Sessions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoInboundProxySessionsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Sessions - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.EsoInboundProxySessionsPerSecond
		});

		public static readonly ExPerformanceCounter EsoInboundProxySessionsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Sessions - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoInboundProxyRequests = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EsoInboundProxyRequestsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoInboundProxyRequestsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Requests - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.EsoInboundProxyRequestsPerSecond
		});

		public static readonly ExPerformanceCounter EsoInboundProxyRequestsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Inbound Proxy Requests - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter StandardSessions = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Sessions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter StandardSessionsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Sessions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter StandardSessionsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Sessions - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.StandardSessionsPerSecond
		});

		public static readonly ExPerformanceCounter StandardSessionsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Sessions - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter StandardRequests = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter StandardRequestsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter StandardRequestsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Requests - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.StandardRequestsPerSecond
		});

		public static readonly ExPerformanceCounter StandardRequestsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Standard RBAC Requests - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoStandardSessions = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Sessions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EsoStandardSessionsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Sessions/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoStandardSessionsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Sessions - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.EsoStandardSessionsPerSecond
		});

		public static readonly ExPerformanceCounter EsoStandardSessionsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Sessions - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoStandardRequests = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Requests", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter EsoStandardRequestsPerSecond = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EsoStandardRequestsTotal = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Requests - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			EcpPerfCounters.EsoStandardRequestsPerSecond
		});

		public static readonly ExPerformanceCounter EsoStandardRequestsPeak = new ExPerformanceCounter("MSExchange Control Panel", "Explicit Sign-On Standard RBAC Requests - Peak", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			EcpPerfCounters.PID,
			EcpPerfCounters.RbacSessions,
			EcpPerfCounters.RbacSessionsTotal,
			EcpPerfCounters.RbacSessionsPeak,
			EcpPerfCounters.AverageRbacSessionCreation,
			EcpPerfCounters.AverageRbacSessionCreationBase,
			EcpPerfCounters.PowerShellRunspace,
			EcpPerfCounters.PowerShellRunspaceTotal,
			EcpPerfCounters.PowerShellRunspacePeak,
			EcpPerfCounters.AveragePowerShellRunspaceCreation,
			EcpPerfCounters.AveragePowerShellRunspaceCreationBase,
			EcpPerfCounters.ActiveRunspaces,
			EcpPerfCounters.ActiveRunspacesTotal,
			EcpPerfCounters.ActiveRunspacesPeak,
			EcpPerfCounters.AverageActiveRunspace,
			EcpPerfCounters.AverageActiveRunspaceBase,
			EcpPerfCounters.AspNetErrors,
			EcpPerfCounters.WebServiceErrors,
			EcpPerfCounters.SendWatson,
			EcpPerfCounters.RedirectToError,
			EcpPerfCounters.WebServiceGetList,
			EcpPerfCounters.WebServiceGetObject,
			EcpPerfCounters.WebServiceNewObject,
			EcpPerfCounters.WebServiceSetObject,
			EcpPerfCounters.WebServiceRemoveObject,
			EcpPerfCounters.ActiveRequests,
			EcpPerfCounters.ActiveRequestsTotal,
			EcpPerfCounters.ActiveRequestsPeak,
			EcpPerfCounters.AverageResponseTime,
			EcpPerfCounters.AverageResponseTimeBase,
			EcpPerfCounters.OutboundProxySessions,
			EcpPerfCounters.OutboundProxySessionsTotal,
			EcpPerfCounters.OutboundProxySessionsPeak,
			EcpPerfCounters.OutboundProxyRequests,
			EcpPerfCounters.OutboundProxyRequestsTotal,
			EcpPerfCounters.OutboundProxyRequestsPeak,
			EcpPerfCounters.AverageOutboundProxyRequestsResponseTime,
			EcpPerfCounters.AverageOutboundProxyRequestsResponseTimeBase,
			EcpPerfCounters.OutboundProxyRequestBytes,
			EcpPerfCounters.OutboundProxyResponseBytes,
			EcpPerfCounters.EsoOutboundProxySessions,
			EcpPerfCounters.EsoOutboundProxySessionsTotal,
			EcpPerfCounters.EsoOutboundProxySessionsPeak,
			EcpPerfCounters.EsoOutboundProxyRequests,
			EcpPerfCounters.EsoOutboundProxyRequestsTotal,
			EcpPerfCounters.EsoOutboundProxyRequestsPeak,
			EcpPerfCounters.AverageEsoOutboundProxyRequestsResponseTime,
			EcpPerfCounters.AverageEsoOutboundProxyRequestsResponseTimeBase,
			EcpPerfCounters.EsoOutboundProxyRequestBytes,
			EcpPerfCounters.EsoOutboundProxyResponseBytes,
			EcpPerfCounters.InboundProxySessions,
			EcpPerfCounters.InboundProxySessionsTotal,
			EcpPerfCounters.InboundProxySessionsPeak,
			EcpPerfCounters.InboundProxyRequests,
			EcpPerfCounters.InboundProxyRequestsTotal,
			EcpPerfCounters.InboundProxyRequestsPeak,
			EcpPerfCounters.EsoInboundProxySessions,
			EcpPerfCounters.EsoInboundProxySessionsTotal,
			EcpPerfCounters.EsoInboundProxySessionsPeak,
			EcpPerfCounters.EsoInboundProxyRequests,
			EcpPerfCounters.EsoInboundProxyRequestsTotal,
			EcpPerfCounters.EsoInboundProxyRequestsPeak,
			EcpPerfCounters.StandardSessions,
			EcpPerfCounters.StandardSessionsTotal,
			EcpPerfCounters.StandardSessionsPeak,
			EcpPerfCounters.StandardRequests,
			EcpPerfCounters.StandardRequestsTotal,
			EcpPerfCounters.StandardRequestsPeak,
			EcpPerfCounters.EsoStandardSessions,
			EcpPerfCounters.EsoStandardSessionsTotal,
			EcpPerfCounters.EsoStandardSessionsPeak,
			EcpPerfCounters.EsoStandardRequests,
			EcpPerfCounters.EsoStandardRequestsTotal,
			EcpPerfCounters.EsoStandardRequestsPeak
		};
	}
}
