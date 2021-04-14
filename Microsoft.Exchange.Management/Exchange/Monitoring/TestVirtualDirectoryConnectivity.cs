using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class TestVirtualDirectoryConnectivity : TestCasConnectivity
	{
		internal TestVirtualDirectoryConnectivity(LocalizedString applicationName, LocalizedString applicationShortName, TransientErrorCache transientErrorCache, string monitoringEventSourceInternal, string monitoringEventSourceExternal)
		{
			this.ApplicationName = applicationName;
			this.ApplicationShortName = applicationShortName;
			this.transientErrorCache = transientErrorCache;
			this.monitoringEventSourceInternal = monitoringEventSourceInternal;
			this.monitoringEventSourceExternal = monitoringEventSourceExternal;
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Alias(new string[]
		{
			"Identity"
		})]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter ClientAccessServer
		{
			get
			{
				return this.clientAccessServer;
			}
			set
			{
				this.clientAccessServer = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter TrustAnySSLCertificate
		{
			get
			{
				return this.trustAllCertificates;
			}
			set
			{
				this.trustAllCertificates = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public OwaConnectivityTestType TestType
		{
			get
			{
				return this.testType;
			}
			set
			{
				this.testType = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public string VirtualDirectoryName { get; set; }

		private protected LocalizedString ApplicationName { protected get; private set; }

		private protected LocalizedString ApplicationShortName { protected get; private set; }

		protected bool UseInternalUrl
		{
			get
			{
				return this.TestType == OwaConnectivityTestType.Internal;
			}
		}

		protected sealed override string PerformanceObject
		{
			get
			{
				return this.MonitoringEventSource;
			}
		}

		protected sealed override string MonitoringEventSource
		{
			get
			{
				if (!this.UseInternalUrl)
				{
					return this.monitoringEventSourceExternal;
				}
				return this.monitoringEventSourceInternal;
			}
		}

		protected sealed override string TransactionFailuresEventMessage(string detailedInformation)
		{
			return Strings.CasHealthWebAppSomeTransactionsFailed(this.ApplicationShortName, detailedInformation);
		}

		protected sealed override string TransactionWarningsEventMessage(string detailedInformation)
		{
			return Strings.CasHealthWebAppTransactionWarnings(this.ApplicationShortName, detailedInformation);
		}

		protected override string TransactionSuccessEventMessage
		{
			get
			{
				return Strings.CasHealthWebAppAllTransactionsSucceeded(this.ApplicationName);
			}
		}

		protected override uint GetDefaultTimeOut()
		{
			return 30U;
		}

		internal override TransientErrorCache GetTransientErrorCache()
		{
			if (!base.MonitoringContext)
			{
				return null;
			}
			return this.transientErrorCache;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string text = "";
				if (this.TrustAnySSLCertificate)
				{
					text = text + Strings.CasHealthWarnTrustAllCertificates + "\r\n";
				}
				if (this.casToTest != null)
				{
					text = text + Strings.CasHealthWebAppConfirmTestWithServer(this.ApplicationName, this.casToTest.Fqdn) + "\r\n";
				}
				return new LocalizedString(text);
			}
		}

		protected sealed override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				this.ValidateTestWebApplicationRequirements();
			}
			finally
			{
				if (base.HasErrors && base.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected virtual void ValidateTestWebApplicationRequirements()
		{
			if (this.casToTest == null)
			{
				base.CasConnectivityWriteError(new ApplicationException(this.NoCasArgumentError), ErrorCategory.InvalidArgument, null);
			}
		}

		protected virtual LocalizedString NoCasArgumentError
		{
			get
			{
				return Strings.CasHealthWebAppNoCasArgumentError;
			}
		}

		protected override List<TestCasConnectivity.TestCasConnectivityRunInstance> PopulateInfoPerCas(TestCasConnectivity.TestCasConnectivityRunInstance clientAccessServerInstance, List<CasTransactionOutcome> outcomeList)
		{
			TaskLogger.LogEnter();
			List<TestCasConnectivity.TestCasConnectivityRunInstance> result;
			try
			{
				base.WriteVerbose(Strings.CasHealthWebAppBuildVdirList(this.ApplicationName, clientAccessServerInstance.CasFqdn));
				IEnumerable<ExchangeVirtualDirectory> virtualDirectories = this.GetVirtualDirectories(this.casToTest.Id);
				result = (from virtualDirectory in virtualDirectories
				where this.ShouldTestVirtualDirectory(clientAccessServerInstance, outcomeList, virtualDirectory)
				select this.CreateRunInstanceForVirtualDirectory(clientAccessServerInstance, virtualDirectory)).ToList<TestCasConnectivity.TestCasConnectivityRunInstance>();
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		private IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories(ADObjectId serverId)
		{
			base.TraceInfo("GetVirtualDirectories: server = {0}", new object[]
			{
				serverId
			});
			QueryFilter queryFilter = new ExistsFilter(ExchangeVirtualDirectorySchema.MetabasePath);
			if (this.VirtualDirectoryName != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.VirtualDirectoryName)
				});
			}
			ExchangeVirtualDirectory[] array = this.GetVirtualDirectories(serverId, queryFilter).ToArray<ExchangeVirtualDirectory>();
			if (array.Length == 0)
			{
				if (this.VirtualDirectoryName != null)
				{
					base.WriteErrorAndMonitoringEvent(new ApplicationException(Strings.CasHealthWebAppVdirNotFoundError(this.ApplicationShortName, this.VirtualDirectoryName)), ErrorCategory.ObjectNotFound, null, 1010, this.MonitoringEventSource, true);
				}
				else
				{
					this.WriteWarning(Strings.CasHealthWebAppNoVirtualDirectories(this.ApplicationShortName, serverId.Name));
				}
			}
			return array;
		}

		protected abstract IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories(ADObjectId serverId, QueryFilter filter);

		protected IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories<TExchangeVirtualDirectory>(ADObjectId serverId, QueryFilter filter) where TExchangeVirtualDirectory : ExchangeVirtualDirectory, new()
		{
			ADPagedReader<TExchangeVirtualDirectory> source = base.CasConfigurationSession.FindPaged<TExchangeVirtualDirectory>(serverId, QueryScope.SubTree, filter, null, 0);
			return source.Cast<ExchangeVirtualDirectory>();
		}

		private bool ShouldTestVirtualDirectory(TestCasConnectivity.TestCasConnectivityRunInstance clientAccessServerInstance, List<CasTransactionOutcome> outcomeList, ExchangeVirtualDirectory virtualDirectory)
		{
			if (this.IsOrphanVdir(virtualDirectory))
			{
				this.WriteWarning(Strings.CasHealthWebAppOrphanVirtualDirectory(this.ApplicationShortName, virtualDirectory.Name));
				CasTransactionOutcome casTransactionOutcome = this.CreateVirtualDirectoryConfigErrorOutcome(clientAccessServerInstance, virtualDirectory);
				this.TestFailedBadVdirConfig(casTransactionOutcome);
				base.WriteObject(casTransactionOutcome);
				outcomeList.Add(casTransactionOutcome);
				return false;
			}
			if ((this.UseInternalUrl && (virtualDirectory.InternalUrl == null || string.IsNullOrEmpty(virtualDirectory.InternalUrl.AbsoluteUri))) || (!this.UseInternalUrl && (virtualDirectory.ExternalUrl == null || string.IsNullOrEmpty(virtualDirectory.ExternalUrl.AbsoluteUri))))
			{
				CasTransactionOutcome casTransactionOutcome2 = this.CreateVirtualDirectoryConfigErrorOutcome(clientAccessServerInstance, virtualDirectory);
				this.TestFailedUrlPropertyNotSet(casTransactionOutcome2);
				base.WriteObject(casTransactionOutcome2);
				outcomeList.Add(casTransactionOutcome2);
				return false;
			}
			return true;
		}

		private bool IsOrphanVdir(ExchangeVirtualDirectory vdir)
		{
			string hostName = IisUtility.GetHostName(vdir.MetabasePath);
			if (string.IsNullOrEmpty(hostName))
			{
				base.TraceInfo("IsOrphanVdir: hostname from vdir.MetabasePath is null.");
				return false;
			}
			string text = TestVirtualDirectoryConnectivity.GetFirstPeriodDelimitedWord(Environment.MachineName).ToLower();
			base.TraceInfo("vdir hostname is {0}, localHost is {1}, vdir metabasepath: {2}", new object[]
			{
				hostName.ToLower(),
				text.ToLower(),
				vdir.MetabasePath
			});
			if (!TestVirtualDirectoryConnectivity.GetFirstPeriodDelimitedWord(hostName.ToLower()).Equals(text))
			{
				base.TraceInfo("IsOrphanVdir: Vdir is not on localhost, so can't check whether it is an orphan.");
				return false;
			}
			if (!IisUtility.Exists(vdir.MetabasePath))
			{
				base.TraceInfo("IsOrphanVdir: Vdir is an orphan.");
				return true;
			}
			base.TraceInfo("IsOrphanVdir: Vdir is not an orphan.");
			return false;
		}

		private TestCasConnectivity.TestCasConnectivityRunInstance CreateRunInstanceForVirtualDirectory(TestCasConnectivity.TestCasConnectivityRunInstance clientAccessServerInstance, ExchangeVirtualDirectory virtualDirectory)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance = new TestCasConnectivity.TestCasConnectivityRunInstance(clientAccessServerInstance)
			{
				UrlType = (this.UseInternalUrl ? VirtualDirectoryUriScope.Internal : VirtualDirectoryUriScope.External),
				VirtualDirectory = virtualDirectory
			};
			base.WriteVerbose(Strings.CasHealthWebAppAddingTestInstance(clientAccessServerInstance.CasFqdn, testCasConnectivityRunInstance.baseUri.AbsoluteUri));
			return testCasConnectivityRunInstance;
		}

		protected override List<CasTransactionOutcome> BuildPerformanceOutcomes(TestCasConnectivity.TestCasConnectivityRunInstance instance, string mailboxFqdn)
		{
			return new List<CasTransactionOutcome>
			{
				this.CreateLogonOutcome(instance)
			};
		}

		protected CasTransactionOutcome CreateLogonOutcome(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			return this.BuildOutcome(Strings.CasHealthWebAppLogonScenarioName, Strings.CasHealthWebAppLogonScenarioDescription(this.ApplicationName), instance);
		}

		protected virtual CasTransactionOutcome CreateVirtualDirectoryConfigErrorOutcome(TestCasConnectivity.TestCasConnectivityRunInstance clientAccessServerInstance, ExchangeVirtualDirectory virtualDirectory)
		{
			return new CasTransactionOutcome(clientAccessServerInstance.CasFqdn, Strings.CasHealthOwaLogonScenarioName, Strings.CasHealthWebAppLogonScenarioDescription(this.ApplicationName), null, base.LocalSiteName, !this.allowUnsecureAccess, clientAccessServerInstance.credentials.UserName, virtualDirectory.Name, null, this.UseInternalUrl ? VirtualDirectoryUriScope.Internal : VirtualDirectoryUriScope.External);
		}

		protected override CasTransactionOutcome BuildOutcome(string scenarioName, string scenarioDescription, TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			return this.BuildOutcome(scenarioName, scenarioDescription, "Logon Latency", instance);
		}

		protected virtual CasTransactionOutcome BuildOutcome(string scenarioName, string scenarioDescription, string perfCounterName, TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			return new CasTransactionOutcome(instance.CasFqdn, scenarioName, scenarioDescription, perfCounterName, base.LocalSiteName, !instance.allowUnsecureAccess, instance.credentials.UserName, instance.VirtualDirectoryName, instance.baseUri, instance.UrlType)
			{
				Result = 
				{
					Value = CasTransactionResultEnum.Success
				}
			};
		}

		protected static string GetFirstPeriodDelimitedWord(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			int num = s.IndexOf('.');
			if (num >= 0)
			{
				return s.Substring(0, num);
			}
			return s;
		}

		private void TestFailedBadVdirConfig(CasTransactionOutcome outcome)
		{
			string text = Strings.CasHealthWebAppBadVdirConfig(this.ApplicationShortName);
			if (base.MonitoringContext)
			{
				text += this.BuildVdirIdentityString(outcome);
			}
			base.SetTestOutcome(outcome, EventTypeEnumeration.Warning, text, null);
		}

		private void TestFailedUrlPropertyNotSet(CasTransactionOutcome outcome)
		{
			string text = (outcome.UrlType == VirtualDirectoryUriScope.Internal) ? Strings.CasHealthOwaNoInternalUrl : Strings.CasHealthOwaNoExternalUrl;
			if (base.MonitoringContext)
			{
				text += this.BuildVdirIdentityString(outcome);
			}
			base.SetTestOutcome(outcome, EventTypeEnumeration.Warning, text, null);
		}

		protected string BuildVdirIdentityString(CasTransactionOutcome outcome)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(Strings.CasHealthOwaVdirColon(outcome.VirtualDirectoryName));
			return stringBuilder.ToString();
		}

		private const uint DefaultTimeout = 30U;

		private const string MonitoringLatencyPerfCounter = "Logon Latency";

		private readonly TransientErrorCache transientErrorCache;

		private readonly string monitoringEventSourceInternal;

		private readonly string monitoringEventSourceExternal;

		private OwaConnectivityTestType testType;
	}
}
