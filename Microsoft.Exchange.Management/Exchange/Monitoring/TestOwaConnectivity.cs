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
using Microsoft.Exchange.Net.MonitoringWebClient.Owa;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "OwaConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "ClientAccessServer")]
	public class TestOwaConnectivity : TestWebApplicationConnectivity2
	{
		public TestOwaConnectivity() : base(Strings.CasHealthOwaLongName, Strings.CasHealthOwaShortName, "MSExchange Monitoring OWAConnectivity Internal", "MSExchange Monitoring OWAConnectivity External")
		{
		}

		internal TestOwaConnectivity(LocalizedString applicationName, LocalizedString applicationShortName, string monitoringEventSourceInternal, string monitoringEventSourceExternal) : base(applicationName, applicationShortName, monitoringEventSourceInternal, monitoringEventSourceExternal)
		{
		}

		private static void InitializeEndpointLists()
		{
			if (TestOwaConnectivity.consumerLiveIdHostNames == null)
			{
				lock (TestOwaConnectivity.lockObject)
				{
					if (TestOwaConnectivity.consumerLiveIdHostNames == null)
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 107, "InitializeEndpointLists", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\TestOwaConnectivity.cs");
						ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
						TestOwaConnectivity.exchangeHostNames = new List<string>();
						TestOwaConnectivity.exchangeHostNames.Add(endpointContainer.GetEndpoint(ServiceEndpointId.ExchangeLoginUrl).Uri.Host);
						TestOwaConnectivity.businessLiveIdHostNames = new List<string>();
						TestOwaConnectivity.businessLiveIdHostNames.Add(endpointContainer.GetEndpoint(ServiceEndpointId.MsoServiceLogin2).Uri.Host);
						TestOwaConnectivity.consumerLiveIdHostNames = new List<string>();
						TestOwaConnectivity.consumerLiveIdHostNames.Add(endpointContainer.GetEndpoint(ServiceEndpointId.LiveServiceLogin2).Uri.Host);
					}
				}
			}
		}

		protected override IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories(ADObjectId serverId, QueryFilter filter)
		{
			filter = new AndFilter(new QueryFilter[]
			{
				filter,
				new ComparisonFilter(ComparisonOperator.NotEqual, ADOwaVirtualDirectorySchema.OwaVersion, OwaVersions.Exchange2003or2000)
			});
			return base.GetVirtualDirectories<ADOwaVirtualDirectory>(serverId, filter);
		}

		internal override IExceptionAnalyzer CreateExceptionAnalyzer(Uri uri)
		{
			TestOwaConnectivity.InitializeEndpointLists();
			Dictionary<string, RequestTarget> dictionary = new Dictionary<string, RequestTarget>();
			dictionary.Add(uri.Host, RequestTarget.Owa);
			this.AddHostMapping(dictionary, RequestTarget.Owa, TestOwaConnectivity.exchangeHostNames);
			this.AddHostMapping(dictionary, RequestTarget.LiveIdConsumer, TestOwaConnectivity.consumerLiveIdHostNames);
			this.AddHostMapping(dictionary, RequestTarget.LiveIdBusiness, TestOwaConnectivity.businessLiveIdHostNames);
			return new OwaExceptionAnalyzer(dictionary);
		}

		internal override ITestStep CreateScenario(TestCasConnectivity.TestCasConnectivityRunInstance instance, Uri testUri, string userName, string domain, SecureString password, VirtualDirectoryUriScope testType, string serverFqdn)
		{
			ITestFactory testFactory = new TestFactory();
			ITestStep result;
			if (testType == VirtualDirectoryUriScope.Internal || testType == VirtualDirectoryUriScope.Unknown)
			{
				result = testFactory.CreateOwaLoginScenario(testUri, userName, domain, password, new OwaLoginParameters(), testFactory);
			}
			else
			{
				result = testFactory.CreateOwaExternalLoginAgainstSpecificServerScenario(testUri, userName, domain, password, serverFqdn, testFactory);
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
			TestOwaConnectivityOutcome testOwaConnectivityOutcome = outcome as TestOwaConnectivityOutcome;
			ScenarioException scenarioException = e.GetScenarioException();
			if (scenarioException != null)
			{
				testOwaConnectivityOutcome.FailureSource = scenarioException.FailureSource.ToString();
				testOwaConnectivityOutcome.FailureReason = scenarioException.FailureReason.ToString();
				testOwaConnectivityOutcome.FailingComponent = scenarioException.FailingComponent.ToString();
			}
			testOwaConnectivityOutcome.Update(CasTransactionResultEnum.Failure, (scenarioException != null) ? scenarioException.Message.ToString() : e.Message.ToString(), EventTypeEnumeration.Error);
			base.WriteMonitoringEvent(1001, this.MonitoringEventSource, EventTypeEnumeration.Error, Strings.CasHealthTransactionFailures(this.FormatExceptionText(scenarioException, e)));
		}

		internal override CasTransactionOutcome CreateOutcome(TestCasConnectivity.TestCasConnectivityRunInstance instance, Uri testUri, IResponseTracker responseTracker)
		{
			return new TestOwaConnectivityOutcome(instance.CasFqdn, (instance.exchangePrincipal == null) ? null : instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn, Strings.CasHealthOwaLogonScenarioName, Strings.CasHealthOwaLogonScenarioDescription, "Logon Latency", base.LocalSiteName, instance.trustAllCertificates, instance.credentials.UserName, instance.VirtualDirectoryName, testUri, instance.UrlType)
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

		private string FormatExceptionText(ScenarioException scenarioException, Exception exception)
		{
			string str = exception.Message;
			if (scenarioException != null)
			{
				str = scenarioException.ToString();
			}
			return str + Environment.NewLine + this.verboseOutput.ToString();
		}

		private const string monitoringEventSourceInternal = "MSExchange Monitoring OWAConnectivity Internal";

		private const string monitoringEventSourceExternal = "MSExchange Monitoring OWAConnectivity External";

		private const string MonitoringLatencyPerfCounter = "Logon Latency";

		private static object lockObject = new object();

		private static List<string> exchangeHostNames = null;

		private static List<string> businessLiveIdHostNames = null;

		private static List<string> consumerLiveIdHostNames = null;
	}
}
