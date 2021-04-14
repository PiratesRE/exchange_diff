using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class ProxyEventLogHelper
	{
		public static void LogServicesDiscoveryFailure(string serverFQDN)
		{
			ProxyEventLogHelper.LocalServerInformation localServerInformation = ProxyEventLogHelper.LocalServerInformation.Create();
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_ServiceDiscoveryFailure, "EWSProxy_ServiceDiscoveryFailure", new object[]
			{
				localServerInformation.Name,
				localServerInformation.ExchangeVersion,
				SingleProxyDeterministicCASBoxScoring.GetSiteIdForServer(serverFQDN),
				serverFQDN
			});
		}

		public static void LogNoApplicableDestinationCAS(string destinationServerFQDN)
		{
			string siteIdForServer = SingleProxyDeterministicCASBoxScoring.GetSiteIdForServer(destinationServerFQDN);
			ProxyEventLogHelper.LocalServerInformation localServerInformation = ProxyEventLogHelper.LocalServerInformation.Create();
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_NoApplicableDestinationCAS, "EWSProxy_NoApplicableDestinationCAS", new object[]
			{
				localServerInformation.Name,
				localServerInformation.SiteDistinguishedName,
				siteIdForServer,
				siteIdForServer
			});
		}

		public static void LogNoRespondingDestinationCAS(string siteId)
		{
			ProxyEventLogHelper.LocalServerInformation localServerInformation = ProxyEventLogHelper.LocalServerInformation.Create();
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_NoRespondingDestinationCAS, string.Empty, new object[]
			{
				localServerInformation.Name,
				siteId,
				siteId
			});
		}

		public static void LogCallerDeniedProxyRight(SecurityIdentifier callerSid)
		{
			ProxyEventLogHelper.LocalServerInformation localServerInformation = ProxyEventLogHelper.LocalServerInformation.Create();
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_ProxyRightDenied, "EWSProxy_ProxyRightDenied_" + callerSid.ToString(), new object[]
			{
				callerSid,
				localServerInformation.Name,
				localServerInformation.Name
			});
		}

		public static void LogNoTrustedCertificateOnDestinationCAS(string destinationServerFQDN)
		{
			ProxyEventLogHelper.LocalServerInformation localServerInformation = ProxyEventLogHelper.LocalServerInformation.Create();
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_NoTrustedCertificateOnDestinationCAS, "EWSProxy_NoTrustedCertificateOnDestinationCAS" + destinationServerFQDN, new object[]
			{
				localServerInformation.Name,
				destinationServerFQDN,
				localServerInformation.Name
			});
		}

		public static void LogKerberosConfigurationProblem(WebServicesInfo destinationEws)
		{
			ProxyEventLogHelper.LocalServerInformation localServerInformation = ProxyEventLogHelper.LocalServerInformation.Create();
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_KerberosConfigurationProblem, "EWSProxy_KerberosConfigurationProblem_" + destinationEws.ServerFullyQualifiedDomainName, new object[]
			{
				localServerInformation.Name,
				destinationEws.ServerFullyQualifiedDomainName,
				destinationEws.ServerFullyQualifiedDomainName,
				destinationEws.ServerFullyQualifiedDomainName
			});
		}

		public static void LogExceededGroupSidLimit(SecurityIdentifier userSid, int sidLimit)
		{
			ProxyEventLogHelper.LocalServerInformation localServerInformation = ProxyEventLogHelper.LocalServerInformation.Create();
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_ExceededGroupSidLimit, "EWSProxy_ExceededGroupSidLimit_" + userSid.ToString(), new object[]
			{
				localServerInformation.Name,
				userSid,
				sidLimit
			});
		}

		private struct LocalServerInformation
		{
			internal static ProxyEventLogHelper.LocalServerInformation Create()
			{
				Server server = null;
				try
				{
					FaultInjection.GenerateFault((FaultInjection.LIDs)3454414141U);
					server = LocalServer.GetServer();
				}
				catch (ADTransientException arg)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceError<ADTransientException>(0L, "LocalServerInformation constructor encountered exception in LocalServer.GetServer() call: {0}", arg);
				}
				return new ProxyEventLogHelper.LocalServerInformation(server);
			}

			private LocalServerInformation(Server server)
			{
				if (server == null)
				{
					this.name = string.Empty;
					this.exchangeVersion = string.Empty;
					this.siteDistinguishedName = string.Empty;
					return;
				}
				this.name = server.Name;
				this.exchangeVersion = server.ExchangeVersion.ToString();
				this.siteDistinguishedName = server.ServerSite.DistinguishedName;
			}

			internal string Name
			{
				get
				{
					return this.name;
				}
			}

			internal string ExchangeVersion
			{
				get
				{
					return this.exchangeVersion;
				}
			}

			internal string SiteDistinguishedName
			{
				get
				{
					return this.siteDistinguishedName;
				}
			}

			private string name;

			private string exchangeVersion;

			private string siteDistinguishedName;
		}
	}
}
