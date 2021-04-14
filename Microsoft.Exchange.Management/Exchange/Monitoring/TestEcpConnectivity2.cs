using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.MonitoringWebClient.Ecp;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "EcpConnectivity2", SupportsShouldProcess = true, DefaultParameterSetName = "ClientAccessServer")]
	public class TestEcpConnectivity2 : TestWebApplicationConnectivity2
	{
		public TestEcpConnectivity2() : base(Strings.CasHealthEcpLongName, Strings.CasHealthEcpShortName, "MSExchange Monitoring ECPConnectivity2 Internal", "MSExchange Monitoring ECPConnectivity2 External")
		{
		}

		internal TestEcpConnectivity2(LocalizedString applicationName, LocalizedString applicationShortName, string monitoringEventSourceInternal, string monitoringEventSourceExternal) : base(applicationName, applicationShortName, monitoringEventSourceInternal, monitoringEventSourceExternal)
		{
		}

		private static void InitializeEndpointLists()
		{
			if (TestEcpConnectivity2.consumerLiveIdHostNames == null)
			{
				lock (TestEcpConnectivity2.lockObject)
				{
					if (TestEcpConnectivity2.consumerLiveIdHostNames == null)
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 102, "InitializeEndpointLists", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\TestEcpConnectivity2.cs");
						ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
						TestEcpConnectivity2.exchangeHostNames = new List<string>();
						TestEcpConnectivity2.exchangeHostNames.Add(endpointContainer.GetEndpoint(ServiceEndpointId.ExchangeLoginUrl).Uri.Host);
						TestEcpConnectivity2.businessLiveIdHostNames = new List<string>();
						TestEcpConnectivity2.businessLiveIdHostNames.Add(endpointContainer.GetEndpoint(ServiceEndpointId.MsoServiceLogin2).Uri.Host);
						TestEcpConnectivity2.consumerLiveIdHostNames = new List<string>();
						TestEcpConnectivity2.consumerLiveIdHostNames.Add(endpointContainer.GetEndpoint(ServiceEndpointId.LiveServiceLogin2).Uri.Host);
					}
				}
			}
		}

		protected override IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories(ADObjectId serverId, QueryFilter filter)
		{
			return base.GetVirtualDirectories<ADEcpVirtualDirectory>(serverId, filter);
		}

		internal override IExceptionAnalyzer CreateExceptionAnalyzer(Uri uri)
		{
			TestEcpConnectivity2.InitializeEndpointLists();
			Dictionary<string, RequestTarget> dictionary = new Dictionary<string, RequestTarget>();
			dictionary.Add(uri.Host, RequestTarget.Ecp);
			this.AddHostMapping(dictionary, RequestTarget.Ecp, TestEcpConnectivity2.exchangeHostNames);
			this.AddHostMapping(dictionary, RequestTarget.LiveIdConsumer, TestEcpConnectivity2.consumerLiveIdHostNames);
			this.AddHostMapping(dictionary, RequestTarget.LiveIdBusiness, TestEcpConnectivity2.businessLiveIdHostNames);
			return new EcpExceptionAnalyzer(dictionary);
		}

		internal override ITestStep CreateScenario(TestCasConnectivity.TestCasConnectivityRunInstance instance, Uri testUri, string userName, string domain, SecureString password, VirtualDirectoryUriScope testType, string serverFqdn)
		{
			ITestFactory testFactory = new TestFactory();
			ITestStep result;
			if (testType == VirtualDirectoryUriScope.Internal || testType == VirtualDirectoryUriScope.Unknown)
			{
				result = testFactory.CreateEcpLoginScenario(testUri, userName, domain, password, testFactory);
			}
			else
			{
				result = testFactory.CreateEcpExternalLoginAgainstSpecificServerScenario(testUri, userName, domain, password, serverFqdn, testFactory);
			}
			return result;
		}

		internal override void CompleteSuccessfulOutcome(CasTransactionOutcome outcome, TestCasConnectivity.TestCasConnectivityRunInstance instance, IResponseTracker responseTracker)
		{
			outcome.Update(CasTransactionResultEnum.Success);
			base.WriteMonitoringEvent(1000, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.CasHealthAllTransactionsSucceeded);
		}

		internal override void CompleteFailedOutcome(CasTransactionOutcome outcome, TestCasConnectivity.TestCasConnectivityRunInstance instance, IResponseTracker responseTracker, Exception e)
		{
			TestEcpConnectivityOutcome testEcpConnectivityOutcome = outcome as TestEcpConnectivityOutcome;
			ScenarioException scenarioException = e.GetScenarioException();
			if (scenarioException != null)
			{
				testEcpConnectivityOutcome.FailureSource = scenarioException.FailureSource.ToString();
				testEcpConnectivityOutcome.FailureReason = scenarioException.FailureReason.ToString();
				testEcpConnectivityOutcome.FailingComponent = scenarioException.FailingComponent.ToString();
			}
			testEcpConnectivityOutcome.Update(CasTransactionResultEnum.Failure, (scenarioException != null) ? scenarioException.Message.ToString() : e.Message.ToString(), EventTypeEnumeration.Error);
			base.WriteMonitoringEvent(1001, this.MonitoringEventSource, EventTypeEnumeration.Error, Strings.CasHealthTransactionFailures((scenarioException != null) ? scenarioException.Message.ToString() : e.Message.ToString()));
		}

		internal override CasTransactionOutcome CreateOutcome(TestCasConnectivity.TestCasConnectivityRunInstance instance, Uri testUri, IResponseTracker responseTracker)
		{
			return new TestEcpConnectivityOutcome(instance.CasFqdn, (instance.exchangePrincipal == null) ? null : instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn, Strings.CasHealthEcpScenarioTestWebService, Strings.CasHealthEcpScenarioTestWebServiceDescription, "Logon Latency", base.LocalSiteName, instance.trustAllCertificates, instance.credentials.UserName, instance.VirtualDirectoryName, testUri, instance.UrlType)
			{
				HttpData = responseTracker.Items
			};
		}

		private void AddHostMapping(Dictionary<string, RequestTarget> sourceMapping, RequestTarget source, List<string> hostNames)
		{
			foreach (string key in hostNames)
			{
				if (!sourceMapping.ContainsKey(key))
				{
					sourceMapping.Add(key, source);
				}
			}
		}

		private const string monitoringEventSourceInternal = "MSExchange Monitoring ECPConnectivity2 Internal";

		private const string monitoringEventSourceExternal = "MSExchange Monitoring ECPConnectivity2 External";

		private const string MonitoringLatencyPerfCounter = "Logon Latency";

		private static object lockObject = new object();

		private static List<string> exchangeHostNames = null;

		private static List<string> businessLiveIdHostNames = null;

		private static List<string> consumerLiveIdHostNames = null;
	}
}
