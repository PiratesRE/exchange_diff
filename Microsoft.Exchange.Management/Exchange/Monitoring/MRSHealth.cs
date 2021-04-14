using System;
using System.Collections.Generic;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	public static class MRSHealth
	{
		public static XElement QueryMRSDiagnostics(string serverName, string arguments, string xPath, string componentName = "MailboxReplicationService")
		{
			string text = ProcessAccessManager.ClientRunProcessCommand(serverName, "MSExchangeMailboxReplication", componentName, arguments, false, true, string.Empty);
			XElement xelement;
			try
			{
				xelement = XElement.Parse(text);
			}
			catch (XmlException ex)
			{
				throw new MRSDiagnosticXmlParsingException(ex.Message, text, ex);
			}
			xelement = xelement.XPathSelectElement(xPath);
			if (xelement == null)
			{
				throw new MRSExpectedDiagnosticsElementMissingException(xPath, text);
			}
			return xelement;
		}

		public static MRSHealthCheckOutcome VerifyServiceIsUp(string serverName, string fqdn, TestMRSHealth testMRSCmdlet = null)
		{
			MRSHealthCheckOutcome result;
			try
			{
				using (ManagementObject serviceObject = WmiWrapper.GetServiceObject(fqdn, "MSExchangeMailboxReplication"))
				{
					if (serviceObject == null)
					{
						if (testMRSCmdlet != null)
						{
							testMRSCmdlet.MonitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MRSHealth", 1001, EventTypeEnumeration.Error, Strings.MailboxReplicationServiceNotInstalled));
						}
						result = new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.ServiceCheck, false, Strings.MailboxReplicationServiceNotInstalled);
					}
					else if (!StringComparer.InvariantCultureIgnoreCase.Equals(serviceObject["State"] as string, "Running"))
					{
						if (testMRSCmdlet != null)
						{
							testMRSCmdlet.MonitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MRSHealth", 1001, EventTypeEnumeration.Error, Strings.MailboxReplicationServiceNotRunning));
						}
						result = new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.ServiceCheck, false, Strings.MailboxReplicationServiceNotRunning);
					}
					else
					{
						result = new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.ServiceCheck, true, Strings.MailboxReplicationServiceIsRunning);
					}
				}
			}
			catch (WmiException ex)
			{
				ServiceHealthWmiFailureException ex2 = new ServiceHealthWmiFailureException(ex.Message, ex);
				if (testMRSCmdlet != null)
				{
					testMRSCmdlet.WriteErrorAndMonitoringEvent(ex2, ErrorCategory.ReadError, 1001);
				}
				result = new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.ServiceCheck, false, CommonUtils.FullExceptionMessage(ex2, true));
			}
			return result;
		}

		public static MRSHealthCheckOutcome VerifyServiceIsRespondingToRPCPing(string serverName, TestMRSHealth testMRSCmdlet = null)
		{
			MRSHealthCheckOutcome outcome = null;
			CommonUtils.CatchKnownExceptions(delegate
			{
				MailboxReplicationServiceClient mailboxReplicationServiceClient2;
				MailboxReplicationServiceClient mailboxReplicationServiceClient = mailboxReplicationServiceClient2 = MailboxReplicationServiceClient.Create(serverName);
				try
				{
					outcome = new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.RPCPingCheck, true, Strings.MailboxReplicationServiceIsRespondingToRPCPing(mailboxReplicationServiceClient.ServerVersion.ToString()));
				}
				finally
				{
					if (mailboxReplicationServiceClient2 != null)
					{
						((IDisposable)mailboxReplicationServiceClient2).Dispose();
					}
				}
			}, delegate(Exception ex)
			{
				if (testMRSCmdlet != null)
				{
					testMRSCmdlet.MonitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MRSHealth", 1002, EventTypeEnumeration.Error, Strings.MailboxReplicationServiceNotResponding(CommonUtils.FullExceptionMessage(ex, true))));
				}
				outcome = new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.RPCPingCheck, false, Strings.MailboxReplicationServiceNotResponding(CommonUtils.FullExceptionMessage(ex, true)));
			});
			return outcome;
		}

		public static MRSHealthCheckOutcome VerifyMRSProxyIsRespondingToWCFPing(string mrsServer, string mrsProxyServer, NetworkCredential credentials, TestMRSHealth testMRSCmdlet = null)
		{
			MRSHealth.<>c__DisplayClass7 CS$<>8__locals1 = new MRSHealth.<>c__DisplayClass7();
			CS$<>8__locals1.mrsServer = mrsServer;
			CS$<>8__locals1.mrsProxyServer = mrsProxyServer;
			CS$<>8__locals1.credentials = credentials;
			CS$<>8__locals1.testMRSCmdlet = testMRSCmdlet;
			if (CS$<>8__locals1.mrsProxyServer == null)
			{
				CS$<>8__locals1.mrsProxyServer = CS$<>8__locals1.mrsServer;
			}
			CS$<>8__locals1.outcome = null;
			using (MailboxReplicationServiceClient mrsClient = MailboxReplicationServiceClient.Create(CS$<>8__locals1.mrsServer, MRSCapabilities.MrsProxyPing))
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					if (CS$<>8__locals1.credentials != null)
					{
						mrsClient.PingMRSProxy(CS$<>8__locals1.mrsProxyServer, CS$<>8__locals1.credentials.UserName, CS$<>8__locals1.credentials.Password, CS$<>8__locals1.credentials.Domain, true);
					}
					else
					{
						mrsClient.PingMRSProxy(CS$<>8__locals1.mrsProxyServer, null, null, null, false);
					}
					CS$<>8__locals1.outcome = new MRSHealthCheckOutcome(CS$<>8__locals1.mrsProxyServer, MRSHealthCheckId.MRSProxyPingCheck, true, Strings.MRSProxyIsRespondingtoPing(CS$<>8__locals1.mrsProxyServer));
				}, delegate(Exception ex)
				{
					if (CS$<>8__locals1.testMRSCmdlet != null)
					{
						CS$<>8__locals1.testMRSCmdlet.MonitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MRSHealth", 1004, EventTypeEnumeration.Error, Strings.MRSProxyNotResponding(CS$<>8__locals1.mrsProxyServer, CS$<>8__locals1.mrsServer, CommonUtils.FullExceptionMessage(ex, true))));
					}
					CS$<>8__locals1.outcome = new MRSHealthCheckOutcome(CS$<>8__locals1.mrsProxyServer, MRSHealthCheckId.MRSProxyPingCheck, false, Strings.MRSProxyNotResponding(CS$<>8__locals1.mrsProxyServer, CS$<>8__locals1.mrsServer, CommonUtils.FullExceptionMessage(ex, true)));
				});
			}
			return CS$<>8__locals1.outcome;
		}

		public static MRSHealthCheckOutcome VerifyServiceIsScanningForJobs(string serverName, long maxQueueScanAgeSeconds, TestMRSHealth testMRSCmdlet = null)
		{
			MRSDiagnosticQueryException ex = null;
			TimeSpan lastScanAge = TimeSpan.MaxValue;
			try
			{
				lastScanAge = MRSHealth.GetDurationSinceLastScan(serverName);
			}
			catch (MRSDiagnosticQueryException ex2)
			{
				ex = ex2;
			}
			if (ex != null)
			{
				if (testMRSCmdlet != null)
				{
					testMRSCmdlet.WriteErrorAndMonitoringEvent(ex, ErrorCategory.ReadError, 1003);
				}
				return new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.QueueScanCheck, false, CommonUtils.FullExceptionMessage(ex, true));
			}
			long num = (long)lastScanAge.TotalSeconds;
			if (testMRSCmdlet != null)
			{
				testMRSCmdlet.MonitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter("MSExchange Monitoring MRSHealth", "Last Scan Age (secs)", serverName, (double)num));
			}
			if (num <= maxQueueScanAgeSeconds)
			{
				return new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.QueueScanCheck, true, Strings.MailboxReplicationServiceIsScanningForJobs(lastScanAge));
			}
			if (testMRSCmdlet != null)
			{
				testMRSCmdlet.MonitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring MRSHealth", 1003, EventTypeEnumeration.Error, Strings.MailboxReplicationServiceNotScanningForJobs(lastScanAge)));
			}
			return new MRSHealthCheckOutcome(serverName, MRSHealthCheckId.QueueScanCheck, false, Strings.MailboxReplicationServiceNotScanningForJobs(lastScanAge));
		}

		public static TimeSpan GetDurationSinceLastScan(string serverName)
		{
			XElement xelement = MRSHealth.QueryMRSDiagnostics(serverName, string.Empty, "/Components/MailboxReplicationService/DurationSinceLastScan", "MailboxReplicationService");
			double value;
			if (double.TryParse(xelement.Value, out value))
			{
				return TimeSpan.FromMilliseconds(value);
			}
			throw new MRSCorruptDataInDiagnosticsException(xelement.Name.ToString(), xelement.Value);
		}

		public static DateTime GetServiceStartTime(string serverName)
		{
			XElement xelement = MRSHealth.QueryMRSDiagnostics(serverName, string.Empty, "/Components/MailboxReplicationService/ServiceStartTime", "MailboxReplicationService");
			DateTime result;
			if (!DateTime.TryParse(xelement.Value, out result))
			{
				throw new MRSCorruptDataInDiagnosticsException(xelement.Name.ToString(), xelement.Value);
			}
			return result;
		}

		public static IDictionary<RequestWorkloadType, bool> GetWorkloadsIsEnabled(string serverName)
		{
			Dictionary<RequestWorkloadType, bool> dictionary = new Dictionary<RequestWorkloadType, bool>();
			XElement xelement = MRSHealth.QueryMRSDiagnostics(serverName, "workloads", "/Components/MailboxReplicationService/Workloads", "MailboxReplicationService");
			foreach (XElement xelement2 in xelement.Elements())
			{
				RequestWorkloadType requestWorkloadType;
				if (!Enum.TryParse<RequestWorkloadType>(xelement2.Attribute("Name").Value, out requestWorkloadType))
				{
					throw new MRSCorruptDataInDiagnosticsException(xelement2.Name.ToString(), xelement2.Attribute("Name").Value);
				}
				bool value;
				if (!bool.TryParse(xelement2.Attribute("IsJobPickupEnabled").Value, out value))
				{
					throw new MRSCorruptDataInDiagnosticsException(requestWorkloadType.ToString(), xelement2.Attribute("IsJobPickupEnabled").Value);
				}
				dictionary.Add(requestWorkloadType, value);
			}
			return dictionary;
		}

		public const int DefaultMaxQueueScanAgeSeconds = 3600;
	}
}
