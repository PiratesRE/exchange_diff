using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.ProcessManager;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class ProtocolBaseServices : ControlObject.TransportWorker, IWorkerProcess, IDisposable
	{
		public ProtocolBaseServices()
		{
			ProtocolBaseServices.serverName = ComputerInformation.DnsFullyQualifiedDomainName;
			ProtocolBaseServices.serverVersion = "15.00.1497.015";
			ProtocolBaseServices.isMultiTenancyEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			this.LoadFlightingState();
			this.IsPartnerHostedOnly = DatacenterRegistry.IsPartnerHostedOnly();
			this.disposed = false;
			if (ProtocolBaseServices.service != null)
			{
				throw new ApplicationException("The service object already assigned!");
			}
			ProtocolBaseServices.service = this;
		}

		public static Microsoft.Exchange.Diagnostics.Trace ServerTracer
		{
			get
			{
				return ProtocolBaseServices.serverTracer;
			}
			set
			{
				ProtocolBaseServices.serverTracer = value;
			}
		}

		public static Microsoft.Exchange.Diagnostics.Trace SessionTracer
		{
			get
			{
				return ProtocolBaseServices.sessionTracer;
			}
			set
			{
				ProtocolBaseServices.sessionTracer = value;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ProtocolBaseServices.faultInjectionTracer == null)
				{
					ProtocolBaseServices.faultInjectionTracer = new FaultInjectionTrace(ProtocolBaseServices.popImapCoreComponentGuid, 0);
				}
				return ProtocolBaseServices.faultInjectionTracer;
			}
		}

		public static TroubleshootingContext TroubleshootingContext
		{
			get
			{
				return ProtocolBaseServices.troubleshootingContext;
			}
			set
			{
				ProtocolBaseServices.troubleshootingContext = value;
			}
		}

		public static string ServerName
		{
			get
			{
				return ProtocolBaseServices.serverName;
			}
		}

		public static string ServerVersion
		{
			get
			{
				return ProtocolBaseServices.serverVersion;
			}
		}

		public static string ExchangeSetupLocation
		{
			get
			{
				string result;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
				{
					if (registryKey == null)
					{
						throw new ApplicationException("ExchangeSetupLocation can't be found!");
					}
					result = (string)registryKey.GetValue("MsiInstallPath", null);
				}
				return result;
			}
		}

		public static VirtualServer VirtualServer
		{
			get
			{
				return ProtocolBaseServices.service.virtualServer;
			}
		}

		public static ProtocolBaseServices Service
		{
			get
			{
				return ProtocolBaseServices.service;
			}
		}

		public static bool IsMultiTenancyEnabled
		{
			get
			{
				return ProtocolBaseServices.isMultiTenancyEnabled;
			}
		}

		public static bool StoredSecretKeysValid { get; private set; }

		public static bool GCCEnabledWithKeys
		{
			get
			{
				return ResponseFactory.GlobalCriminalComplianceEnabled && ProtocolBaseServices.StoredSecretKeysValid;
			}
		}

		public static ServerServiceRole ServerRoleService
		{
			get
			{
				return ProtocolBaseServices.serverRoleService;
			}
		}

		public static ServerComponentEnum TargetProtocol
		{
			get
			{
				return ProtocolBaseServices.targetProtocol;
			}
			set
			{
				ProtocolBaseServices.targetProtocol = value;
			}
		}

		public bool EnableExactRFC822Size
		{
			get
			{
				return this.configuration.EnableExactRFC822Size;
			}
		}

		public int ConnectionCount
		{
			get
			{
				return this.virtualServer.ConnectionCount;
			}
		}

		public abstract string MaxConnectionsError { get; }

		public abstract string NoSslCertificateError { get; }

		public abstract string TimeoutError { get; }

		public abstract string ServerShutdownMessage { get; }

		public abstract ExEventLog.EventTuple ServerStartEventTuple { get; }

		public abstract ExEventLog.EventTuple ServerStopEventTuple { get; }

		public abstract ExEventLog.EventTuple NoConfigurationFoundEventTuple { get; }

		public abstract ExEventLog.EventTuple BasicOverPlainTextEventTuple { get; }

		public abstract ExEventLog.EventTuple MaxConnectionCountExceededEventTuple { get; }

		public abstract ExEventLog.EventTuple MaxConnectionsFromSingleIpExceededEventTuple { get; }

		public abstract ExEventLog.EventTuple SslConnectionNotStartedEventTuple { get; }

		public abstract ExEventLog.EventTuple PortBusyEventTuple { get; }

		public abstract ExEventLog.EventTuple SslCertificateNotFoundEventTuple { get; }

		public abstract ExEventLog.EventTuple ProcessNotRespondingEventTuple { get; }

		public abstract ExEventLog.EventTuple ControlAddressInUseEventTuple { get; }

		public abstract ExEventLog.EventTuple ControlAddressDeniedEventTuple { get; }

		public abstract ExEventLog.EventTuple SpnRegisterFailureEventTuple { get; }

		public abstract ExEventLog.EventTuple CreateMailboxLoggerFailedEventTuple { get; }

		public abstract ExEventLog.EventTuple NoPerfCounterTimerEventTuple { get; }

		public abstract ExEventLog.EventTuple OnlineValueChangedEventTuple { get; }

		public abstract ExEventLog.EventTuple BadPasswordCodePageEventTuple { get; }

		public abstract ExEventLog.EventTuple LrsListErrorEventTuple { get; }

		public abstract ExEventLog.EventTuple LrsPartnerResolutionWarningEventTuple { get; }

		public LoginOptions LoginType
		{
			get
			{
				return this.configuration.LoginType;
			}
		}

		public bool LiveIdBasicAuthReplacement
		{
			get
			{
				return this.configuration.LiveIdBasicAuthReplacement;
			}
		}

		public bool SuppressReadReceipt
		{
			get
			{
				return this.configuration.SuppressReadReceipt;
			}
		}

		public int ConnectionTimeout
		{
			get
			{
				return (int)this.configuration.AuthenticatedConnectionTimeout.TotalSeconds;
			}
		}

		public virtual CalendarItemRetrievalOptions CalendarItemRetrievalOption
		{
			get
			{
				return this.configuration.CalendarItemRetrievalOption;
			}
		}

		public virtual string OwaServer
		{
			get
			{
				return this.configuration.OwaServerUrl.AbsoluteUri;
			}
		}

		public int PreAuthConnectionTimeout
		{
			get
			{
				return (int)this.configuration.PreAuthenticatedConnectionTimeout.TotalSeconds;
			}
		}

		public int MaxCommandLength
		{
			get
			{
				return this.configuration.MaxCommandSize;
			}
		}

		public int MaxConnectionCount
		{
			get
			{
				return this.configuration.MaxConnections;
			}
		}

		public int MaxConcurrentConnectionsFromSingleIp
		{
			get
			{
				return this.configuration.MaxConnectionFromSingleIP;
			}
		}

		public int MaxConnectionsPerUser
		{
			get
			{
				return this.configuration.MaxConnectionsPerUser;
			}
		}

		public MimeTextFormat MessagesRetrievalMimeTextFormat
		{
			get
			{
				return this.configuration.MessageRetrievalMimeFormat;
			}
		}

		public int ProxyPort
		{
			get
			{
				return this.configuration.ProxyTargetPort;
			}
		}

		internal static string ServiceName
		{
			get
			{
				return ProtocolBaseServices.serviceName;
			}
			set
			{
				ProtocolBaseServices.serviceName = value;
			}
		}

		internal static string ShortServiceName { get; set; }

		internal static bool EnforceCertificateErrors { get; private set; }

		internal static bool LrsLogEnabled { get; private set; }

		internal static List<OrganizationId> LrsPartners
		{
			get
			{
				if (!ProtocolBaseServices.LrsLogEnabled)
				{
					return null;
				}
				lock (ProtocolBaseServices.service)
				{
					if (!ProtocolBaseServices.LrsLogEnabled)
					{
						return null;
					}
					if (ProtocolBaseServices.service.lrsPartners == null && !ProtocolBaseServices.service.LoadLrsPartnerList())
					{
						ProtocolBaseServices.LrsLogEnabled = false;
						return null;
					}
				}
				return ProtocolBaseServices.service.lrsPartners;
			}
		}

		internal static LrsLog LrsLog { get; private set; }

		internal LightWeightLog LightLog
		{
			get
			{
				return this.lightLog;
			}
		}

		internal ProtocolLoggingLevel ProtocolLoggingLevel
		{
			get
			{
				if (!this.lightLogEnabled)
				{
					return ProtocolLoggingLevel.None;
				}
				return ProtocolLoggingLevel.Verbose;
			}
		}

		internal int MaxFailedLoginAttempts
		{
			get
			{
				return this.maxFailedLoginAttempts;
			}
		}

		internal ExtendedProtectionConfig ExtendedProtectionConfig { get; private set; }

		internal bool GSSAPIAndNTLMAuthDisabled
		{
			get
			{
				return !this.configuration.EnableGSSAPIAndNTLMAuth || !ResponseFactory.KerberosAuthEnabled;
			}
		}

		internal bool IsPartnerHostedOnly { get; private set; }

		internal ProtocolConnectionSettings ExternalProxySettings { get; private set; }

		internal int PasswordCodePage
		{
			get
			{
				return this.passwordCodePage;
			}
		}

		protected static ExEventLog Logger
		{
			get
			{
				return ProtocolBaseServices.logger;
			}
			set
			{
				ProtocolBaseServices.logger = value;
			}
		}

		protected PopImapAdConfiguration Configuration
		{
			get
			{
				return this.configuration;
			}
			set
			{
				this.configuration = value;
			}
		}

		public static void Exit(int exitCode)
		{
			ProtocolBaseServices.ServerTracer.TraceDebug<int>(0L, "Exit {0} was called", exitCode);
			ProtocolBaseServices.InMemoryTraceOperationCompleted(0L);
			Environment.Exit(exitCode);
		}

		public static void SendWatsonForUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				ProtocolBaseServices.SendWatson(ex, true);
				return;
			}
			ExWatson.HandleException(sender, e);
		}

		public static void InMemoryTraceOperationCompleted(long id)
		{
			ProtocolBaseServices.TroubleshootingContext.TraceOperationCompletedAndUpdateContext(id);
		}

		public static bool AuthErrorReportEnabled(string username)
		{
			if (ProtocolBaseServices.authErrorReportEnabled || ResponseFactory.UseClientIpTestMocks)
			{
				return true;
			}
			if (string.IsNullOrEmpty(username))
			{
				return false;
			}
			username = username.ToLower();
			return username.StartsWith("healthmailbox") || username.EndsWith("exchangemon.net");
		}

		public static void SendWatson(Exception exception, bool terminating)
		{
			if (ExTraceConfiguration.Instance.InMemoryTracingEnabled)
			{
				ProtocolBaseServices.ServerTracer.TraceError<Exception>(0L, "Exception caught: {0}", exception);
				ProtocolBaseServices.TroubleshootingContext.SendExceptionReportWithTraces(exception, terminating, true);
				return;
			}
			if (exception != TroubleshootingContext.FaultInjectionInvalidOperationException)
			{
				ExWatson.SendReport(exception, terminating ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None, null);
			}
		}

		public static bool IsCriticalException(Exception exception)
		{
			return exception is OutOfMemoryException || exception is StackOverflowException || exception is AccessViolationException || exception is InvalidProgramException || exception is CannotUnloadAppDomainException || exception is BadImageFormatException || exception is TypeInitializationException || exception is TypeLoadException;
		}

		public abstract void LoadFlightingState();

		void IDisposable.Dispose()
		{
			if (this.resetHandle != null)
			{
				((IDisposable)this.resetHandle).Dispose();
				this.resetHandle = null;
			}
			if (this.stopHandle != null)
			{
				((IDisposable)this.stopHandle).Dispose();
				this.stopHandle = null;
			}
			if (this.readyHandle != null)
			{
				((IDisposable)this.readyHandle).Dispose();
				this.readyHandle = null;
			}
			if (!this.disposed)
			{
				this.disposed = true;
			}
			GC.SuppressFinalize(this);
		}

		public abstract PopImapAdConfiguration GetConfiguration(ITopologyConfigurationSession session);

		public void Retire()
		{
			try
			{
				this.stopHandle.Release();
			}
			catch (SemaphoreFullException)
			{
				ProtocolBaseServices.ServerTracer.TraceError(0L, "Stop handle already signaled!");
			}
		}

		public void Stop()
		{
			this.Retire();
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		public void Activate()
		{
		}

		public void ConfigUpdate()
		{
		}

		public void HandleMemoryPressure()
		{
		}

		public void ClearConfigCache()
		{
		}

		public void HandleBlockedSubmissionQueue()
		{
		}

		public void HandleLogFlush()
		{
		}

		public void HandleForceCrash()
		{
		}

		public void HandleConnection(Socket clientConnection)
		{
			this.virtualServer.AcceptClientConnection(clientConnection);
		}

		public abstract VirtualServer NewVirtualServer(ProtocolBaseServices server, PopImapAdConfiguration configuration);

		public void Start(PopImapAdConfiguration configuration, bool selfListening)
		{
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Start");
			if (configuration == null)
			{
				ProtocolBaseServices.ServerTracer.TraceError(0L, "No configuration found.");
				ProtocolBaseServices.Exit(1);
				return;
			}
			ObjectSchema instance = ObjectSchema.GetInstance<PopImapConditionalHandlerSchema>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("default", "SmtpAddress,TenantName,DisplayName,MailboxServer,Cmd,Parameters,Response,LightLogContext,Exception");
			BaseConditionalRegistration.Initialize(ProtocolBaseServices.ServiceName, instance, instance, dictionary);
			this.configuration = configuration;
			Kerberos.FlushTicketCache();
			if (ProtocolBaseServices.ServerTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ProtocolBaseServices.ServerTracer.Information<string>(0L, "Configuration of the server: {0}", this.configuration.DisplayString());
			}
			this.virtualServer = this.NewVirtualServer(this, this.configuration);
			if (!this.virtualServer.Initialize())
			{
				ProtocolBaseServices.ServerTracer.TraceError(0L, "Could not initialize virtual server.");
				ProtocolBaseServices.Exit(1);
				return;
			}
			StoreSession.UseRPCContextPool = true;
			if (this.LiveIdBasicAuthReplacement)
			{
				int num;
				if (!int.TryParse(ConfigurationManager.AppSettings["PodSiteStartRange"], out num))
				{
					ProtocolBaseServices.ServerTracer.TraceError(0L, "No pod site start range specified in exe.config.");
					ProtocolBaseServices.Exit(1);
					return;
				}
				ResponseFactory.PodSiteStartRange = num;
				if (!int.TryParse(ConfigurationManager.AppSettings["PodSiteEndRange"], out num))
				{
					ProtocolBaseServices.ServerTracer.TraceError(0L, "No pod site end range specified in exe.config.");
					ProtocolBaseServices.Exit(1);
					return;
				}
				ResponseFactory.PodSiteEndRange = num;
				ResponseFactory.PodRedirectTemplate = ConfigurationManager.AppSettings["PodRedirectTemplate"];
				ProtocolBaseServices.StoredSecretKeysValid = GccUtils.AreStoredSecretKeysValid();
			}
			if (!int.TryParse(ConfigurationManager.AppSettings["OfflineTimerCheckSeconds"], out ProtocolBaseServices.onlineCacheThreshold))
			{
				ProtocolBaseServices.onlineCacheThreshold = 60;
			}
			if (!bool.TryParse(ConfigurationManager.AppSettings["AuthErrorReportEnabled"], out ProtocolBaseServices.authErrorReportEnabled))
			{
				ProtocolBaseServices.authErrorReportEnabled = false;
			}
			int num2;
			if (!int.TryParse(ConfigurationManager.AppSettings["MaxIoThreadsPerCPU"], out num2))
			{
				num2 = 0;
			}
			if (num2 < 1 || num2 > 1000)
			{
				num2 = 0;
			}
			int num3;
			int arg;
			if (num2 != 0)
			{
				ThreadPool.GetMaxThreads(out num3, out arg);
				ThreadPool.SetMaxThreads(num3, Environment.ProcessorCount * num2);
			}
			ThreadPool.GetMaxThreads(out num3, out arg);
			ProtocolBaseServices.ServerTracer.Information<int, int>(0L, "Maximum worker threads: {0}, Maximum completion port threads: {1}", num3, arg);
			if (configuration.LoginType == LoginOptions.PlainTextLogin && this.configuration.UnencryptedOrTLSBindings.Count > 0)
			{
				ProtocolBaseServices.ServerTracer.TraceWarning(0L, "Basic authentication is available over plain text connections!");
				ProtocolBaseServices.LogEvent(this.BasicOverPlainTextEventTuple, null, new string[0]);
			}
			if (selfListening)
			{
				List<IPBinding> list = new List<IPBinding>();
				if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox)
				{
					list.Add(new IPBinding(IPAddress.Any, this.configuration.ProxyTargetPort));
					list.Add(new IPBinding(IPAddress.IPv6Any, this.configuration.ProxyTargetPort));
				}
				else
				{
					list = this.configuration.GetBindings();
				}
				this.connectionPools = new List<ConnectionPool>(list.Count);
				foreach (IPBinding ipbinding in list)
				{
					try
					{
						this.connectionPools.Add(new ConnectionPool(ipbinding, new ConnectionPool.ConnectionAcceptedDelegate(this.virtualServer.AcceptClientConnection)));
					}
					catch (SocketException arg2)
					{
						ProtocolBaseServices.ServerTracer.TraceError<IPBinding, SocketException>(0L, "Exception caught while opening port {0}:\r\n{1}", ipbinding, arg2);
						ProtocolBaseServices.LogEvent(this.PortBusyEventTuple, null, new string[]
						{
							ipbinding.ToString()
						});
						ProtocolBaseServices.Exit(1);
					}
				}
			}
			this.ConfigureExtendedProtection();
			ProtocolBaseServices.LogEvent(this.ServerStartEventTuple, null, new string[0]);
		}

		public virtual void TerminateAllSessions()
		{
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "TerminateAllSessions");
			this.virtualServer.Stop();
			ProtocolBaseServices.LogEvent(this.ServerStopEventTuple, null, new string[0]);
		}

		public void WaitForHangSignal()
		{
			this.hangHandle.WaitOne();
			ProtocolBaseServices.LogEvent(this.ProcessNotRespondingEventTuple, null, new string[0]);
			string message = string.Format(ProtocolBaseStrings.ProcessNotResponding, "Hang during stop");
			Exception exception = new Exception(message);
			ProtocolBaseServices.SendWatson(exception, true);
			Environment.Exit(1);
		}

		internal static void Assert(bool condition, string formatString, params object[] parameters)
		{
		}

		internal static bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params string[] messageArgs)
		{
			return ProtocolBaseServices.logger.LogEvent(tuple, periodicKey, messageArgs);
		}

		protected static Exception FaultInjectionCallback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null && exceptionType.Equals("System.InvalidOperationException", StringComparison.OrdinalIgnoreCase))
			{
				result = TroubleshootingContext.FaultInjectionInvalidOperationException;
			}
			return result;
		}

		protected static void DetermineServiceRole()
		{
			string fileName;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				fileName = currentProcess.MainModule.FileName;
			}
			string directoryName = Path.GetDirectoryName(fileName);
			string directoryName2 = Path.GetDirectoryName(directoryName);
			int num = directoryName2.LastIndexOf("FrontEnd", StringComparison.OrdinalIgnoreCase);
			int num2 = directoryName2.LastIndexOf("ClientAccess", StringComparison.OrdinalIgnoreCase);
			if (num == directoryName2.Length - "FrontEnd".Length)
			{
				ProtocolBaseServices.serverRoleService = ServerServiceRole.cafe;
				return;
			}
			if (num2 == directoryName2.Length - "ClientAccess".Length)
			{
				ProtocolBaseServices.serverRoleService = ServerServiceRole.mailbox;
				return;
			}
			throw new FileLoadException("Installation error: Pop Imap executables not found under ClientAccess or FrontEnd subdirectory: {0}", fileName);
		}

		protected void ProcessGetConfigurationException(Exception ex)
		{
			if (ex != null)
			{
				ProtocolBaseServices.ServerTracer.TraceError<Exception>(0L, "{0}", ex);
				ProtocolBaseServices.LogEvent(this.NoConfigurationFoundEventTuple, null, new string[]
				{
					ex.Message
				});
				return;
			}
			ProtocolBaseServices.ServerTracer.TraceError(0L, "No configuration found.");
			ProtocolBaseServices.LogEvent(this.NoConfigurationFoundEventTuple, null, new string[]
			{
				string.Empty
			});
		}

		protected void Run(string[] args)
		{
			long value = 0L;
			bool flag = false;
			bool flag2 = false;
			string text = null;
			string name = null;
			string name2 = null;
			string name3 = null;
			ProtocolBaseServices.ServerTracer.TraceDebug<string>(0L, "Process {0} starting", ProtocolBaseServices.ServiceName);
			foreach (string text2 in args)
			{
				if (text2.StartsWith("-?", StringComparison.OrdinalIgnoreCase))
				{
					ProtocolBaseServices.Usage();
					ProtocolBaseServices.Exit(0);
				}
				if (text2.StartsWith("-stopkey:", StringComparison.OrdinalIgnoreCase))
				{
					text = text2.Remove(0, "-stopkey:".Length);
				}
				else if (text2.StartsWith("-hangkey:", StringComparison.OrdinalIgnoreCase))
				{
					name = text2.Remove(0, "-hangkey:".Length);
				}
				else if (text2.StartsWith("-resetkey:", StringComparison.OrdinalIgnoreCase))
				{
					name2 = text2.Remove(0, "-resetkey:".Length);
				}
				else if (text2.StartsWith("-readykey:", StringComparison.OrdinalIgnoreCase))
				{
					name3 = text2.Remove(0, "-readykey:".Length);
				}
				else if (text2.StartsWith("-pipe:", StringComparison.OrdinalIgnoreCase))
				{
					value = long.Parse(text2.Remove(0, "-pipe:".Length));
				}
				else if (text2.StartsWith("-wait", StringComparison.OrdinalIgnoreCase))
				{
					flag2 = true;
				}
				else if (text2.StartsWith("-console", StringComparison.OrdinalIgnoreCase))
				{
					ProtocolBaseServices.runningFromConsole = true;
				}
			}
			flag = !string.IsNullOrEmpty(text);
			if (!flag)
			{
				if (!ProtocolBaseServices.runningFromConsole)
				{
					ProtocolBaseServices.Usage();
					ProtocolBaseServices.Exit(0);
				}
				Console.WriteLine("Starting {0}, running in console mode.", ProtocolBaseServices.ServiceName);
				if (flag2)
				{
					Console.WriteLine("Press ENTER to continue.");
					Console.ReadLine();
				}
			}
			this.resetHandle = ProtocolBaseServices.OpenSemaphore(name2, "reset");
			this.stopHandle = ProtocolBaseServices.OpenSemaphore(text, "stop");
			this.hangHandle = ProtocolBaseServices.OpenSemaphore(name, "hang");
			this.readyHandle = ProtocolBaseServices.OpenSemaphore(name3, "ready");
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Protocolbase Service::Run - Parsed Arguments & Called OpenSemaphore");
			if (this.hangHandle != null)
			{
				Thread thread = new Thread(new ThreadStart(this.WaitForHangSignal));
				thread.Start();
			}
			if (!int.TryParse(ConfigurationManager.AppSettings["MaxFailedLoginAttempts"], out this.maxFailedLoginAttempts))
			{
				this.maxFailedLoginAttempts = 4;
			}
			if (!int.TryParse(ConfigurationManager.AppSettings["PasswordCodePage"], out this.passwordCodePage))
			{
				this.passwordCodePage = 1252;
			}
			if (!this.TestPasswordDecoder(this.passwordCodePage))
			{
				ProtocolBaseServices.Exit(0);
			}
			this.RegisterSpn();
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Protocolbase Service::Run - RegisterSpn called successfully.");
			Globals.InitializeMultiPerfCounterInstance("PopImap");
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Protocolbase Service::Run - Initialized MultiPerf Counter instance for PopImap.");
			string text3 = ConfigurationManager.AppSettings["LogFileNameTemplate"];
			if (string.IsNullOrEmpty(text3))
			{
				if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.cafe)
				{
					text3 = ProtocolBaseServices.serviceName;
				}
				else
				{
					text3 = ProtocolBaseServices.serviceName + "BE";
				}
			}
			if (this.IsPartnerHostedOnly)
			{
				string text4 = ConfigurationManager.AppSettings["ExternalProxy"];
				if (!string.IsNullOrEmpty(text4))
				{
					try
					{
						this.ExternalProxySettings = ProtocolConnectionSettings.Parse(text4);
					}
					catch (FormatException ex)
					{
						ProtocolBaseServices.ServerTracer.TraceError(0L, "ExternalProxy value is invalid.\r\n" + ex.Message);
					}
				}
			}
			PopImapAdConfiguration popImapAdConfiguration = this.GetConfiguration();
			ProtocolBaseServices.ServerTracer.TraceDebug<string>(0L, "Reading configuration from AD was successful. ConfigurationValues:{0}", (popImapAdConfiguration == null) ? "<null>" : popImapAdConfiguration.ToString());
			if (popImapAdConfiguration == null)
			{
				ProtocolBaseServices.Exit(0);
			}
			this.lightLogEnabled = popImapAdConfiguration.ProtocolLogEnabled;
			ProtocolBaseServices.EnforceCertificateErrors = popImapAdConfiguration.EnforceCertificateErrors;
			if (this.lightLogEnabled)
			{
				int num;
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DurationToKeepLogsFor"]) || !int.TryParse(ConfigurationManager.AppSettings["DurationToKeepLogsFor"], out num))
				{
					num = 7;
				}
				TimeSpan timeSpan = (num > 0 && ResponseFactory.EnforceLogsRetentionPolicyEnabled) ? TimeSpan.FromDays((double)num) : TimeSpan.MaxValue;
				this.lightLog = new LightWeightLog("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version, ProtocolBaseServices.serviceName + " Log", text3, ProtocolBaseServices.serviceName + "Logs");
				if (!popImapAdConfiguration.LogPerFileSizeQuota.IsUnlimited && popImapAdConfiguration.LogPerFileSizeQuota.Value == ByteQuantifiedSize.Zero)
				{
					this.lightLog.Configure(popImapAdConfiguration.LogFileLocation, popImapAdConfiguration.LogFileRollOverSettings, timeSpan);
				}
				else
				{
					this.lightLog.Configure(popImapAdConfiguration.LogFileLocation, timeSpan, long.MaxValue, (long)(popImapAdConfiguration.LogPerFileSizeQuota.IsUnlimited ? 9223372036854775807UL : popImapAdConfiguration.LogPerFileSizeQuota.Value.ToBytes()));
				}
				ProtocolBaseServices.LrsLogEnabled = (ProtocolBaseServices.IsMultiTenancyEnabled && ResponseFactory.LrsLoggingEnabled && ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox);
				if (ProtocolBaseServices.LrsLogEnabled)
				{
					string text5 = ConfigurationManager.AppSettings["LogFileNameTemplate"];
					if (string.IsNullOrEmpty(text5))
					{
						text5 = ProtocolBaseServices.serviceName;
					}
					text5 += "LRS";
					ProtocolBaseServices.LrsLog = new LrsLog("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version, ProtocolBaseServices.serviceName + " Lrs Log", text5, ProtocolBaseServices.serviceName + "LrsLogs");
					if (!popImapAdConfiguration.LogPerFileSizeQuota.IsUnlimited && popImapAdConfiguration.LogPerFileSizeQuota.Value == ByteQuantifiedSize.Zero)
					{
						ProtocolBaseServices.LrsLog.Configure(popImapAdConfiguration.LogFileLocation, popImapAdConfiguration.LogFileRollOverSettings, timeSpan);
					}
					else
					{
						ProtocolBaseServices.LrsLog.Configure(popImapAdConfiguration.LogFileLocation, timeSpan, long.MaxValue, (long)(popImapAdConfiguration.LogPerFileSizeQuota.IsUnlimited ? 9223372036854775807UL : popImapAdConfiguration.LogPerFileSizeQuota.Value.ToBytes()));
					}
				}
			}
			this.Start(popImapAdConfiguration, !flag);
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Register Diagnostics components.");
			ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents();
			PipeStream pipeStream = null;
			if (flag)
			{
				SafeFileHandle handle = new SafeFileHandle(new IntPtr(value), true);
				pipeStream = new PipeStream(handle, FileAccess.Read, true);
				this.controlObject = new ControlObject(pipeStream, this);
				if (this.controlObject.Initialize())
				{
					if (this.readyHandle != null)
					{
						ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Signal the process is ready");
						this.readyHandle.Release();
						this.readyHandle.Close();
						this.readyHandle = null;
					}
					ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Wait for shutdown signal to exit");
					this.stopHandle.WaitOne();
				}
			}
			else
			{
				Console.WriteLine("Press ENTER to exit ");
				Console.ReadLine();
			}
			ProtocolBaseServices.ServerTracer.TraceDebug(0L, "Received a signal to shutdown");
			if (this.connectionPools != null)
			{
				foreach (ConnectionPool connectionPool in this.connectionPools)
				{
					connectionPool.Shutdown();
				}
			}
			this.TerminateAllSessions();
			if (pipeStream != null)
			{
				try
				{
					pipeStream.Close();
				}
				catch (IOException)
				{
				}
				catch (ObjectDisposedException)
				{
				}
			}
			if (this.lightLog != null)
			{
				this.lightLog.Close();
			}
			if (ProtocolBaseServices.LrsLog != null)
			{
				ProtocolBaseServices.LrsLog.Close();
			}
		}

		internal bool TestPasswordDecoder(int codePage)
		{
			Decoder decoder = null;
			bool result = true;
			try
			{
				decoder = Encoding.GetEncoding(codePage).GetDecoder();
			}
			catch (ArgumentException)
			{
				ProtocolBaseServices.ServerTracer.TraceError<int>(0L, "ArgumentException thrown while testing PasswordCodePage {0}.", codePage);
				ProtocolBaseServices.LogEvent(this.BadPasswordCodePageEventTuple, null, new string[]
				{
					"ArgumentException thrown while testing PasswordCodePage."
				});
				result = false;
			}
			catch (NotSupportedException)
			{
				ProtocolBaseServices.ServerTracer.TraceError<int>(0L, "NotSupportedException thrown while testing PasswordCodePage {0}.", codePage);
				ProtocolBaseServices.LogEvent(this.BadPasswordCodePageEventTuple, null, new string[]
				{
					"NotSupportedException thrown while testing PasswordCodePage."
				});
				result = false;
			}
			if (decoder == null)
			{
				decoder = Encoding.GetEncoding(20127).GetDecoder();
				if (decoder == null)
				{
					ProtocolBaseServices.ServerTracer.TraceError<int>(0L, "No Ascii decoder created while testing PasswordCodePage {0}.", codePage);
					ProtocolBaseServices.LogEvent(this.BadPasswordCodePageEventTuple, null, new string[]
					{
						"No Ascii decoder created while testing PasswordCodePage."
					});
					result = false;
				}
			}
			return result;
		}

		internal bool IsOnline()
		{
			if (ProtocolBaseServices.ServerRoleService == ServerServiceRole.mailbox)
			{
				return true;
			}
			if (ExDateTime.UtcNow < ProtocolBaseServices.onlineLastCheckTime.AddSeconds((double)ProtocolBaseServices.onlineCacheThreshold))
			{
				return ProtocolBaseServices.isOnline;
			}
			bool flag = ProtocolBaseServices.isOnline;
			ProtocolBaseServices.isOnline = ServerComponentStateManager.IsOnline(ProtocolBaseServices.TargetProtocol);
			ProtocolBaseServices.onlineLastCheckTime = ExDateTime.UtcNow;
			if (flag != ProtocolBaseServices.isOnline)
			{
				ProtocolBaseServices.LogEvent(this.OnlineValueChangedEventTuple, null, new string[]
				{
					ProtocolBaseServices.isOnline.ToString()
				});
			}
			return ProtocolBaseServices.isOnline;
		}

		private static Semaphore OpenSemaphore(string name, string semaphoreLabel)
		{
			Semaphore semaphore = null;
			if (!string.IsNullOrEmpty(name))
			{
				try
				{
					semaphore = Semaphore.OpenExisting(name);
				}
				catch (WaitHandleCannotBeOpenedException)
				{
					semaphore = null;
				}
				catch (UnauthorizedAccessException)
				{
					semaphore = null;
				}
				if (semaphore == null)
				{
					ProtocolBaseServices.ServerTracer.TraceError<string, string>(0L, "Failed to open the {0} semaphore (name={1}). Exiting.", semaphoreLabel, name);
					Environment.Exit(1);
				}
			}
			return semaphore;
		}

		private static void Usage()
		{
			Console.WriteLine(ProtocolBaseStrings.UsageText, Assembly.GetExecutingAssembly().GetName().Name, ProtocolBaseServices.serviceName);
		}

		private PopImapAdConfiguration GetConfiguration()
		{
			PopImapAdConfiguration result;
			try
			{
				ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2108, "GetConfiguration", "f:\\15.00.1497\\sources\\dev\\PopImap\\src\\Core\\ProtocolBaseServices.cs");
				result = this.GetConfiguration(session);
			}
			catch (LocalizedException ex)
			{
				this.ProcessGetConfigurationException(ex);
				result = null;
			}
			return result;
		}

		private void RegisterSpn()
		{
			this.RegisterSpn(ProtocolBaseServices.ServiceName);
			this.RegisterSpn(ProtocolBaseServices.ShortServiceName);
		}

		private void RegisterSpn(string name)
		{
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				if (!current.User.IsWellKnown(WellKnownSidType.NetworkServiceSid) && !current.User.IsWellKnown(WellKnownSidType.LocalSystemSid))
				{
					return;
				}
			}
			int num = ServicePrincipalName.RegisterServiceClass(name);
			if (num != 0)
			{
				ProtocolBaseServices.ServerTracer.TraceError<int>(0L, "RegisterServiceClass returned {0}", num);
				ProtocolBaseServices.LogEvent(this.SpnRegisterFailureEventTuple, null, new string[]
				{
					num.ToString()
				});
			}
		}

		private void ConfigureExtendedProtection()
		{
			if (this.configuration.ExtendedProtectionPolicy != ExtendedProtectionTokenCheckingMode.None)
			{
				this.ExtendedProtectionConfig = new ExtendedProtectionConfig((int)this.configuration.ExtendedProtectionPolicy, null, false);
				return;
			}
			this.ExtendedProtectionConfig = ExtendedProtectionConfig.NoExtendedProtection;
		}

		private bool LoadLrsPartnerList()
		{
			return true;
		}

		private const string StopKeyOption = "-stopkey:";

		private const string HangKeyOption = "-hangkey:";

		private const string ResetKeyOption = "-resetkey:";

		private const string ReadyKeyOption = "-readykey:";

		private const string ControlPipeOption = "-pipe:";

		private const string HelpOption = "-?";

		private const string WaitToContinue = "-wait";

		private const string ConsoleOption = "-console";

		private const string ServerRoleCafe = "FrontEnd";

		private const string ServerRoleMbx = "ClientAccess";

		private const string DefaultPropertyGroup = "SmtpAddress,TenantName,DisplayName,MailboxServer,Cmd,Parameters,Response,LightLogContext,Exception";

		private const int DefaultMaxIoThreadsValue = 0;

		private const int MaximumMaxIoThreadsValue = 1000;

		private const int MinimumMaxIoThreadsValue = 1;

		private const int TagFaultInjection = 0;

		private const int MaintenanceModeCheckIntervalSeconds = 60;

		private const int DefaultDurationToKeepLogsFor = 7;

		private const string ExchangeSetupLocationKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private const string ExchangeInstallPathValue = "MsiInstallPath";

		private static Guid popImapCoreComponentGuid = new Guid("EFEA6219-825A-4d56-9B26-7393EF24D917");

		private static bool runningFromConsole;

		private static bool isMultiTenancyEnabled;

		private static ExEventLog logger;

		private static string serverName;

		private static string serverVersion;

		private static string serviceName;

		private static Microsoft.Exchange.Diagnostics.Trace serverTracer;

		private static Microsoft.Exchange.Diagnostics.Trace sessionTracer;

		private static FaultInjectionTrace faultInjectionTracer;

		private static TroubleshootingContext troubleshootingContext;

		private static ProtocolBaseServices service;

		private static ServerServiceRole serverRoleService = ServerServiceRole.unknown;

		private static ServerComponentEnum targetProtocol;

		private static bool isOnline = true;

		private static ExDateTime onlineLastCheckTime = ExDateTime.MinValue;

		private static int onlineCacheThreshold;

		private static bool authErrorReportEnabled;

		private Semaphore resetHandle;

		private Semaphore stopHandle;

		private Semaphore hangHandle;

		private Semaphore readyHandle;

		private PopImapAdConfiguration configuration;

		private ControlObject controlObject;

		private bool lightLogEnabled;

		private LightWeightLog lightLog;

		private int maxFailedLoginAttempts;

		private VirtualServer virtualServer;

		private List<ConnectionPool> connectionPools;

		private int passwordCodePage;

		private bool disposed;

		private List<OrganizationId> lrsPartners;
	}
}
