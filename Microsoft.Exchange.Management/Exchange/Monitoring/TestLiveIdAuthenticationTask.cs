using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "LiveIdAuthentication", SupportsShouldProcess = true)]
	public sealed class TestLiveIdAuthenticationTask : Task
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (SwitchParameter)(base.Fields["MonitoringContext"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public ServerIdParameter Server
		{
			get
			{
				return ((ServerIdParameter)base.Fields["Server"]) ?? new ServerIdParameter();
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LiveIdAuthenticationUserTypeEnum UserType
		{
			get
			{
				return (LiveIdAuthenticationUserTypeEnum)(base.Fields["UserType"] ?? LiveIdAuthenticationUserTypeEnum.ManagedConsumer);
			}
			set
			{
				base.Fields["UserType"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public PSCredential MailboxCredential
		{
			get
			{
				return (PSCredential)base.Fields["MailboxCredential"];
			}
			set
			{
				base.Fields["MailboxCredential"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public SwitchParameter TestLegacyAPI { get; set; }

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public SwitchParameter SyncADBackendOnly { get; set; }

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public SwitchParameter PreferOfflineAuth
		{
			get
			{
				return (SwitchParameter)(base.Fields["PreferOfflineAuth"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["PreferOfflineAuth"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public FailoverFlags TestFailOver
		{
			get
			{
				return (FailoverFlags)(base.Fields["TestFailOver"] ?? FailoverFlags.None);
			}
			set
			{
				base.Fields["TestFailOver"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public SwitchParameter IgnoreLowPasswordConfidence
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreLowPasswordConfidence"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreLowPasswordConfidence"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public SwitchParameter LiveIdXmlAuth
		{
			get
			{
				return (SwitchParameter)(base.Fields["LiveIdXmlAuth"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["LiveIdXmlAuth"] = value;
			}
		}

		internal ITopologyConfigurationSession SystemConfigurationSession
		{
			get
			{
				if (this.systemConfigurationSession == null)
				{
					this.systemConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 339, "SystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Authentication\\TestLiveIdAuthenticationTask.cs");
				}
				return this.systemConfigurationSession;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					this.ServerObject = TestLiveIdAuthenticationTask.EnsureSingleObject<Server>(() => this.Server.GetObjects<Server>(null, this.SystemConfigurationSession));
					if (this.ServerObject == null)
					{
						throw new CasHealthMailboxServerNotFoundException(this.Server.ToString());
					}
					if (this.MailboxCredential == null)
					{
						this.GetTestMailbox();
					}
				}
			}
			catch (LocalizedException exception)
			{
				this.WriteError(exception, ErrorCategory.OperationStopped, this, true);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (this.MonitoringContext)
			{
				this.monitoringData = new MonitoringData();
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalBeginProcessing();
			TaskLogger.LogEnter();
			try
			{
				this.VerifyServiceIsRunning();
				LiveIdAuthenticationOutcome sendToPipeline = new LiveIdAuthenticationOutcome(this.Server.ToString(), this.MailboxCredential.UserName);
				this.PerformLiveIdAuthenticationTest(ref sendToPipeline);
				base.WriteObject(sendToPipeline);
			}
			catch (LocalizedException e)
			{
				this.HandleException(e);
			}
			finally
			{
				if (this.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestLiveIdAuthenticationIdentity(this.MailboxCredential.UserName);
			}
		}

		private void VerifyServiceIsRunning()
		{
			string name = "MSExchangeProtectedServiceHost";
			try
			{
				using (ServiceController serviceController = new ServiceController(name, this.Server.Fqdn))
				{
					if (serviceController.Status != ServiceControllerStatus.Running)
					{
						base.WriteVerbose(new LocalizedString("Service is not Running, Trying to start the service"));
						serviceController.Start();
						base.WriteVerbose(new LocalizedString("Service has been Started"));
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				base.WriteVerbose(new LocalizedString("InvalidOperationException while verifying the service status : " + ex.ToString()));
			}
			catch (Win32Exception ex2)
			{
				base.WriteVerbose(new LocalizedString("Win32Exception while verifying the service status : " + ex2.ToString()));
			}
		}

		private void KillAndStartService()
		{
			string text = "Microsoft.Exchange.ProtectedServiceHost";
			int num = 30000;
			int millisecondsTimeout = 500;
			try
			{
				base.WriteVerbose(new LocalizedString("Before killing and restarting making sure the problem exists"));
				bool flag;
				LiveIdAuthenticationError liveIdAuthenticationError;
				string text2;
				TimeSpan timeSpan;
				this.InternalPerformLiveIdAuthentication(out flag, out liveIdAuthenticationError, out text2, out timeSpan);
				if (liveIdAuthenticationError == LiveIdAuthenticationError.None)
				{
					base.WriteVerbose(new LocalizedString("Issue was self fixed"));
				}
				else
				{
					Process[] processesByName = Process.GetProcessesByName(text);
					if (processesByName.Length != 1)
					{
						throw new InvalidOperationException(string.Format("{0} process not found", text));
					}
					processesByName[0].Kill();
					Stopwatch stopwatch = Stopwatch.StartNew();
					for (long num2 = 0L; num2 < (long)num; num2 = stopwatch.ElapsedMilliseconds)
					{
						stopwatch.Start();
						processesByName = Process.GetProcessesByName(text);
						if (processesByName.Length == 0)
						{
							break;
						}
						Thread.Sleep(millisecondsTimeout);
						stopwatch.Stop();
					}
					this.VerifyServiceIsRunning();
				}
			}
			catch (Exception ex)
			{
				base.WriteVerbose(new LocalizedString("Exception while killing and starting the service : " + ex.ToString()));
			}
		}

		private void InternalPerformLiveIdAuthentication(out bool success, out LiveIdAuthenticationError error, out string iisLogs, out TimeSpan latency)
		{
			int num = 1009;
			string uri = string.Format("net.tcp://{0}:{1}/Microsoft.Exchange.Security.Authentication.FederatedAuthService", this.Server.ToString(), num);
			NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
			EndpointAddress remoteAddress = new EndpointAddress(uri);
			using (AuthServiceClient authServiceClient = new AuthServiceClient(binding, remoteAddress))
			{
				error = LiveIdAuthenticationError.None;
				byte[] bytes = Encoding.Default.GetBytes(this.MailboxCredential.UserName);
				byte[] bytes2 = Encoding.Default.GetBytes(this.ConvertToUnsecureString(this.MailboxCredential.Password));
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					TestFailoverFlags testFailoverFlags;
					if (this.TestFailOver == FailoverFlags.Random)
					{
						if (!this.PreferOfflineAuth)
						{
							TestFailoverFlags[] array;
							if (this.UserType == LiveIdAuthenticationUserTypeEnum.ManagedConsumer)
							{
								array = new TestFailoverFlags[]
								{
									TestFailoverFlags.HRDRequest,
									TestFailoverFlags.HRDResponse,
									TestFailoverFlags.LiveIdRequest,
									TestFailoverFlags.LiveIdResponse,
									TestFailoverFlags.OrgIdRequest,
									TestFailoverFlags.OrgIdResponse,
									TestFailoverFlags.HRDRequestTimeout,
									TestFailoverFlags.LiveIdRequestTimeout,
									TestFailoverFlags.OrgIdRequestTimeout
								};
							}
							else
							{
								array = new TestFailoverFlags[]
								{
									TestFailoverFlags.HRDRequest,
									TestFailoverFlags.HRDResponse,
									TestFailoverFlags.OrgIdRequest,
									TestFailoverFlags.OrgIdResponse,
									TestFailoverFlags.HRDRequestTimeout,
									TestFailoverFlags.OrgIdRequestTimeout
								};
							}
							testFailoverFlags = array[new Random().Next(0, array.Length)];
						}
						else
						{
							TestFailoverFlags[] array2;
							if (this.UserType == LiveIdAuthenticationUserTypeEnum.ManagedConsumer)
							{
								array2 = new TestFailoverFlags[]
								{
									TestFailoverFlags.OfflineHRD,
									TestFailoverFlags.OfflineAuthentication,
									TestFailoverFlags.LowPasswordConfidence,
									TestFailoverFlags.LiveIdRequest,
									TestFailoverFlags.LiveIdResponse
								};
							}
							else
							{
								array2 = new TestFailoverFlags[]
								{
									TestFailoverFlags.OfflineHRD,
									TestFailoverFlags.OfflineAuthentication,
									TestFailoverFlags.LowPasswordConfidence
								};
							}
							testFailoverFlags = array2[new Random().Next(0, array2.Length)];
						}
					}
					else
					{
						testFailoverFlags = (TestFailoverFlags)this.TestFailOver;
					}
					AuthOptions authOptions = AuthOptions.SyncAD;
					if (this.TestLegacyAPI)
					{
						authOptions |= AuthOptions.ReturnWindowsIdentity;
					}
					if (this.SyncADBackendOnly)
					{
						authOptions |= AuthOptions.SyncADBackEndOnly;
					}
					if (this.LiveIdXmlAuth)
					{
						authOptions |= AuthOptions.LiveIdXmlAuth;
					}
					string text;
					AuthStatus authStatus = authServiceClient.LogonCommonAccessTokenFederationCredsTest(uint.MaxValue, bytes, bytes2, authOptions, null, null, null, null, Guid.NewGuid(), new bool?(this.PreferOfflineAuth), testFailoverFlags, out text, out iisLogs);
					if (this.TestFailOver != FailoverFlags.None)
					{
						iisLogs = testFailoverFlags.ToString() + "." + iisLogs;
					}
					error = ((authStatus == AuthStatus.LogonSuccess) ? LiveIdAuthenticationError.None : LiveIdAuthenticationError.LoginFailure);
					if (this.IgnoreLowPasswordConfidence && iisLogs != null && iisLogs.IndexOf("low confidence") > 0)
					{
						error = LiveIdAuthenticationError.None;
					}
				}
				catch (CommunicationException ex)
				{
					iisLogs = ex.Message;
					error = LiveIdAuthenticationError.CommunicationException;
				}
				catch (InvalidOperationException ex2)
				{
					iisLogs = ex2.Message;
					error = LiveIdAuthenticationError.InvalidOperationException;
				}
				catch (Exception ex3)
				{
					iisLogs = ex3.Message;
					error = LiveIdAuthenticationError.OtherException;
				}
				stopwatch.Stop();
				success = (error == LiveIdAuthenticationError.None);
				latency = stopwatch.Elapsed;
			}
		}

		private void PerformLiveIdAuthenticationTest(ref LiveIdAuthenticationOutcome result)
		{
			bool flag = false;
			string empty = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			LiveIdAuthenticationError liveIdAuthenticationError = LiveIdAuthenticationError.None;
			TimeSpan latency = new TimeSpan(0, 0, 0);
			for (int i = 0; i < 3; i++)
			{
				this.InternalPerformLiveIdAuthentication(out flag, out liveIdAuthenticationError, out empty, out latency);
				if (liveIdAuthenticationError == LiveIdAuthenticationError.LoginFailure || liveIdAuthenticationError == LiveIdAuthenticationError.None)
				{
					break;
				}
				stringBuilder.AppendLine(" Retrying: " + empty);
				base.WriteVerbose(new LocalizedString(string.Format("Failed with Error {0}. Trying to Kill and Restart.", empty)));
				this.KillAndStartService();
			}
			stringBuilder.AppendLine(empty);
			result.Update(flag ? LiveIdAuthenticationResultEnum.Success : LiveIdAuthenticationResultEnum.Failure, latency, stringBuilder.ToString());
			liveIdAuthenticationError = (flag ? LiveIdAuthenticationError.None : LiveIdAuthenticationError.LoginFailure);
			if (this.MonitoringContext)
			{
				this.monitoringData.Events.Add(new MonitoringEvent(TestLiveIdAuthenticationTask.CmdletMonitoringEventSource, (int)((flag ? 1000 : 2000) + this.UserType + (int)liveIdAuthenticationError), flag ? EventTypeEnumeration.Success : EventTypeEnumeration.Error, flag ? Strings.LiveIdAuthenticationSuccess(this.UserType.ToString()) : Strings.LiveIdAuthenticationFailed(this.UserType.ToString(), stringBuilder.ToString())));
			}
		}

		private bool IsExplicitlySet(string param)
		{
			return base.Fields.Contains(param);
		}

		private string ConvertToUnsecureString(SecureString securePassword)
		{
			if (securePassword == null)
			{
				throw new ArgumentNullException("securePassword");
			}
			IntPtr intPtr = IntPtr.Zero;
			string result;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
				result = Marshal.PtrToStringUni(intPtr);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
			}
			return result;
		}

		private void GetTestMailbox()
		{
			ADSite localSite = this.SystemConfigurationSession.GetLocalSite();
			base.WriteVerbose(Strings.RunCmdletOnSite(localSite.ToString()));
			string domain = this.ServerObject.Domain;
			base.WriteVerbose(Strings.RunCmdletOnDomain(domain));
			NetworkCredential defaultTestAccount = TestLiveIdAuthenticationTask.GetDefaultTestAccount(new TestLiveIdAuthenticationTask.ClientAccessContext
			{
				Instance = this,
				MonitoringContext = this.MonitoringContext,
				ConfigurationSession = this.SystemConfigurationSession,
				WindowsDomain = domain,
				Site = localSite
			}, this.UserType);
			base.WriteVerbose(Strings.RunCmdletOnUser(defaultTestAccount.UserName));
			this.MailboxCredential = TestLiveIdAuthenticationTask.MakePSCredential(defaultTestAccount);
		}

		internal static IRecipientSession GetRecipientSession(string domain)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(domain);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantLocal, 806, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\Authentication\\TestLiveIdAuthenticationTask.cs");
		}

		internal static NetworkCredential GetDefaultTestAccount(TestLiveIdAuthenticationTask.ClientAccessContext context, LiveIdAuthenticationUserTypeEnum userType)
		{
			SmtpAddress? defaultTestUser = null;
			if (!TestConnectivityCredentialsManager.IsExchangeMultiTenant())
			{
				throw new InvalidOperationException();
			}
			if (userType <= LiveIdAuthenticationUserTypeEnum.ManagedBusiness)
			{
				if (userType != LiveIdAuthenticationUserTypeEnum.ManagedConsumer)
				{
					if (userType == LiveIdAuthenticationUserTypeEnum.ManagedBusiness)
					{
						defaultTestUser = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(context.Instance, context.ConfigurationSession, context.Site, DatacenterUserType.BPOS);
					}
				}
				else
				{
					defaultTestUser = TestConnectivityCredentialsManager.GetMultiTenantAutomatedTaskUser(context.Instance, context.ConfigurationSession, context.Site, DatacenterUserType.EDU);
				}
			}
			else if (userType == LiveIdAuthenticationUserTypeEnum.FederatedConsumer || userType == LiveIdAuthenticationUserTypeEnum.FederatedBusiness)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(), null);
			}
			if (defaultTestUser == null)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(), null);
			}
			MailboxIdParameter localMailboxId = new MailboxIdParameter(string.Format("{0}", defaultTestUser.Value.Local));
			ADUser aduser = TestLiveIdAuthenticationTask.EnsureSingleObject<ADUser>(() => localMailboxId.GetObjects<ADUser>(null, TestLiveIdAuthenticationTask.GetRecipientSession(defaultTestUser.GetValueOrDefault().Domain)));
			if (aduser == null)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(defaultTestUser.ToString()), null);
			}
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(aduser.OrganizationId.ToADSessionSettings(), aduser);
			if (exchangePrincipal == null)
			{
				throw new MailboxNotFoundException(new MailboxIdParameter(defaultTestUser.ToString()), null);
			}
			NetworkCredential networkCredential = new NetworkCredential(defaultTestUser.Value.ToString(), string.Empty, context.WindowsDomain);
			NetworkCredential networkCredential2 = TestLiveIdAuthenticationTask.MakeCasCredential(networkCredential);
			LocalizedException ex = TestConnectivityCredentialsManager.LoadAutomatedTestCasConnectivityInfo(exchangePrincipal, networkCredential2);
			if (ex != null)
			{
				throw ex;
			}
			networkCredential.Domain = defaultTestUser.Value.Domain;
			networkCredential.Password = networkCredential2.Password;
			return networkCredential;
		}

		internal static NetworkCredential MakeCasCredential(NetworkCredential networkCredential)
		{
			return new NetworkCredential(networkCredential.UserName, networkCredential.Password, networkCredential.Domain);
		}

		private static PSCredential MakePSCredential(NetworkCredential networkCredential)
		{
			string userName = networkCredential.UserName;
			SecureString secureString = new SecureString();
			foreach (char c in networkCredential.Password)
			{
				secureString.AppendChar(c);
			}
			return new PSCredential(userName, secureString);
		}

		private TimeSpan TotalLatency { get; set; }

		private Server ServerObject { get; set; }

		internal static T EnsureSingleObject<T>(Func<IEnumerable<T>> getObjects) where T : class
		{
			T t = default(T);
			foreach (T t2 in getObjects())
			{
				if (t != null)
				{
					throw new DataValidationException(new ObjectValidationError(Strings.MoreThanOneObjects(typeof(T).ToString()), null, null));
				}
				t = t2;
			}
			return t;
		}

		private void HandleException(LocalizedException e)
		{
			if (!this.MonitoringContext)
			{
				this.WriteError(e, ErrorCategory.OperationStopped, this, true);
				return;
			}
			this.monitoringData.Events.Add(new MonitoringEvent(TestLiveIdAuthenticationTask.CmdletMonitoringEventSource, 3006, EventTypeEnumeration.Error, Strings.LiveIdConnectivityExceptionThrown(e.ToString())));
		}

		private const LiveIdAuthenticationUserTypeEnum DefaultUserType = LiveIdAuthenticationUserTypeEnum.ManagedConsumer;

		private const string ServerParam = "Server";

		private const string MailboxCredentialParam = "MailboxCredential";

		private const string MonitoringContextParam = "MonitoringContext";

		private const string PreferOfflineAuthParam = "PreferOfflineAuth";

		private const string TestFailOverParam = "TestFailOver";

		private const string IgnoreLowPasswordConfidenceParam = "IgnoreLowPasswordConfidence";

		private const string LiveIdXmlAuthParam = "LiveIdXmlAuth";

		private const string UserTypeParam = "UserType";

		private const int FailedEventIdBase = 2000;

		private const int SuccessEventIdBase = 1000;

		internal const string LiveIdAuthentication = "LiveIdAuthentication";

		private MonitoringData monitoringData;

		private ITopologyConfigurationSession systemConfigurationSession;

		public static readonly string CmdletMonitoringEventSource = "MSExchange Monitoring LiveIdAuthentication";

		public static readonly string PerformanceCounter = "LiveIdAuthentication Latency";

		public enum ScenarioId
		{
			PlaceHolderNoException = 1006,
			ExceptionThrown = 3006,
			AllTransactionsSucceeded = 3001
		}

		internal struct ClientAccessContext
		{
			internal Task Instance;

			internal bool MonitoringContext;

			internal ITopologyConfigurationSession ConfigurationSession;

			internal string WindowsDomain;

			internal ADSite Site;
		}
	}
}
