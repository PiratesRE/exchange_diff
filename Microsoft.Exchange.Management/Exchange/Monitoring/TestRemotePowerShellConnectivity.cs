using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Security;
using System.ServiceProcess;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.PowerShell.HostingTools;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "PowerShellConnectivity", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestRemotePowerShellConnectivity : TestVirtualDirectoryConnectivity
	{
		[Parameter]
		public AuthenticationMechanism Authentication
		{
			get
			{
				if (!base.Fields.Contains("Authentication"))
				{
					return AuthenticationMechanism.Default;
				}
				return (AuthenticationMechanism)base.Fields["Authentication"];
			}
			set
			{
				base.Fields["Authentication"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "URL")]
		[ValidateNotNullOrEmpty]
		public Uri ConnectionUri
		{
			get
			{
				return (Uri)base.Fields["ConnectionUri"];
			}
			set
			{
				base.Fields["ConnectionUri"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "URL")]
		public PSCredential TestCredential
		{
			get
			{
				return this.mailboxCredential;
			}
			set
			{
				this.mailboxCredential = value;
			}
		}

		private new SwitchParameter LightMode
		{
			get
			{
				return base.LightMode;
			}
			set
			{
				base.LightMode = value;
			}
		}

		private new uint Timeout
		{
			get
			{
				return base.Timeout;
			}
			set
			{
				base.Timeout = value;
			}
		}

		public TestRemotePowerShellConnectivity() : base(Strings.CasHealthPowerShellLongName, Strings.CasHealthPowerShellShortName, TransientErrorCache.PowerShellTransientErrorCache, "MSExchange Monitoring PowerShellConnectivity Internal", "MSExchange Monitoring PowerShellConnectivity External")
		{
		}

		protected override IEnumerable<ExchangeVirtualDirectory> GetVirtualDirectories(ADObjectId serverId, QueryFilter filter)
		{
			List<ExchangeVirtualDirectory> list = new List<ExchangeVirtualDirectory>();
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "PowerShell (Default Web Site)"),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "PowerShell-LiveID (Default Web Site)")
			});
			filter = new AndFilter(new QueryFilter[]
			{
				filter,
				queryFilter
			});
			foreach (ExchangeVirtualDirectory exchangeVirtualDirectory in base.GetVirtualDirectories<ADPowerShellVirtualDirectory>(serverId, filter))
			{
				ADPowerShellVirtualDirectory adpowerShellVirtualDirectory = (ADPowerShellVirtualDirectory)exchangeVirtualDirectory;
				this.ProcessMetabaseProperties(adpowerShellVirtualDirectory);
				list.Add(adpowerShellVirtualDirectory);
				base.TraceInfo("Virtual Directory " + adpowerShellVirtualDirectory.DistinguishedName.ToString() + " found in server " + serverId.DistinguishedName.ToString());
			}
			return list;
		}

		internal override void UpdateTransientErrorCache(TestCasConnectivity.TestCasConnectivityRunInstance instance, int cFailed, int cWarning, ref int cFailedTransactions, ref int cWarningTransactions, StringBuilder failedStr, StringBuilder warningStr, ref StringBuilder failedTransactionsStr, ref StringBuilder warningTransactionsStr)
		{
			TransientErrorCache transientErrorCache = this.GetTransientErrorCache();
			string text = (instance.exchangePrincipal != null) ? instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn : null;
			if (cFailed > 0 || cWarning > 0)
			{
				if (transientErrorCache != null && !string.IsNullOrEmpty(text) && !transientErrorCache.ContainsError(text, instance.VirtualDirectoryName))
				{
					transientErrorCache.Add(new CASServiceError(text, instance.VirtualDirectoryName));
					return;
				}
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
			else if (transientErrorCache != null && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(instance.VirtualDirectoryName))
			{
				transientErrorCache.Remove(text, instance.VirtualDirectoryName);
			}
		}

		protected override void ValidateTestWebApplicationRequirements()
		{
			if (this.casToTest == null)
			{
				if (this.localServer != null && (this.localServer.IsMailboxServer || this.localServer.IsUnifiedMessagingServer || this.localServer.IsHubTransportServer))
				{
					base.TraceInfo("The MBX,UM or HT server to test is the local server.");
					this.isCasServer = false;
					this.casToTest = this.localServer;
				}
				else
				{
					base.TraceInfo("Local computer is not a CAS,MBX, UM or HT server. No CAS, MBX, UM or HT server has been found.");
				}
				if (this.casToTest == null)
				{
					base.CasConnectivityWriteError(new ApplicationException(this.NoCasMbxUmHtArgumentError), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		internal LocalizedString NoCasMbxUmHtArgumentError
		{
			get
			{
				return Strings.CasHealthWebAppNoCasMbxUmHtArgumentError;
			}
		}

		protected override List<TestCasConnectivity.TestCasConnectivityRunInstance> PopulateInfoPerCas(TestCasConnectivity.TestCasConnectivityRunInstance instance, List<CasTransactionOutcome> outcomeList)
		{
			TaskLogger.LogEnter();
			List<TestCasConnectivity.TestCasConnectivityRunInstance> result;
			try
			{
				if (base.Fields.IsModified("ConnectionUri"))
				{
					base.WriteVerbose(Strings.CasHealthOwaTestUrlSpecified(this.ConnectionUri.AbsoluteUri));
					TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance = new TestCasConnectivity.TestCasConnectivityRunInstance(instance);
					testCasConnectivityRunInstance.baseUri = TestCasConnectivity.GetUrlWithTrailingSlash(this.ConnectionUri);
					testCasConnectivityRunInstance.UrlType = VirtualDirectoryUriScope.Unknown;
					testCasConnectivityRunInstance.CasFqdn = null;
					result = new List<TestCasConnectivity.TestCasConnectivityRunInstance>
					{
						testCasConnectivityRunInstance
					};
				}
				else
				{
					result = base.PopulateInfoPerCas(instance, outcomeList);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		protected override List<CasTransactionOutcome> ExecuteTests(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			ExDateTime now = ExDateTime.Now;
			TaskLogger.LogEnter();
			base.WriteVerbose(Strings.CasHealthScenarioLogon);
			if (base.TestType == OwaConnectivityTestType.External && !this.ValidateExternalTest(instance))
			{
				instance.Result.Complete();
				return null;
			}
			LocalizedString localizedString = LocalizedString.Empty;
			Uri uri;
			if (base.Fields.IsModified("ConnectionUri"))
			{
				uri = this.ConnectionUri;
				localizedString = Strings.CasHealthPowerShellConnectionUri(uri.ToString(), "user supplied Uri");
			}
			else if (base.TestType == OwaConnectivityTestType.External)
			{
				uri = instance.VirtualDirectory.ExternalUrl;
				localizedString = Strings.CasHealthPowerShellConnectionUri(uri.ToString(), "Virtual Directory External Uri");
			}
			else
			{
				uri = instance.VirtualDirectory.InternalUrl;
				localizedString = Strings.CasHealthPowerShellConnectionUri(uri.ToString(), "Virtual Directory Internal Uri");
			}
			base.TraceInfo(localizedString);
			base.WriteVerbose(localizedString);
			ADPowerShellVirtualDirectory adpowerShellVirtualDirectory = instance.VirtualDirectory as ADPowerShellVirtualDirectory;
			if (adpowerShellVirtualDirectory != null)
			{
				base.TraceInfo(Strings.CasHealthPowerShellConnectionVirtualDirectory(adpowerShellVirtualDirectory.Name));
				base.WriteVerbose(Strings.CasHealthPowerShellConnectionVirtualDirectory(adpowerShellVirtualDirectory.Name));
			}
			if (uri == null)
			{
				CasTransactionOutcome casTransactionOutcome = this.BuildOutcome(Strings.CasHealthOwaLogonScenarioName, null, instance);
				base.TraceInfo("No External or Internal Url found for testing");
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, (base.TestType == OwaConnectivityTestType.External) ? Strings.CasHealthOwaNoExternalUrl : Strings.CasHealthOwaNoInternalUrl);
				instance.Outcomes.Enqueue(casTransactionOutcome);
				instance.Result.Outcomes.Add(casTransactionOutcome);
				base.WriteErrorAndMonitoringEvent(new LocalizedException((base.TestType == OwaConnectivityTestType.External) ? Strings.CasHealthOwaNoExternalUrl : Strings.CasHealthOwaNoInternalUrl), (ErrorCategory)1001, null, 1010, (base.TestType == OwaConnectivityTestType.External) ? "MSExchange Monitoring PowerShellConnectivity External" : "MSExchange Monitoring PowerShellConnectivity Internal", false);
				base.WriteObject(casTransactionOutcome);
			}
			else
			{
				CasTransactionOutcome casTransactionOutcome2 = this.BuildOutcome(Strings.CasHealthScenarioLogon, Strings.CasHealthPowerShellRemoteConnectionScenario(uri.ToString(), this.Authentication.ToString()), instance);
				PSCredential credential;
				if (base.ParameterSetName == "URL")
				{
					credential = this.TestCredential;
					localizedString = Strings.CasHealthPowerShellConnectionUserCredential(this.TestCredential.UserName, "user supplied");
				}
				else
				{
					credential = new PSCredential(instance.credentials.UserName, instance.credentials.Password.ConvertToSecureString());
					localizedString = Strings.CasHealthPowerShellConnectionUserCredential(instance.credentials.UserName, "default for test user");
				}
				base.TraceInfo(localizedString);
				base.WriteVerbose(localizedString);
				WSManConnectionInfo wsmanConnectionInfo;
				if (base.TestType == OwaConnectivityTestType.External)
				{
					wsmanConnectionInfo = new WSManConnectionInfo(uri, "http://schemas.microsoft.com/powershell/Microsoft.Exchange", this.certThumbprint);
				}
				else
				{
					wsmanConnectionInfo = new WSManConnectionInfo(uri, "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credential);
				}
				if (base.Fields.IsModified("Authentication"))
				{
					wsmanConnectionInfo.AuthenticationMechanism = this.Authentication;
				}
				else if (adpowerShellVirtualDirectory != null && adpowerShellVirtualDirectory.LiveIdBasicAuthentication != null && adpowerShellVirtualDirectory.LiveIdBasicAuthentication.Value)
				{
					wsmanConnectionInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;
				}
				base.TraceInfo(Strings.CasHealthPowerShellConnectionAuthenticationType(wsmanConnectionInfo.AuthenticationMechanism.ToString()));
				base.WriteVerbose(Strings.CasHealthPowerShellConnectionAuthenticationType(wsmanConnectionInfo.AuthenticationMechanism.ToString()));
				if (base.TrustAnySSLCertificate)
				{
					wsmanConnectionInfo.SkipCACheck = true;
					wsmanConnectionInfo.SkipCNCheck = true;
					wsmanConnectionInfo.SkipRevocationCheck = true;
				}
				using (Runspace runspace = RunspaceFactory.CreateRunspace(TestRemotePowerShellConnectivity.psHost, wsmanConnectionInfo))
				{
					try
					{
						runspace.Open();
						base.WriteVerbose(Strings.CasHealtRemotePowerShellOpenRunspaceSucceeded);
						base.TraceInfo(Strings.CasHealtRemotePowerShellOpenRunspaceSucceeded);
						runspace.Close();
						base.TraceInfo(Strings.CasHealtRemotePowerShellCloseRunspaceSucceeded);
						base.WriteVerbose(Strings.CasHealtRemotePowerShellCloseRunspaceSucceeded);
						casTransactionOutcome2.Update(CasTransactionResultEnum.Success, base.ComputeLatency(now), null);
					}
					catch (Exception ex)
					{
						casTransactionOutcome2.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(now), ex.Message);
						instance.Outcomes.Enqueue(casTransactionOutcome2);
						instance.Result.Outcomes.Add(casTransactionOutcome2);
						try
						{
							string hostName = Dns.GetHostName();
							if (adpowerShellVirtualDirectory != null && string.Compare(this.casToTest.Fqdn, Dns.GetHostEntry(hostName).HostName, true) == 0)
							{
								this.CheckRequiredServicesAndAppPool(adpowerShellVirtualDirectory);
							}
						}
						catch (Exception)
						{
						}
						base.WriteErrorAndMonitoringEvent(new LocalizedException(Strings.CasHealthPowerShellLogonFailed((adpowerShellVirtualDirectory != null) ? adpowerShellVirtualDirectory.Name : this.ConnectionUri.ToString(), ex.Message)), (ErrorCategory)1001, null, 1001, (base.TestType == OwaConnectivityTestType.External) ? "MSExchange Monitoring PowerShellConnectivity External" : "MSExchange Monitoring PowerShellConnectivity Internal", false);
					}
					finally
					{
						base.WriteObject(casTransactionOutcome2);
					}
				}
				instance.Result.Complete();
			}
			return null;
		}

		private void CheckRequiredServicesAndAppPool(ADPowerShellVirtualDirectory psVdir)
		{
			this.CheckServices(this.requiredCommonServices, psVdir);
			if (string.Compare(psVdir.Name, "PowerShell (Default Web Site)", true) == 0)
			{
				this.CheckAppPool("MSExchangePowerShellAppPool");
				return;
			}
			if (string.Compare(psVdir.Name, "PowerShell-LiveID (Default Web Site)", true) == 0)
			{
				this.CheckAppPool("MSExchangePowerShellLiveIDAppPool");
				this.CheckServices(this.requiredPowershellLiveIdServices, psVdir);
			}
		}

		private void CheckServices(string[] requiredServices, ADPowerShellVirtualDirectory psVdir)
		{
			foreach (string text in requiredServices)
			{
				if (ManageServiceBase.GetServiceStatus(text) != ServiceControllerStatus.Running)
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_RequiredServiceNotRunning, new string[]
					{
						psVdir.Name,
						text
					});
					base.WriteVerbose(Strings.CasHealthPowerShellServiceNotRunning(psVdir.Name, text));
					base.TraceInfo(Strings.CasHealthPowerShellServiceNotRunning(psVdir.Name, text));
				}
			}
		}

		private void CheckAppPool(string appPoolName)
		{
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPool applicationPool = serverManager.ApplicationPools[appPoolName];
				if (applicationPool != null && applicationPool.State != 1)
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_AppPoolNotRunning, new string[]
					{
						applicationPool.Name
					});
					base.WriteVerbose(Strings.CasHealthPowerShellAppPoolNotRunning(applicationPool.Name));
					base.TraceInfo(Strings.CasHealthPowerShellAppPoolNotRunning(applicationPool.Name));
				}
			}
		}

		private void ProcessMetabaseProperties(ADPowerShellVirtualDirectory virtualDirectory)
		{
			try
			{
				using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(virtualDirectory.MetabasePath, new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), virtualDirectory.Identity))
				{
					virtualDirectory.BasicAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic));
					virtualDirectory.DigestAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Digest));
					virtualDirectory.WindowsAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Ntlm));
					virtualDirectory.CertificateAuthentication = new bool?(IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Certificate));
					virtualDirectory.LiveIdBasicAuthentication = new bool?(virtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.LiveIdBasic));
					virtualDirectory.WSSecurityAuthentication = new bool?(virtualDirectory.InternalAuthenticationMethods.Contains(AuthenticationMethod.WSSecurity) && IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.WSSecurity));
					virtualDirectory.ResetChangeTracking();
				}
			}
			catch (Exception ex)
			{
				base.WriteErrorAndMonitoringEvent(new CannotPopulateMetabaseInformationException(virtualDirectory.Name, ex.Message, ex), (ErrorCategory)1001, null, 1001, "MSExchange Monitoring PowerShellConnectivity Internal", true);
			}
		}

		private void GetRpsCertificateThumbprint()
		{
			if (this.isCasServer)
			{
				Exception ex = null;
				try
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 614, "GetRpsCertificateThumbprint", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Tasks\\TestRemotePowerShellConnectivity.cs");
					ServiceEndpoint endpoint = topologyConfigurationSession.GetEndpointContainer().GetEndpoint("ForwardSyncRpsEndPoint");
					this.certThumbprint = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(endpoint.CertificateSubject).Thumbprint;
				}
				catch (ServiceEndpointNotFoundException ex2)
				{
					ex = ex2;
				}
				catch (ArgumentException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					this.monitoringData.Events.Add(new MonitoringEvent("MSExchange Monitoring PowerShellConnectivity External", 1012, EventTypeEnumeration.Warning, "PS ExternalUrl test for certificate connection skipped:" + ex.ToString()));
				}
			}
		}

		private bool ValidateExternalTest(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			bool result = true;
			if (string.Compare(instance.VirtualDirectory.Name, "PowerShell-LiveID (Default Web Site)", true) == 0)
			{
				base.WriteVerbose(Strings.CasHealthPowerShellSkipCertVDir("PS liveID ExternalUrl"));
				result = false;
			}
			else
			{
				this.GetRpsCertificateThumbprint();
				if (this.certThumbprint == null)
				{
					this.WriteWarning(Strings.CasHealthPowerShellSkipCertVDir("PS ExternalUrl without ForwardSync Certificate"));
					result = false;
				}
			}
			return result;
		}

		private const string monitoringEventSourceInternal = "MSExchange Monitoring PowerShellConnectivity Internal";

		private const string monitoringEventSourceExternal = "MSExchange Monitoring PowerShellConnectivity External";

		private const string ExchangeShellSchema = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

		private const string strPSVdirAppPool = "MSExchangePowerShellAppPool";

		private const string strPSLiveIdVdirAppPool = "MSExchangePowerShellLiveIDAppPool";

		private const string strPSVdirName = "PowerShell (Default Web Site)";

		private const string strPSLiveIdVdirName = "PowerShell-LiveID (Default Web Site)";

		private string[] requiredCommonServices = new string[]
		{
			"W3SVC",
			"MSExchangeIS"
		};

		private string[] requiredPowershellLiveIdServices = new string[]
		{
			"MSExchangeProtectedServiceHost"
		};

		private static TestRemotePowerShellConnectivity.TPSHost psHost = new TestRemotePowerShellConnectivity.TPSHost();

		private bool isCasServer = true;

		private string certThumbprint;

		private class TPSHost : RunspaceHost
		{
			public TPSHost()
			{
				this.hostUI = new TestRemotePowerShellConnectivity.TPSHostUI(this);
				this.hostRawUI = new TestRemotePowerShellConnectivity.TPSHostRawUI(this, (TestRemotePowerShellConnectivity.TPSHostUI)this.UI);
			}

			public sealed override PSHostUserInterface UI
			{
				get
				{
					return this.hostUI;
				}
			}

			public TestRemotePowerShellConnectivity.TPSHostRawUI RawUI
			{
				get
				{
					return this.hostRawUI;
				}
			}

			private TestRemotePowerShellConnectivity.TPSHostUI hostUI;

			private TestRemotePowerShellConnectivity.TPSHostRawUI hostRawUI;
		}

		private class TPSHostUI : RunspaceHostUI
		{
			public TPSHostUI(TestRemotePowerShellConnectivity.TPSHost owner) : base(owner)
			{
				this.owner = owner;
			}

			public override PSHostRawUserInterface RawUI
			{
				get
				{
					return this.owner.RawUI;
				}
			}

			public override string ReadLine()
			{
				return null;
			}

			public override SecureString ReadLineAsSecureString()
			{
				return null;
			}

			public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
			{
			}

			public override void Write(string value)
			{
			}

			public override void WriteDebugLine(string message)
			{
			}

			public override void WriteLine(string value)
			{
			}

			public override void WriteVerboseLine(string message)
			{
			}

			public override void WriteProgress(long value, ProgressRecord record)
			{
			}

			public override void WriteErrorLine(string message)
			{
			}

			public override void WriteWarningLine(string message)
			{
			}

			private TestRemotePowerShellConnectivity.TPSHost owner;
		}

		private class TPSHostRawUI : PSHostRawUserInterface
		{
			public TPSHostRawUI(TestRemotePowerShellConnectivity.TPSHost owner, TestRemotePowerShellConnectivity.TPSHostUI ownerUI)
			{
			}

			public override ConsoleColor ForegroundColor
			{
				get
				{
					return ConsoleColor.White;
				}
				set
				{
				}
			}

			public override ConsoleColor BackgroundColor
			{
				get
				{
					return ConsoleColor.Black;
				}
				set
				{
				}
			}

			public override Size BufferSize
			{
				get
				{
					return new Size(0, 0);
				}
				set
				{
				}
			}

			public override Coordinates CursorPosition
			{
				get
				{
					return new Coordinates(0, 0);
				}
				set
				{
				}
			}

			public override Coordinates WindowPosition
			{
				get
				{
					return new Coordinates(0, 0);
				}
				set
				{
				}
			}

			public override Size WindowSize
			{
				get
				{
					return new Size(0, 0);
				}
				set
				{
				}
			}

			public override Size MaxWindowSize
			{
				get
				{
					return new Size(0, 0);
				}
			}

			public override Size MaxPhysicalWindowSize
			{
				get
				{
					return new Size(0, 0);
				}
			}

			public override KeyInfo ReadKey(ReadKeyOptions options)
			{
				return default(KeyInfo);
			}

			public override bool KeyAvailable
			{
				get
				{
					return false;
				}
			}

			public override string WindowTitle
			{
				get
				{
					return string.Empty;
				}
				set
				{
				}
			}

			public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
			{
			}

			public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
			{
			}

			public override BufferCell[,] GetBufferContents(Rectangle rectangle)
			{
				return null;
			}

			public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
			{
			}

			public override int CursorSize
			{
				get
				{
					return 0;
				}
				set
				{
				}
			}

			public override void FlushInputBuffer()
			{
			}
		}
	}
}
