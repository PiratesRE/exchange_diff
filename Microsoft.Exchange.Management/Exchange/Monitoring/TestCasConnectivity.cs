using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Monitoring;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.LiveIDAuthentication;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class TestCasConnectivity : Task
	{
		protected string CasFqdn
		{
			get
			{
				if (this.casToTest == null)
				{
					return null;
				}
				return this.casToTest.Fqdn;
			}
		}

		protected virtual DatacenterUserType DefaultUserType
		{
			get
			{
				return DatacenterUserType.LEGACY;
			}
		}

		protected void CasConnectivityWriteError(Exception exception, ErrorCategory category, object target)
		{
			this.CasConnectivityWriteError(exception, category, target, true);
		}

		protected void CasConnectivityWriteError(Exception exception, ErrorCategory category, object target, bool reThrow)
		{
			if (!this.MonitoringContext || reThrow)
			{
				this.WriteError(exception, category, target, reThrow);
			}
		}

		protected string ShortErrorMsgFromException(Exception exception)
		{
			string result = string.Empty;
			if (exception.InnerException != null && !(exception is StorageTransientException) && !(exception is StoragePermanentException))
			{
				result = Strings.CasHealthShortErrorMsgFromExceptionWithInnerException(exception.GetType().ToString(), exception.Message, exception.InnerException.GetType().ToString(), exception.InnerException.Message);
			}
			else
			{
				result = Strings.CasHealthShortErrorMsgFromException(exception.GetType().ToString(), exception.Message);
			}
			return result;
		}

		protected virtual CasTransactionOutcome BuildOutcome(string scenarioName, string scenarioDescription, TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			return new CasTransactionOutcome(instance.CasFqdn, scenarioName, scenarioDescription, null, this.LocalSiteName, true, instance.credentials.UserName);
		}

		protected LocalizedException ResetAutomatedCredentialsOnMailboxServer(Server server, bool onlyResetIfLocalMailbox)
		{
			bool flag = TestConnectivityCredentialsManager.IsExchangeMultiTenant();
			ADSite localSite = this.configurationSession.GetLocalSite();
			TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance = new TestCasConnectivity.TestCasConnectivityRunInstance();
			testCasConnectivityRunInstance.credentials = new NetworkCredential("", "", server.Domain);
			SmtpAddress? address;
			if (flag)
			{
				address = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(this, this.configurationSession, localSite);
			}
			else
			{
				address = TestConnectivityCredentialsManager.GetEnterpriseAutomatedTaskUser(localSite, server.Domain);
			}
			if (address == null)
			{
				return new CasHealthCouldNotBuildTestUserNameException(this.localServer.Fqdn);
			}
			testCasConnectivityRunInstance.credentials.UserName = TestCasConnectivity.GetInstanceUserNameFromTestUser(address);
			LocalizedException ex = this.SetExchangePrincipal(testCasConnectivityRunInstance);
			ex = this.ProcessSetExchangePrincipalResult(ex, testCasConnectivityRunInstance, server);
			if (ex != null)
			{
				return ex;
			}
			if (!flag)
			{
				if (onlyResetIfLocalMailbox)
				{
					this.TraceInfo("Checking if test user's mailbox is on current server: " + server.Fqdn);
					string serverFqdn = testCasConnectivityRunInstance.exchangePrincipal.MailboxInfo.Location.ServerFqdn;
					if (!serverFqdn.Equals(server.Fqdn, StringComparison.InvariantCultureIgnoreCase))
					{
						this.TraceInfo("Test user's mailbox is on " + serverFqdn + " skip restting password");
						return null;
					}
				}
				return this.ResetAutomatedCredentialsAndVerify(testCasConnectivityRunInstance);
			}
			if (this.mailboxCredential == null)
			{
				return new CasHealthCouldNotBuildTestUserNameException(this.localServer.Fqdn);
			}
			testCasConnectivityRunInstance.credentials.UserName = this.mailboxCredential.UserName;
			testCasConnectivityRunInstance.credentials.Password = this.mailboxCredential.Password.AsUnsecureString();
			return this.SaveAutomatedCredentials(testCasConnectivityRunInstance);
		}

		protected LocalizedException SaveAutomatedCredentials(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			ExDateTime now = ExDateTime.Now;
			string scenarioDescription = Strings.CasHealthResetCredentialsScenario(instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn);
			CasTransactionOutcome casTransactionOutcome = this.BuildOutcome(Strings.CasHealthScenarioResetCredentials, scenarioDescription, instance);
			LocalizedException ex = TestConnectivityCredentialsManager.SaveAutomatedCredentials(instance.exchangePrincipal, instance.credentials);
			if (ex == null)
			{
				casTransactionOutcome.Update(CasTransactionResultEnum.Success, this.ComputeLatency(now), null);
				base.WriteObject(casTransactionOutcome);
			}
			else
			{
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, this.ComputeLatency(now), this.ShortErrorMsgFromException(ex));
				base.WriteObject(casTransactionOutcome);
			}
			instance.safeToBuildUpnString = true;
			return ex;
		}

		protected LocalizedException ResetAutomatedCredentialsAndVerify(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			ExDateTime now = ExDateTime.Now;
			string scenarioDescription = Strings.CasHealthResetCredentialsScenario(instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn);
			CasTransactionOutcome casTransactionOutcome = this.BuildOutcome(Strings.CasHealthScenarioResetCredentials, scenarioDescription, instance);
			bool flag = false;
			LocalizedException ex = TestConnectivityCredentialsManager.ResetAutomatedCredentialsAndVerify(instance.exchangePrincipal, instance.credentials, this.ResetTestAccountCredentials, out flag);
			if (ex == null)
			{
				if (flag)
				{
					casTransactionOutcome.Update(CasTransactionResultEnum.Success, this.ComputeLatency(now), null);
					base.WriteObject(casTransactionOutcome);
				}
			}
			else
			{
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, this.ComputeLatency(now), this.ShortErrorMsgFromException(ex));
				base.WriteObject(casTransactionOutcome);
			}
			instance.safeToBuildUpnString = true;
			return ex;
		}

		protected LocalizedException LoadAutomatedCredentials(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			ExDateTime now = ExDateTime.Now;
			string scenarioDescription = Strings.CasHealthResetCredentialsScenario(instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn);
			CasTransactionOutcome casTransactionOutcome = this.BuildOutcome(Strings.CasHealthScenarioResetCredentials, scenarioDescription, instance);
			LocalizedException ex = TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(instance.exchangePrincipal, instance.credentials);
			if (ex != null)
			{
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, this.ComputeLatency(now), this.ShortErrorMsgFromException(ex));
				base.WriteObject(casTransactionOutcome);
			}
			instance.safeToBuildUpnString = true;
			return ex;
		}

		protected virtual List<CasTransactionOutcome> DoAutodiscoverCas(TestCasConnectivity.TestCasConnectivityRunInstance instance, List<TestCasConnectivity.TestCasConnectivityRunInstance> newInstances)
		{
			throw new NotImplementedException("DoAutodiscoverCas");
		}

		protected virtual string PerformanceObject
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected virtual string MonitoringEventSource
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected virtual string TransactionFailuresEventMessage(string detailedInformation)
		{
			return Strings.CasHealthTransactionFailures(detailedInformation);
		}

		protected virtual string TransactionWarningsEventMessage(string detailedInformation)
		{
			return Strings.CasHealthTransactionWarnings(detailedInformation);
		}

		protected abstract uint GetDefaultTimeOut();

		protected virtual string TransactionSuccessEventMessage
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected virtual List<TestCasConnectivity.TestCasConnectivityRunInstance> PopulateInfoPerCas(TestCasConnectivity.TestCasConnectivityRunInstance instance, List<CasTransactionOutcome> outcomeList)
		{
			throw new NotImplementedException("PopulateInfoPerCas");
		}

		protected virtual List<CasTransactionOutcome> ExecuteTests(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			throw new NotImplementedException("ExecuteTests");
		}

		internal virtual TransientErrorCache GetTransientErrorCache()
		{
			return null;
		}

		internal virtual void UpdateTransientErrorCache(TestCasConnectivity.TestCasConnectivityRunInstance instance, int cFailed, int cWarning, ref int cFailedTransactions, ref int cWarningTransactions, StringBuilder failedStr, StringBuilder warningStr, ref StringBuilder failedTransactionsStr, ref StringBuilder warningTransactionsStr)
		{
			TransientErrorCache transientErrorCache = this.GetTransientErrorCache();
			string text = (instance.exchangePrincipal != null) ? instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn : null;
			if (cFailed > 0 || cWarning > 0)
			{
				if (transientErrorCache != null && !string.IsNullOrEmpty(text) && !transientErrorCache.ContainsError(text))
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_TransientErrorCacheInsertEntry, new string[]
					{
						(null != instance.baseUri) ? instance.baseUri.ToString() : string.Empty,
						text,
						failedStr.ToString(),
						warningStr.ToString()
					});
					transientErrorCache.Add(new CASServiceError(text));
					return;
				}
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_TransientErrorCacheFindEntry, new string[]
				{
					(null != instance.baseUri) ? instance.baseUri.ToString() : string.Empty,
					text,
					failedStr.ToString(),
					warningStr.ToString()
				});
				if (cFailed > 0)
				{
					cFailedTransactions += cFailed;
					failedTransactionsStr.Append(failedStr);
				}
				if (cWarning > 0)
				{
					cWarningTransactions += cWarning;
					warningTransactionsStr.Append(warningStr);
					return;
				}
			}
			else if (transientErrorCache != null && !string.IsNullOrEmpty(text))
			{
				if (transientErrorCache.ContainsError(text))
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_TransientErrorCacheRemoveEntry, new string[]
					{
						(null != instance.baseUri) ? instance.baseUri.ToString() : string.Empty,
						text
					});
				}
				transientErrorCache.Remove(text);
			}
		}

		private IAsyncResult BeginExecute(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			instance.Result = new AsyncResult<CasTransactionOutcome>();
			this.InvokeExecuteTests(instance);
			if (!this.MonitoringContext)
			{
				uint seconds = (this.Timeout <= 0U || this.Timeout > 3600U) ? this.GetDefaultTimeOut() : this.Timeout;
				this.WaitAll(new IAsyncResult[]
				{
					instance.Result
				}, ExDateTime.Now.Add(new TimeSpan(0, 0, (int)seconds)), instance.Outcomes);
			}
			return instance.Result;
		}

		private void InvokeExecuteTests(object state)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance instance = state as TestCasConnectivity.TestCasConnectivityRunInstance;
			this.ExecuteTests(instance);
		}

		[Parameter]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ResetTestAccountCredentials
		{
			get
			{
				return (bool)(base.Fields["ResetTestAccountCredentials"] ?? false);
			}
			set
			{
				base.Fields["ResetTestAccountCredentials"] = value.IsPresent;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value.IsPresent;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter LightMode
		{
			get
			{
				return (bool)(base.Fields["LightMode"] ?? false);
			}
			set
			{
				base.Fields["LightMode"] = value.IsPresent;
			}
		}

		[Parameter(Mandatory = false)]
		public uint Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public ServerIdParameter MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
			set
			{
				this.mailboxServer = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public DatacenterUserType UserType
		{
			get
			{
				return (DatacenterUserType)(base.Fields["UserType"] ?? this.DefaultUserType);
			}
			set
			{
				base.Fields["UserType"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || MonitoringHelper.IsKnownExceptionForMonitoring(exception);
		}

		internal ITopologyConfigurationSession CasConfigurationSession
		{
			get
			{
				return this.configurationSession;
			}
		}

		internal IRecipientSession CasGlobalCatalogSession
		{
			get
			{
				return this.globalCatalogSession;
			}
		}

		internal string LocalSiteName
		{
			get
			{
				return this.CasConfigurationSession.GetLocalSite().Name;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 1218, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Cas\\TestCasConnectivity.cs");
			this.configurationSession.ServerTimeout = new TimeSpan?(new TimeSpan(0, 0, 30000));
			this.globalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, sessionSettings, 1226, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Cas\\TestCasConnectivity.cs");
			if (!this.globalCatalogSession.IsReadConnectionAvailable())
			{
				this.globalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 1234, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Cas\\TestCasConnectivity.cs");
			}
			this.globalCatalogSession.ServerTimeout = new TimeSpan?(new TimeSpan(0, 0, 30000));
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				this.InitializeTopologyInformation();
			}
			catch (ADTransientException exception)
			{
				this.CasConnectivityWriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			finally
			{
				if (base.HasErrors && this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		private Server GetMailboxServerObjectFromParam()
		{
			if (this.mailboxServer == null)
			{
				return null;
			}
			LocalizedString? localizedString;
			IEnumerable<Server> objects = this.mailboxServer.GetObjects<Server>(null, this.CasConfigurationSession, null, out localizedString);
			IEnumerator<Server> enumerator = objects.GetEnumerator();
			if (base.HasErrors || !enumerator.MoveNext())
			{
				CasHealthMailboxServerNotFoundException exception = new CasHealthMailboxServerNotFoundException(this.mailboxServer.ToString());
				this.CasConnectivityWriteError(exception, ErrorCategory.ObjectNotFound, null);
				return null;
			}
			Server server = enumerator.Current;
			if (server.MajorVersion != Server.CurrentExchangeMajorVersion)
			{
				this.TraceInfo("The specified mailbox server is not on Exchange {0}", new object[]
				{
					Server.CurrentExchangeMajorVersion
				});
				this.CasConnectivityWriteError(new LocalizedException(Strings.CasHealthSpecifiedMailboxServerVersionNotMatched(base.CommandRuntime.ToString(), server.MajorVersion)), ErrorCategory.InvalidArgument, null);
			}
			if (enumerator.MoveNext())
			{
				this.WriteWarning(Strings.CasHealthTestMultipleMailboxServersFound);
			}
			return server;
		}

		private string GenerateDeviceId()
		{
			IPGlobalProperties ipglobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			int num = string.Concat(new string[]
			{
				ipglobalProperties.HostName,
				".",
				ipglobalProperties.DomainName,
				":",
				this.monitoringInstance,
				":",
				this.autodiscoverCas ? "1" : "0"
			}).GetHashCode();
			if (num < 0)
			{
				num = -num;
			}
			string text = num.ToString();
			this.TraceInfo("DeviceId generated: " + text);
			return text;
		}

		private List<TestCasConnectivity.TestCasConnectivityRunInstance> BuildRunInstances()
		{
			string deviceId = this.GenerateDeviceId();
			List<TestCasConnectivity.TestCasConnectivityRunInstance> result;
			if (this.mailboxCredential == null)
			{
				result = this.BuildRunInstanceForSiteMBox(deviceId);
			}
			else
			{
				result = this.BuildRunInstancesForOneMbox(deviceId);
			}
			return result;
		}

		private List<TestCasConnectivity.TestCasConnectivityRunInstance> BuildRunInstancesForOneMbox(string deviceId)
		{
			List<TestCasConnectivity.TestCasConnectivityRunInstance> list = new List<TestCasConnectivity.TestCasConnectivityRunInstance>();
			List<CasTransactionOutcome> list2 = new List<CasTransactionOutcome>();
			TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance = new TestCasConnectivity.TestCasConnectivityRunInstance();
			testCasConnectivityRunInstance.CasFqdn = this.CasFqdn;
			List<TestCasConnectivity.TestCasConnectivityRunInstance> result;
			try
			{
				if (TestConnectivityCredentialsManager.IsExchangeMultiTenant())
				{
					testCasConnectivityRunInstance.credentials = new NetworkCredential(this.mailboxCredential.UserName, this.mailboxCredential.Password.AsUnsecureString(), (this.casToTest != null) ? this.casToTest.Domain : string.Empty);
				}
				else
				{
					testCasConnectivityRunInstance.credentials = this.mailboxCredential.GetNetworkCredential();
				}
				testCasConnectivityRunInstance.deviceId = deviceId;
				testCasConnectivityRunInstance.allowUnsecureAccess = this.allowUnsecureAccess;
				testCasConnectivityRunInstance.trustAllCertificates = this.trustAllCertificates;
				bool flag2;
				if (this.clientAccessServer == null && this.autodiscoverCas)
				{
					LocalizedException ex = this.SetExchangePrincipal(testCasConnectivityRunInstance);
					if (ex != null)
					{
						LocalizedException exception = new CasHealthCouldNotLogUserForAutodiscoverException(testCasConnectivityRunInstance.credentials.Domain, testCasConnectivityRunInstance.credentials.UserName, this.ShortErrorMsgFromException(ex));
						this.CasConnectivityWriteError(exception, ErrorCategory.ObjectNotFound, null);
						return null;
					}
					this.TraceInfo("Calling LogonUser for user " + testCasConnectivityRunInstance.credentials.Domain + "\\" + testCasConnectivityRunInstance.credentials.UserName);
					SafeUserTokenHandle safeUserTokenHandle;
					bool flag = NativeMethods.LogonUser(testCasConnectivityRunInstance.credentials.UserName, testCasConnectivityRunInstance.credentials.Domain, testCasConnectivityRunInstance.credentials.Password, 8, 0, out safeUserTokenHandle);
					safeUserTokenHandle.Dispose();
					if (!flag)
					{
						ex = new CasHealthCouldNotLogUserException(TestConnectivityCredentialsManager.GetDomainUserNameFromCredentials(testCasConnectivityRunInstance.credentials), "", TestConnectivityCredentialsManager.NewTestUserScriptName, Marshal.GetLastWin32Error().ToString());
						this.CasConnectivityWriteError(ex, ErrorCategory.ObjectNotFound, null, false);
						return null;
					}
					List<TestCasConnectivity.TestCasConnectivityRunInstance> list3 = new List<TestCasConnectivity.TestCasConnectivityRunInstance>();
					this.DoAutodiscoverCas(testCasConnectivityRunInstance, list3);
					if (list3.Count == 0)
					{
						return null;
					}
					foreach (TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance2 in list3)
					{
						testCasConnectivityRunInstance2.CasFqdn = this.CasFqdn;
					}
					list.AddRange(list3);
					flag2 = true;
					if (null == testCasConnectivityRunInstance.baseUri)
					{
						return null;
					}
					this.TraceInfo("Exchange Autodiscovery found CAS URI: " + testCasConnectivityRunInstance.baseUri);
				}
				else
				{
					list.AddRange(this.PopulateInfoPerCas(testCasConnectivityRunInstance, list2));
					flag2 = true;
				}
				if (list2.Count > 0)
				{
					this.WriteMonitoringEventForOutcomes(list2, 1010, EventTypeEnumeration.Warning);
				}
				else if (flag2)
				{
					this.WriteMonitoringEvent(1011, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.CasConfigurationCheckSuccess);
				}
				result = list;
			}
			catch (PSArgumentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, this.mailboxCredential);
				result = null;
			}
			return result;
		}

		private LocalizedException ProcessSetExchangePrincipalResult(LocalizedException originalException, TestCasConnectivity.TestCasConnectivityRunInstance instance, Server server)
		{
			LocalizedException result = null;
			if (this.MonitoringContext)
			{
				return originalException;
			}
			if (originalException != null)
			{
				bool flag = TestConnectivityCredentialsManager.IsExchangeMultiTenant();
				this.TraceInfo("Exception getting ex principal mailbox server {0}, exception {1}", new object[]
				{
					server.Name,
					originalException.Message
				});
				if (flag)
				{
					this.WriteWarning(Strings.CasHealthTestUserInDataCenterNotAccessible(TestConnectivityCredentialsManager.GetAutomatedTaskDataCenterDomain(this.configurationSession.GetLocalSite())));
				}
				else
				{
					this.WriteWarning(Strings.CasHealthTestUserNotAccessible(instance.credentials.UserName));
				}
				if (!(originalException is CasHealthUserNotFoundException))
				{
					string errorString = this.ShortErrorMsgFromException(originalException);
					if (flag)
					{
						result = new CasHealthCouldNotLogUserDataCenterException(TestConnectivityCredentialsManager.GetAutomatedTaskDataCenterDomain(this.configurationSession.GetLocalSite()), TestConnectivityCredentialsManager.NewTestUserScriptName, errorString);
					}
					else
					{
						result = new CasHealthCouldNotLogUserException(TestConnectivityCredentialsManager.GetDomainUserNameFromCredentials(instance.credentials), (server.Fqdn == null) ? "" : server.Fqdn, TestConnectivityCredentialsManager.NewTestUserScriptName, errorString);
					}
				}
				else if (flag)
				{
					result = new CasHealthCouldNotLogUserDataCenterNoDetailedInfoException(TestConnectivityCredentialsManager.GetAutomatedTaskDataCenterDomain(this.configurationSession.GetLocalSite()), TestConnectivityCredentialsManager.NewTestUserScriptName);
				}
				else
				{
					result = new CasHealthCouldNotLogUserNoDetailedInfoException(TestConnectivityCredentialsManager.GetDomainUserNameFromCredentials(instance.credentials), (server.Fqdn == null) ? "" : server.Fqdn, TestConnectivityCredentialsManager.NewTestUserScriptName);
				}
			}
			return result;
		}

		private List<TestCasConnectivity.TestCasConnectivityRunInstance> BuildRunInstanceForSiteMBox(string deviceId)
		{
			if (this.casToTest == null)
			{
				CasHealthTaskNotRunOnExchangeServerException exception = new CasHealthTaskNotRunOnExchangeServerException();
				this.WriteErrorAndMonitoringEvent(exception, ErrorCategory.PermissionDenied, null, 1002, this.MonitoringEventSource, true);
				return null;
			}
			if (this.casToTest.ServerSite == null)
			{
				CasHealthTaskCasHasNoTopologySiteException exception2 = new CasHealthTaskCasHasNoTopologySiteException();
				this.WriteErrorAndMonitoringEvent(exception2, ErrorCategory.PermissionDenied, null, 1002, this.MonitoringEventSource, true);
				return null;
			}
			List<TestCasConnectivity.TestCasConnectivityRunInstance> list = new List<TestCasConnectivity.TestCasConnectivityRunInstance>();
			List<CasTransactionOutcome> list2 = new List<CasTransactionOutcome>();
			List<CasTransactionOutcome> list3 = new List<CasTransactionOutcome>();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			ADSite localSite = this.configurationSession.GetLocalSite();
			List<CasTransactionOutcome> list4 = new List<CasTransactionOutcome>();
			List<CasTransactionOutcome> list5 = new List<CasTransactionOutcome>();
			bool flag4 = false;
			TransientErrorCache transientErrorCache = this.GetTransientErrorCache();
			Server server = this.casToTest;
			TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance = new TestCasConnectivity.TestCasConnectivityRunInstance();
			testCasConnectivityRunInstance.CasFqdn = this.CasFqdn;
			testCasConnectivityRunInstance.deviceId = deviceId;
			testCasConnectivityRunInstance.allowUnsecureAccess = this.allowUnsecureAccess;
			testCasConnectivityRunInstance.trustAllCertificates = this.trustAllCertificates;
			SmtpAddress? address;
			if (TestConnectivityCredentialsManager.IsExchangeMultiTenant())
			{
				address = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(this, this.configurationSession, localSite, this.UserType);
			}
			else
			{
				address = TestConnectivityCredentialsManager.GetEnterpriseAutomatedTaskUser(localSite, this.casToTest.Domain);
			}
			testCasConnectivityRunInstance.credentials = new NetworkCredential();
			testCasConnectivityRunInstance.credentials.UserName = TestCasConnectivity.GetInstanceUserNameFromTestUser(address);
			if (testCasConnectivityRunInstance.credentials.UserName == null)
			{
				this.WriteError(new CasHealthCouldNotBuildTestUserNameException(server.Fqdn), ErrorCategory.ObjectNotFound, null, false);
			}
			else
			{
				testCasConnectivityRunInstance.credentials.Domain = this.casToTest.Domain;
				testCasConnectivityRunInstance.safeToBuildUpnString = true;
				LocalizedException ex = this.SetExchangePrincipal(testCasConnectivityRunInstance);
				ex = this.ProcessSetExchangePrincipalResult(ex, testCasConnectivityRunInstance, server);
				if (ex != null)
				{
					this.CasConnectivityWriteError(ex, ErrorCategory.ObjectNotFound, null, false);
					List<CasTransactionOutcome> list6 = this.BuildFailedPerformanceOutcomesWithMessage(testCasConnectivityRunInstance, this.ShortErrorMsgFromException(ex), this.casToTest.Fqdn);
					if (list6 != null)
					{
						list5.AddRange(list6);
					}
				}
				else
				{
					string serverFqdn = testCasConnectivityRunInstance.exchangePrincipal.MailboxInfo.Location.ServerFqdn;
					ex = this.VerifyMailboxServerIsInSite(testCasConnectivityRunInstance.credentials.UserName, localSite, serverFqdn);
					flag3 = true;
					if (!this.MonitoringContext && !TestConnectivityCredentialsManager.IsExchangeMultiTenant())
					{
						ex = this.ResetAutomatedCredentialsAndVerify(testCasConnectivityRunInstance);
					}
					else
					{
						ex = this.LoadAutomatedCredentials(testCasConnectivityRunInstance);
						testCasConnectivityRunInstance.safeToBuildUpnString = true;
					}
					if (ex != null)
					{
						List<CasTransactionOutcome> list7 = this.BuildFailedPerformanceOutcomesWithMessage(testCasConnectivityRunInstance, this.ShortErrorMsgFromException(ex), serverFqdn);
						if (list7 != null)
						{
							if (ex is CasHealthInstructResetCredentialsException)
							{
								list5.AddRange(list7);
							}
							else
							{
								list4.AddRange(list7);
							}
						}
					}
					else
					{
						flag3 = true;
						bool flag5;
						if (Datacenter.IsMicrosoftHostedOnly(false))
						{
							flag5 = true;
						}
						else
						{
							string text = string.Empty;
							if (Datacenter.IsPartnerHostedOnly(false))
							{
								text = testCasConnectivityRunInstance.credentials.UserName;
							}
							else
							{
								text = testCasConnectivityRunInstance.credentials.UserName + "@" + testCasConnectivityRunInstance.credentials.Domain;
							}
							this.TraceInfo("Calling LogonUser for user " + text);
							Microsoft.Exchange.Diagnostics.Components.Monitoring.ExTraceGlobals.MonitoringTasksTracer.TraceDebug<string>((long)this.GetHashCode(), "BuildRunInstancesForCasMboxTuples: calling LogonUser for User: {0}", text);
							SafeUserTokenHandle safeUserTokenHandle;
							flag5 = NativeMethods.LogonUser(text, null, testCasConnectivityRunInstance.credentials.Password, 8, 0, out safeUserTokenHandle);
							safeUserTokenHandle.Dispose();
						}
						if (!flag5)
						{
							CasHealthCouldNotLogUserException exception3 = new CasHealthCouldNotLogUserException(TestConnectivityCredentialsManager.GetDomainUserNameFromCredentials(testCasConnectivityRunInstance.credentials), serverFqdn, TestConnectivityCredentialsManager.NewTestUserScriptName, Marshal.GetLastWin32Error().ToString());
							this.CasConnectivityWriteError(exception3, ErrorCategory.PermissionDenied, null, false);
							List<CasTransactionOutcome> list8 = this.BuildFailedPerformanceOutcomesWithMessage(testCasConnectivityRunInstance, this.ShortErrorMsgFromException(exception3), testCasConnectivityRunInstance.exchangePrincipal.MailboxInfo.Location.ServerFqdn);
							if (list8 != null)
							{
								list5.AddRange(list8);
							}
						}
						else
						{
							flag2 = true;
							if (this.autodiscoverCas)
							{
								List<TestCasConnectivity.TestCasConnectivityRunInstance> list9 = new List<TestCasConnectivity.TestCasConnectivityRunInstance>();
								List<CasTransactionOutcome> list10 = this.DoAutodiscoverCas(testCasConnectivityRunInstance, list9);
								if (list9.Count == 0)
								{
									if (list10 != null && list10.Count != 0)
									{
										flag4 = true;
										list2.AddRange(list10);
									}
								}
								else
								{
									foreach (TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance2 in list9)
									{
										testCasConnectivityRunInstance.CasFqdn = this.CasFqdn;
									}
									list.AddRange(list9);
									this.TraceInfo("Exchange Autodiscovery found CAS URI: " + testCasConnectivityRunInstance.baseUri);
								}
							}
							else
							{
								list.AddRange(this.PopulateInfoPerCas(testCasConnectivityRunInstance, list3));
								flag = true;
							}
						}
					}
				}
			}
			if (flag4)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (CasTransactionOutcome casTransactionOutcome in list2)
				{
					stringBuilder.Append(casTransactionOutcome.ToString() + "\r\n\r\n");
				}
				this.WriteMonitoringEvent(1003, this.MonitoringEventSource, EventTypeEnumeration.Warning, stringBuilder.ToString());
			}
			else if (this.autodiscoverCas)
			{
				this.WriteMonitoringEvent(1004, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.AutodiscoverySuccess);
			}
			if (list3.Count > 0)
			{
				this.WriteMonitoringEventForOutcomes(list3, 1010, EventTypeEnumeration.Warning);
			}
			else if (flag)
			{
				this.WriteMonitoringEvent(1011, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.CasConfigurationCheckSuccess);
			}
			if (list5.Count != 0)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (CasTransactionOutcome casTransactionOutcome2 in list5)
				{
					if (casTransactionOutcome2.LocalSite != null)
					{
						stringBuilder2.Append(Strings.LocalSiteColon + casTransactionOutcome2.LocalSite + "\r\n\r\n");
					}
					stringBuilder2.Append(casTransactionOutcome2.Error + "\r\n\r\n\r\n");
				}
				this.WriteMonitoringEvent(1008, this.MonitoringEventSource, EventTypeEnumeration.Warning, Strings.InstructResetCredentials(stringBuilder2.ToString()));
			}
			else if (flag2 && list4.Count == 0)
			{
				this.WriteMonitoringEvent(1007, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.LoadCredentialsSuccess);
			}
			if (list4.Count != 0)
			{
				bool flag6 = false;
				StringBuilder stringBuilder3 = new StringBuilder();
				foreach (CasTransactionOutcome casTransactionOutcome3 in list4)
				{
					string text2 = string.Empty;
					string text3 = string.Empty;
					if (casTransactionOutcome3.LocalSite != null)
					{
						text3 = casTransactionOutcome3.LocalSite.ToString();
						text2 = Strings.LocalSiteColon + casTransactionOutcome3.LocalSite + "\r\n\r\n";
					}
					text2 = text2 + casTransactionOutcome3.Error + "\r\n\r\n\r\n";
					if (!string.IsNullOrEmpty(text3) && transientErrorCache != null && !transientErrorCache.ContainsError(text3))
					{
						transientErrorCache.Add(new CASServiceError(text3));
					}
					else
					{
						stringBuilder3.Append(text2);
						flag6 = true;
					}
					this.WriteMomPerformanceCounters(casTransactionOutcome3);
				}
				if (flag6)
				{
					this.WriteMonitoringEvent(1005, this.MonitoringEventSource, EventTypeEnumeration.Error, Strings.AccessStoreError(stringBuilder3.ToString()));
				}
			}
			else if (flag3)
			{
				this.WriteMonitoringEvent(1006, this.MonitoringEventSource, EventTypeEnumeration.Success, Strings.AccessStoreSuccess);
			}
			return list;
		}

		private LocalizedException VerifyMailboxServerIsInSite(string userName, ADSite localSite, string serverFqdn)
		{
			LocalizedException result = null;
			Server server = this.FindServerByHostName(serverFqdn);
			if (server == null || server.ServerSite == null || !server.ServerSite.DistinguishedName.Equals(localSite.DistinguishedName, StringComparison.InvariantCultureIgnoreCase))
			{
				result = new CasHealthTestUserOnWrongSiteException(userName, localSite.Name, server.ServerSite.Name);
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				this.DetermineCasServer();
				if (this.monitoringInstance == null)
				{
					this.monitoringInstance = "";
				}
				List<TestCasConnectivity.TestCasConnectivityRunInstance> list = new List<TestCasConnectivity.TestCasConnectivityRunInstance>();
				if (this.ResetTestAccountCredentials)
				{
					LocalizedException ex = null;
					Server server = null;
					if (this.mailboxServer != null)
					{
						server = this.GetMailboxServerObjectFromParam();
					}
					else if (this.localServer != null && this.localServer.IsMailboxServer)
					{
						server = this.localServer;
					}
					else
					{
						ex = new CasHealthSpecifyMailboxForResetCredentialsException();
					}
					if (server != null)
					{
						ex = this.ResetAutomatedCredentialsOnMailboxServer(server, false);
					}
					if (ex != null)
					{
						this.CasConnectivityWriteError(ex, ErrorCategory.ObjectNotFound, null, false);
					}
				}
				else
				{
					list = this.BuildRunInstances();
					if (list == null)
					{
						return;
					}
					if (list.Count == 0 && !this.ResetTestAccountCredentials)
					{
						this.WriteWarning(Strings.CasHealthNoTuplesToTest);
						if (this.HandleNoRunInstances())
						{
							return;
						}
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				int num = 0;
				int num2 = 0;
				if (this.ResetTestAccountCredentials)
				{
					list.Clear();
				}
				List<IAsyncResult> list2 = new List<IAsyncResult>();
				Queue queue = Queue.Synchronized(new Queue());
				foreach (TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance in list)
				{
					testCasConnectivityRunInstance.Outcomes = queue;
					testCasConnectivityRunInstance.LightMode = this.LightMode;
					testCasConnectivityRunInstance.MonitoringContext = this.MonitoringContext;
					list2.Add(this.BeginExecute(testCasConnectivityRunInstance));
				}
				uint seconds = (this.Timeout <= 0U || this.Timeout > 3600U) ? this.GetDefaultTimeOut() : this.Timeout;
				if (this.MonitoringContext)
				{
					this.WaitAll(list2.ToArray(), ExDateTime.Now.Add(new TimeSpan(0, 0, (int)seconds)), queue);
				}
				foreach (TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance2 in list)
				{
					if (testCasConnectivityRunInstance2.Outcomes.Count <= 0)
					{
						break;
					}
					lock (testCasConnectivityRunInstance2.Outcomes.SyncRoot)
					{
						foreach (object output in testCasConnectivityRunInstance2.Outcomes)
						{
							this.WriteToConsole(output);
						}
					}
				}
				foreach (TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance3 in list)
				{
					using (AsyncResult<CasTransactionOutcome> result = testCasConnectivityRunInstance3.Result)
					{
						List<CasTransactionOutcome> list3 = result.Outcomes;
						bool flag2 = result.DidTimeout();
						string mailboxFqdn = (testCasConnectivityRunInstance3.exchangePrincipal != null) ? testCasConnectivityRunInstance3.exchangePrincipal.MailboxInfo.Location.ServerFqdn : null;
						if (flag2)
						{
							list3 = this.BuildFailedPerformanceOutcomesWithMessage(testCasConnectivityRunInstance3, Strings.CasConnectivityTaskTimeout(seconds), mailboxFqdn);
							if (list3 != null && list3.Count > 0)
							{
								this.WriteToConsole(list3[0]);
							}
						}
						StringBuilder stringBuilder3 = new StringBuilder();
						StringBuilder stringBuilder4 = new StringBuilder();
						int num3 = 0;
						int num4 = 0;
						foreach (CasTransactionOutcome casTransactionOutcome in list3)
						{
							if (casTransactionOutcome.ClientAccessServer != null)
							{
								this.WriteMomPerformanceCounters(casTransactionOutcome);
								if (CasTransactionResultEnum.Success == casTransactionOutcome.Result.Value)
								{
									lock (testCasConnectivityRunInstance3.Outcomes.SyncRoot)
									{
										testCasConnectivityRunInstance3.Outcomes.Clear();
										continue;
									}
								}
								string transactionTarget = casTransactionOutcome.ClientAccessServer.ToString() + "|" + casTransactionOutcome.LocalSite;
								StringBuilder stringBuilder5 = new StringBuilder();
								if (testCasConnectivityRunInstance3.Outcomes.Count > 0)
								{
									while (testCasConnectivityRunInstance3.Outcomes.Count > 0)
									{
										object obj = testCasConnectivityRunInstance3.Outcomes.Dequeue();
										stringBuilder5.AppendLine();
										if (obj is Warning)
										{
											stringBuilder5.AppendLine();
										}
										stringBuilder5.Append(obj);
									}
									stringBuilder5.AppendLine();
								}
								if (casTransactionOutcome.EventType == EventTypeEnumeration.Warning)
								{
									stringBuilder4.Append(Strings.CasHealthTransactionFailedSummary(transactionTarget, casTransactionOutcome.Error, stringBuilder5.ToString()));
									num4++;
								}
								else
								{
									stringBuilder3.Append(Strings.CasHealthTransactionFailedSummary(transactionTarget, casTransactionOutcome.Error, stringBuilder5.ToString()));
									num3++;
								}
							}
						}
						this.UpdateTransientErrorCache(testCasConnectivityRunInstance3, num3, num4, ref num, ref num2, stringBuilder3, stringBuilder4, ref stringBuilder, ref stringBuilder2);
					}
				}
				if (num > 0 || num2 > 0)
				{
					if (num > 0)
					{
						stringBuilder.AppendLine(Strings.CasHealthPowerShellCmdletExecutionSummary(Environment.MachineName));
						this.WriteMonitoringEvent(1001, this.MonitoringEventSource, EventTypeEnumeration.Error, this.TransactionFailuresEventMessage(stringBuilder.ToString()));
					}
					if (num2 > 0)
					{
						stringBuilder2.AppendLine(Strings.CasHealthPowerShellCmdletExecutionSummary(Environment.MachineName));
						this.WriteMonitoringEvent(1012, this.MonitoringEventSource, EventTypeEnumeration.Warning, this.TransactionWarningsEventMessage(stringBuilder2.ToString()));
					}
				}
				else if (list.Count != 0)
				{
					this.WriteMonitoringEvent(1000, this.MonitoringEventSource, EventTypeEnumeration.Success, this.TransactionSuccessEventMessage);
				}
			}
			catch (StorageTransientException exception)
			{
				this.CasConnectivityWriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			catch (ADTransientException exception2)
			{
				this.CasConnectivityWriteError(exception2, ErrorCategory.ObjectNotFound, null);
			}
			catch (DataValidationException ex2)
			{
				CasHealthDataValidationErrorException exception3 = new CasHealthDataValidationErrorException(ex2.ToString());
				this.WriteErrorAndMonitoringEvent(exception3, ErrorCategory.InvalidData, null, 1009, this.MonitoringEventSource, true);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					this.WriteMonitoringData();
				}
				TaskLogger.LogExit();
			}
		}

		internal void WriteMonitoringEventForOutcomes(List<CasTransactionOutcome> outcomes, int eventId, EventTypeEnumeration eventType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CasTransactionOutcome casTransactionOutcome in outcomes)
			{
				stringBuilder.Append(casTransactionOutcome.Error + "\r\n\r\n");
			}
			this.WriteMonitoringEvent(eventId, this.MonitoringEventSource, EventTypeEnumeration.Warning, stringBuilder.ToString());
		}

		protected void WriteMonitoringData()
		{
			base.WriteObject(this.monitoringData);
		}

		protected virtual bool HandleNoRunInstances()
		{
			return false;
		}

		protected abstract List<CasTransactionOutcome> BuildPerformanceOutcomes(TestCasConnectivity.TestCasConnectivityRunInstance instance, string mailboxFqdn);

		private List<CasTransactionOutcome> BuildFailedPerformanceOutcomesWithMessage(TestCasConnectivity.TestCasConnectivityRunInstance instance, string message, string mailboxFqdn)
		{
			List<CasTransactionOutcome> list = this.BuildPerformanceOutcomes(instance, mailboxFqdn);
			if (list != null)
			{
				foreach (CasTransactionOutcome casTransactionOutcome in list)
				{
					casTransactionOutcome.Update(CasTransactionResultEnum.Failure, new TimeSpan(0, 0, 0, 0, -1000), message);
				}
			}
			return list;
		}

		private void WriteMomPerformanceCounters(CasTransactionOutcome outcome)
		{
			string text = outcome.ClientAccessServer.ToString() + "|" + outcome.LocalSite;
			if (!string.IsNullOrEmpty(outcome.PerformanceCounterName))
			{
				double num;
				if (CasTransactionResultEnum.Success == outcome.Result.Value)
				{
					num = outcome.Latency.TotalMilliseconds;
				}
				else
				{
					num = -1000.0;
				}
				this.TraceInfo(string.Concat(new object[]
				{
					"Outputing performance counter instance ",
					text,
					" with latency ",
					num
				}));
				this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(this.PerformanceObject, outcome.PerformanceCounterName, text, num));
			}
		}

		internal static string GetInstanceUserNameFromTestUser(SmtpAddress? address)
		{
			if (address == null)
			{
				return null;
			}
			if (Datacenter.IsMultiTenancyEnabled())
			{
				return address.Value.ToString();
			}
			return address.Value.Local;
		}

		protected LocalizedException SetExchangePrincipal(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			this.TraceInfo(string.Concat(new string[]
			{
				"Looking up user in ActiveDirectory: userName: '",
				instance.credentials.UserName,
				"' domain: '",
				instance.credentials.Domain,
				"'"
			}));
			if (instance.exchangePrincipal != null)
			{
				return null;
			}
			ADUser aduser = null;
			string text = null;
			if (instance.safeToBuildUpnString)
			{
				string text2 = instance.credentials.UserName;
				if (!Datacenter.IsMultiTenancyEnabled())
				{
					text2 = text2 + "@" + instance.credentials.Domain;
				}
				this.TraceInfo("UPN is {0}", new object[]
				{
					text2
				});
				try
				{
					using (WindowsIdentity windowsIdentity = new WindowsIdentity(text2))
					{
						aduser = (ADUser)this.CasGlobalCatalogSession.FindBySid(windowsIdentity.User);
					}
				}
				catch (SecurityException ex)
				{
					this.TraceInfo("new WindowsIdentity has security exception: {0}", new object[]
					{
						ex.Message
					});
					return new CasHealthUserNotFoundException(text2, this.ShortErrorMsgFromException(ex));
				}
				if (aduser == null)
				{
					this.TraceInfo("FindBySid returned null");
					return new CasHealthUserNotFoundException(text2, "");
				}
				text = text2;
			}
			else
			{
				string domain;
				if (Datacenter.IsMultiTenancyEnabled())
				{
					SmtpAddress smtpAddress = SmtpAddress.Parse(instance.credentials.UserName);
					text = smtpAddress.Local;
					domain = smtpAddress.Domain;
				}
				else
				{
					text = instance.credentials.UserName;
					domain = instance.credentials.Domain;
				}
				if (string.IsNullOrEmpty(domain))
				{
					if (this.casToTest != null)
					{
						domain = this.casToTest.Domain;
					}
					else
					{
						domain = this.localServer.Domain;
					}
				}
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(domain))
				{
					MailboxIdParameter mailboxIdParameter = new MailboxIdParameter(string.Format("{0}\\{1}", domain, text));
					foreach (ADUser aduser2 in mailboxIdParameter.GetObjects<ADUser>(null, this.CasGlobalCatalogSession))
					{
						if (aduser != null)
						{
							throw new NonUniqueRecipientException(mailboxIdParameter.ToString(), new ObjectValidationError(DirectoryStrings.ErrorNonUniqueDomainAccount(domain, text), null, string.Empty));
						}
						aduser = aduser2;
					}
				}
				if (aduser == null)
				{
					this.TraceInfo("AD FindByAccountName returned null.");
					return new CasHealthUserNotFoundException(text, "");
				}
				this.TraceInfo("Found user in the ActiveDirectory!");
			}
			this.TraceInfo("Found user in the ActiveDirectory!");
			this.TraceInfo("Building Exchange Principal...");
			try
			{
				instance.exchangePrincipal = ExchangePrincipal.FromADUser(aduser, null);
			}
			catch (ObjectNotFoundException ex2)
			{
				this.TraceInfo("ObjectNotFoundException: {0}", new object[]
				{
					ex2.Message
				});
				return new CasHealthMailboxNotFoundException(text);
			}
			this.TraceInfo("Exchange Principal built.");
			return null;
		}

		protected void InitializeTopologyInformation()
		{
			if (this.thisFqdn == null)
			{
				this.thisFqdn = NativeHelpers.GetLocalComputerFqdn(false);
				if (this.thisFqdn == null)
				{
					CasHealthCouldNotObtainFqdnException exception = new CasHealthCouldNotObtainFqdnException();
					this.CasConnectivityWriteError(exception, ErrorCategory.PermissionDenied, null);
				}
				this.localServer = this.CasConfigurationSession.ReadLocalServer();
			}
			if (this.casToTest == null)
			{
				this.DetermineCasServer();
			}
		}

		protected void DetermineCasServer()
		{
			if (this.clientAccessServer != null && !string.IsNullOrEmpty(this.clientAccessServer.RawIdentity))
			{
				this.TraceInfo("The CAS server to test is determined by -ClientAccessServer argument.");
				Server server = this.FindServerByHostName(this.clientAccessServer.ToString());
				if (server == null)
				{
					this.TraceInfo("The specified server was not found.");
					this.CasConnectivityWriteError(new LocalizedException(Strings.CasHealthCasServerNotFound), ErrorCategory.ObjectNotFound, null);
					return;
				}
				if (!server.IsClientAccessServer)
				{
					this.TraceInfo("The specified server is not a CAS server.");
					this.CasConnectivityWriteError(new LocalizedException(Strings.CasHealthSpecifiedServerIsNotCas), ErrorCategory.ObjectNotFound, null);
					return;
				}
				this.TraceInfo("The specified server is a CAS server.");
				if (server.MajorVersion != Server.CurrentExchangeMajorVersion)
				{
					this.TraceInfo("The specified CAS server is not on Exchange {0}", new object[]
					{
						Server.CurrentExchangeMajorVersion
					});
					this.CasConnectivityWriteError(new LocalizedException(Strings.CasHealthSpecifiedCASServerVersionNotMatched(base.CommandRuntime.ToString(), server.MajorVersion)), ErrorCategory.InvalidArgument, null);
					return;
				}
				this.casToTest = server;
				return;
			}
			else
			{
				if (this.localServer != null && this.localServer.IsClientAccessServer)
				{
					this.TraceInfo("The CAS server to test is the local server.");
					this.casToTest = this.localServer;
					return;
				}
				this.TraceInfo("Local computer is not a CAS server. No CAS server has been found.");
				return;
			}
		}

		protected Server FindServerByHostName(string hostName)
		{
			if (hostName.IndexOf('.') > 0)
			{
				return this.configurationSession.FindServerByFqdn(hostName);
			}
			return this.configurationSession.FindServerByName(hostName);
		}

		protected TimeSpan ComputeLatency(ExDateTime startTime)
		{
			ExDateTime now = ExDateTime.Now;
			while (now == startTime)
			{
				now = ExDateTime.Now;
			}
			return now - startTime;
		}

		internal static void TraceInfo(int hashCode, string info)
		{
			Microsoft.Exchange.Diagnostics.Components.Tasks.ExTraceGlobals.TraceTracer.Information((long)hashCode, info);
		}

		protected void TraceInfo(string info)
		{
			TestCasConnectivity.TraceInfo(this.GetHashCode(), info);
		}

		protected void TraceInfo(string fmt, params object[] p)
		{
			this.TraceInfo(string.Format(fmt, p));
		}

		protected void WriteErrorAndMonitoringEvent(Exception exception, ErrorCategory errorCategory, object target, int eventId, string eventSource, bool terminateTask)
		{
			this.WriteMonitoringEvent(eventId, eventSource, EventTypeEnumeration.Error, this.ShortErrorMsgFromException(exception));
			this.CasConnectivityWriteError(exception, errorCategory, target, terminateTask);
		}

		internal void WriteMonitoringEvent(int eventId, string eventSource, EventTypeEnumeration eventType, string eventMessage)
		{
			this.monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, eventType, eventMessage));
		}

		private bool WaitAll(IAsyncResult[] asyncResultArray, ExDateTime deadline, Queue outcomeQueue)
		{
			bool flag = false;
			for (int i = asyncResultArray.Length - 1; i >= 0; i--)
			{
				IAsyncResult asyncResult = asyncResultArray[i];
				if (!asyncResult.IsCompleted)
				{
					TimeSpan t = deadline - ExDateTime.Now;
					if (t <= TimeSpan.Zero)
					{
						flag = true;
						break;
					}
					if (!this.MonitoringContext)
					{
						while (!asyncResult.AsyncWaitHandle.WaitOne(100, false))
						{
							for (int j = outcomeQueue.Count; j > 0; j--)
							{
								object output = outcomeQueue.Dequeue();
								this.WriteToConsole(output);
							}
							t = deadline - ExDateTime.Now;
							if (t <= TimeSpan.Zero)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
					t = deadline - ExDateTime.Now;
					if (t <= TimeSpan.Zero)
					{
						flag = true;
						break;
					}
					if (!asyncResult.AsyncWaitHandle.WaitOne(t, false))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				foreach (IAsyncResult asyncResult2 in asyncResultArray)
				{
					if (!asyncResult2.IsCompleted)
					{
						((AsyncResult<CasTransactionOutcome>)asyncResult2).SetTimeout();
					}
				}
				return false;
			}
			return true;
		}

		private void WriteToConsole(object output)
		{
			if (output != null)
			{
				if (output is CasTransactionOutcome)
				{
					base.WriteObject(output);
					return;
				}
				if (output is Warning)
				{
					base.WriteWarning(((Warning)output).Message);
					return;
				}
				base.WriteVerbose((LocalizedString)output);
			}
		}

		protected static Uri GetUrlWithTrailingSlash(Uri originalUri)
		{
			if (originalUri.AbsoluteUri.EndsWith("/"))
			{
				return originalUri;
			}
			UriBuilder uriBuilder = new UriBuilder(originalUri.AbsoluteUri + "/");
			return uriBuilder.Uri;
		}

		protected void SetTestOutcome(CasTransactionOutcome outcome, EventTypeEnumeration eventType, string message, Queue instanceOutcomes)
		{
			if (eventType == EventTypeEnumeration.Warning)
			{
				outcome.Update(CasTransactionResultEnum.Skipped, message, eventType);
			}
			else
			{
				outcome.Update(CasTransactionResultEnum.Failure, message, eventType);
			}
			if (instanceOutcomes != null)
			{
				instanceOutcomes.Enqueue(new Warning(message));
				return;
			}
			base.WriteWarning(message);
		}

		protected const int ADSessionTimeout = 30000;

		internal const string VerboseFormat = "[{1}] : {0}";

		internal const string VerboseTimeFormat = "HH:mm:ss.fff";

		protected const double LatencyPerformanceInCaseOfError = -1000.0;

		private ITopologyConfigurationSession configurationSession;

		private IRecipientSession globalCatalogSession;

		protected string thisFqdn;

		protected Server casToTest;

		protected Server localServer;

		protected PSCredential mailboxCredential;

		protected bool allowUnsecureAccess;

		protected bool trustAllCertificates;

		protected bool autodiscoverCas;

		protected string monitoringInstance;

		protected ServerIdParameter clientAccessServer;

		protected ServerIdParameter mailboxServer;

		protected SecureString password;

		protected MonitoringData monitoringData = new MonitoringData();

		private uint timeout;

		internal static class EventId
		{
			internal const int AllTransactionsSucceeded = 1000;

			internal const int SomeTransactionsFailed = 1001;

			internal const int AutomatedTaskMustBeRunOnExchangeServer = 1002;

			internal const int AutodiscoveryError = 1003;

			internal const int AutodiscoverySuccess = 1004;

			internal const int AccessStoreError = 1005;

			internal const int AccessStoreSuccess = 1006;

			internal const int LoadCredentialsSuccess = 1007;

			internal const int InstructResetCredentials = 1008;

			internal const int DataValidationError = 1009;

			internal const int CasConfigurationError = 1010;

			internal const int CasConfigurationSuccess = 1011;

			internal const int SomeTransactionsHadWarnings = 1012;
		}

		public class TestCasConnectivityRunInstance
		{
			internal LiveIdAuthenticationConfiguration LiveIdAuthenticationConfiguration
			{
				get
				{
					if (this.liveIdAuthenticationConfiguration == null)
					{
						LiveIdAuthenticationConfiguration liveIdAuthenticationConfiguration = new LiveIdAuthenticationConfiguration();
						FederationTrust federationTrust = FederationTrustCache.GetFederationTrust("MicrosoftOnline");
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 343, "LiveIdAuthenticationConfiguration", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Cas\\TestCasConnectivity.cs");
						ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
						liveIdAuthenticationConfiguration.MsoTokenIssuerUri = federationTrust.TokenIssuerUri.ToString();
						liveIdAuthenticationConfiguration.LiveServiceLogin1Uri = endpointContainer.GetEndpoint(ServiceEndpointId.LiveServiceLogin1).Uri;
						liveIdAuthenticationConfiguration.LiveServiceLogin2Uri = endpointContainer.GetEndpoint(ServiceEndpointId.LiveServiceLogin2).Uri;
						liveIdAuthenticationConfiguration.MsoServiceLogin2Uri = endpointContainer.GetEndpoint(ServiceEndpointId.MsoServiceLogin2).Uri;
						liveIdAuthenticationConfiguration.MsoGetUserRealmUri = endpointContainer.GetEndpoint(ServiceEndpointId.MsoGetUserRealm).Uri;
						this.liveIdAuthenticationConfiguration = liveIdAuthenticationConfiguration;
					}
					return this.liveIdAuthenticationConfiguration;
				}
			}

			public bool LightMode
			{
				get
				{
					return this.lightMode;
				}
				set
				{
					this.lightMode = value;
				}
			}

			public bool MonitoringContext
			{
				get
				{
					return this.monitoringContext;
				}
				set
				{
					this.monitoringContext = value;
				}
			}

			public string CasFqdn
			{
				get
				{
					return this.casFqdn;
				}
				set
				{
					this.casFqdn = value;
				}
			}

			public string VirtualDirectoryName { get; private set; }

			public VirtualDirectoryUriScope UrlType
			{
				get
				{
					return this.urlType;
				}
				set
				{
					this.urlType = value;
				}
			}

			public ExchangeVirtualDirectory VirtualDirectory
			{
				get
				{
					return this.virtualDirectory;
				}
				set
				{
					this.virtualDirectory = value;
					if (value != null)
					{
						this.VirtualDirectoryName = value.Name;
						if (this.UrlType == VirtualDirectoryUriScope.External)
						{
							this.baseUri = TestCasConnectivity.GetUrlWithTrailingSlash(value.ExternalUrl);
						}
						else
						{
							this.baseUri = TestCasConnectivity.GetUrlWithTrailingSlash(value.InternalUrl);
						}
						if (value is ExchangeWebAppVirtualDirectory)
						{
							WebAppVirtualDirectoryHelper.UpdateFromMetabase((ExchangeWebAppVirtualDirectory)value);
						}
					}
				}
			}

			public TestCasConnectivityRunInstance()
			{
			}

			public TestCasConnectivityRunInstance(TestCasConnectivity.TestCasConnectivityRunInstance other)
			{
				this.allowUnsecureAccess = other.allowUnsecureAccess;
				this.autodiscoveredCas = other.autodiscoveredCas;
				this.liveRSTEndpointUri = other.liveRSTEndpointUri;
				this.baseUri = other.baseUri;
				this.credentials = other.credentials;
				this.deviceId = other.deviceId;
				this.exchangePrincipal = other.exchangePrincipal;
				this.safeToBuildUpnString = other.safeToBuildUpnString;
				this.trustAllCertificates = other.trustAllCertificates;
				this.urlType = other.urlType;
				this.casFqdn = other.casFqdn;
				this.ConnectionType = other.ConnectionType;
				this.Port = other.Port;
			}

			internal ExchangePrincipal exchangePrincipal;

			internal AsyncResult<CasTransactionOutcome> Result;

			public Queue Outcomes;

			internal CreateTestItemContext createTestItemContext;

			public NetworkCredential credentials;

			public string deviceId;

			public Uri baseUri;

			public Uri liveRSTEndpointUri;

			private LiveIdAuthenticationConfiguration liveIdAuthenticationConfiguration;

			public bool allowUnsecureAccess;

			public bool trustAllCertificates;

			public bool autodiscoveredCas;

			public bool safeToBuildUpnString;

			private bool lightMode;

			private bool monitoringContext;

			private string casFqdn;

			public ProtocolConnectionType ConnectionType;

			public int Port;

			private VirtualDirectoryUriScope urlType;

			private ExchangeVirtualDirectory virtualDirectory;
		}
	}
}
