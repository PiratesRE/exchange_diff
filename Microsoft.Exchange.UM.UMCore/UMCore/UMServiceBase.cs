using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMServiceBase : ExServiceBase, IDisposeTrackable, IDisposable
	{
		public static string ServiceShortName { get; private set; }

		public static LocalizedString ServiceNameForEventLogging { get; private set; }

		public static LocalizedString ServerNameForEventLogging { get; private set; }

		public UMADSettings ADSettings { get; protected set; }

		public UMServiceBase()
		{
			ProcessLog.WriteLine("UMAbstractService::C'tor", new object[0]);
			base.ServiceName = UMServiceBase.ServiceShortName;
			base.CanStop = true;
			base.CanPauseAndContinue = false;
			base.AutoLog = false;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs eventArgs)
		{
			try
			{
				Exception ex = (Exception)eventArgs.ExceptionObject;
				string text = CommonUtil.ToEventLogString(ex);
				if (ex is UMServiceBaseException || ex is UnableToInitializeResourceException || ex is ExchangeServerNotFoundException)
				{
					if (!UMServiceBase.hasServiceStarted)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServiceStartFailure, null, new object[]
						{
							UMServiceBase.ServiceNameForEventLogging,
							text
						});
					}
					else
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServiceFatalException, null, new object[]
						{
							UMServiceBase.ServiceNameForEventLogging,
							text
						});
					}
				}
				else
				{
					if (!UMServiceBase.hasServiceStarted)
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServiceStartFailure, null, new object[]
						{
							UMServiceBase.ServiceNameForEventLogging,
							text
						});
					}
					else
					{
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServiceUnhandledException, null, new object[]
						{
							UMServiceBase.ServiceNameForEventLogging,
							text
						});
					}
					lock (UMServiceBase.mutex)
					{
						CallIdTracer.TraceError(ExTraceGlobals.ServiceTracer, 0, "Unhandled Exception Received, Sending Watson Report. Exception = " + ex.ToString(), new object[0]);
						using (ITempFile tempFile = Breadcrumbs.GenerateDump())
						{
							using (ITempFile tempFile2 = ProcessLog.GenerateDump())
							{
								ExWatson.TryAddExtraFile(tempFile.FilePath);
								ExWatson.TryAddExtraFile(tempFile2.FilePath);
								ExWatson.HandleException(sender, eventArgs);
							}
						}
					}
				}
			}
			finally
			{
				Utils.KillThisProcess();
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UMServiceBase>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public X509Certificate2 GetCertificateFromThumbprint(string thumbprint)
		{
			X509Certificate2 x509Certificate;
			try
			{
				x509Certificate = CertificateUtils.FindCertByThumbprint(thumbprint);
			}
			catch (SecurityException innerException)
			{
				throw new UMServiceBaseException(Strings.UnableToFindCertificate(thumbprint, UMServiceBase.ServerNameForEventLogging), innerException);
			}
			catch (CryptographicException innerException2)
			{
				throw new UMServiceBaseException(Strings.UnableToFindCertificate(thumbprint, UMServiceBase.ServerNameForEventLogging), innerException2);
			}
			if (x509Certificate == null)
			{
				CallIdTracer.TraceError(ExTraceGlobals.ServiceTracer, 0, "UMServiceBase.UpdateCertificate: Unable to find the cert in the store.", new object[0]);
				throw new UMServiceBaseException(Strings.UnableToFindCertificate(thumbprint, UMServiceBase.ServerNameForEventLogging));
			}
			if (CertificateUtils.IsExpired(x509Certificate))
			{
				CallIdTracer.TraceError(ExTraceGlobals.ServiceTracer, 0, "UMServiceBase.UpdateCertificate: Expired Certificate.", new object[0]);
				throw new UMServiceBaseException(Strings.ExpiredCertificate(thumbprint, UMServiceBase.ServerNameForEventLogging));
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceTracer, 0, "UMServiceBase.UpdateCertificate: Certificate found in the store with Thumbprint '{0}' and Issuer '{1}'.", new object[]
			{
				x509Certificate.Thumbprint,
				x509Certificate.Issuer
			});
			return x509Certificate;
		}

		protected static void Initialize(string serviceShortName, LocalizedString serviceName, LocalizedString serverName)
		{
			ProcessLog.WriteLine("UMAbstractService::Initialize", new object[0]);
			UMServiceBase.ServiceShortName = serviceShortName;
			UMServiceBase.ServiceNameForEventLogging = serviceName;
			UMServiceBase.ServerNameForEventLogging = serverName;
			int num = Privileges.RemoveAllExcept(new string[]
			{
				"SeAuditPrivilege",
				"SeChangeNotifyPrivilege",
				"SeCreateGlobalPrivilege",
				"SeIncreaseQuotaPrivilege",
				"SeAssignPrimaryTokenPrivilege"
			});
			if (num != 0)
			{
				UMServiceBaseException exception = new UMServiceBaseException(Strings.UnableToRemovePermissions(UMServiceBase.ServiceNameForEventLogging, num));
				UMServiceBase.GlobalExceptionHandler(null, new UnhandledExceptionEventArgs(exception, true));
			}
			ProcessLog.WriteLine("UMAbstractService::Initialize: Removed unnecessary privileges", new object[0]);
			ExWatson.Init();
			ProcessLog.WriteLine("UMAbstractService::Initialize: Initialized Watson.", new object[0]);
			AppDomain.CurrentDomain.UnhandledException += UMServiceBase.GlobalExceptionHandler;
			ProcessLog.WriteLine("UMAbstractService::Initialize: Register for Unhandled Exceptions", new object[0]);
			Globals.InitializeSinglePerfCounterInstance();
			ProcessLog.WriteLine("Initialize: Initialized single performance counters.", new object[0]);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			base.Dispose(disposing);
		}

		protected override void OnStopInternal()
		{
			ProcessLog.WriteLine("UMServiceBase::OnStopInternal", new object[0]);
			try
			{
				this.OnEventStartTime = ExDateTime.UtcNow;
				CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStopTracer, 0, "Stopping UMServiceBase {0}", new object[]
				{
					ExDateTime.UtcNow
				});
				ADNotificationsManager.Instance.Dispose();
				UmServiceGlobals.ShuttingDown = true;
				ProcessLog.WriteLine("OnStopInternal: Shutdown AD notifications.", new object[0]);
				lock (this)
				{
					this.DisposeModeChangedTimer();
				}
				this.InternalStop();
				if (this.certificateDiagnostics != null)
				{
					this.certificateDiagnostics.Dispose();
					this.certificateDiagnostics = null;
				}
				if (InstrumentationCollector.Stop())
				{
					ProcessLog.WriteLine("OnStopInternal: InstrumentationCollector Stopped", new object[0]);
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServiceStopSuccess, null, new object[]
				{
					UMServiceBase.ServiceNameForEventLogging
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStopTracer, 0, "Service stopped successfully", new object[0]);
				ProcessLog.WriteLine("OnStopInternal: Success.", new object[0]);
			}
			catch (Exception exception)
			{
				UMServiceBase.GlobalExceptionHandler(this, new UnhandledExceptionEventArgs(exception, true));
			}
		}

		protected override void OnStartInternal(string[] args)
		{
			ProcessLog.WriteLine("UMServiceBase::OnStartInternal", new object[0]);
			this.OnEventStartTime = ExDateTime.UtcNow;
			try
			{
				this.StartService();
				UMServiceBase.hasServiceStarted = true;
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServiceStartSuccess, null, new object[]
				{
					UMServiceBase.ServiceNameForEventLogging
				});
				CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "UMAbstractService started successfully", new object[0]);
				ProcessLog.WriteLine("OnStartInternal: Service Started Sucessfully", new object[0]);
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.ServiceStartTracer, 0, "UMServiceBase failed to start ", new object[]
				{
					ex.ToString()
				});
				if (ex is ADTransientException || ex is ADExternalException)
				{
					ex = new UMServiceBaseException(ex.Message, ex);
				}
				UMServiceBase.GlobalExceptionHandler(this, new UnhandledExceptionEventArgs(ex, true));
				throw;
			}
		}

		protected abstract void InternalStart();

		protected abstract void InternalStop();

		protected abstract CertificateDiagnostics CreateCertificateDiagnostics();

		protected abstract void HandleCertChange();

		protected abstract void LoadServiceADSettings();

		protected void ADServerUpdateCallback(ADNotificationEventArgs args)
		{
			lock (this)
			{
				if (args != null && args.Id != null && args.ChangeType == ADNotificationChangeType.ModifyOrAdd && this.ADSettings.Id.Equals(args.Id))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.ServiceTracer, 0, "ADServerUpdateCallback is for the local configuration.", new object[0]);
					this.ADSettings = this.ADSettings.RefreshFromAD();
					if (UMRecyclerConfig.UMStartupType != this.ADSettings.UMStartupMode)
					{
						this.StartModeChangedTimer(this.ADSettings.UMStartupMode);
					}
					else
					{
						this.DisposeModeChangedTimer();
						this.UpdateCertificateIfNeeded(UMRecyclerConfig.UMStartupType, this.ADSettings.UMCertificateThumbprint);
					}
				}
			}
		}

		private static void VerifyDatacenterMandatorySettings()
		{
			if (CommonConstants.UseDataCenterCallRouting)
			{
				if (SipPeerManager.Instance.SIPAccessService == null)
				{
					throw new UMServiceBaseException(Strings.SIPAccessServiceNotSet);
				}
				if (SipPeerManager.Instance.SBCService == null)
				{
					throw new UMServiceBaseException(Strings.SIPSessionBorderControllerNotSet);
				}
				if (SipPeerManager.Instance.AVAuthenticationService == null)
				{
					throw new UMServiceBaseException(Strings.AVAuthenticationServiceNotSet);
				}
			}
		}

		private void StartService()
		{
			ProcessLog.WriteLine("UMServiceBase::StartService", new object[0]);
			this.LogServiceStateChangeInfo("LoadServiceADSettings");
			this.LoadServiceADSettings();
			this.LogServiceStateChangeInfo("InstrumentationCollectorStart");
			if (InstrumentationCollector.Start(new SystemInstrumentationStrategy()))
			{
				ProcessLog.WriteLine("UMService:: InstrumentationCollector Started", new object[0]);
			}
			CallIdTracer.TracePfd(ExTraceGlobals.ServiceStartTracer, 0, "PFD UMS {0} - Starting UM Service {1}", new object[]
			{
				9210,
				ExDateTime.UtcNow
			});
			this.LogServiceStateChangeInfo("UMRecyclerConfigInit");
			UMRecyclerConfig.Init(this.ADSettings);
			ProcessLog.WriteLine("StartService: Initialized the UMRecycler config.", new object[0]);
			UmServiceGlobals.StartupMode = UMRecyclerConfig.UMStartupType;
			this.LogServiceStateChangeInfo("SipPeerManagerInitialize");
			SipPeerManager.Initialize(false, this.ADSettings);
			ProcessLog.WriteLine("StartService: Initialized SipPeerManager.", new object[0]);
			this.LogServiceStateChangeInfo("VerifyDatacenterMandatorySettings");
			UMServiceBase.VerifyDatacenterMandatorySettings();
			ProcessLog.WriteLine("StartService: Verified DatacenterMandatorySettings", new object[0]);
			this.InternalStart();
			this.LogServiceStateChangeInfo("InitializeCertificateDiagnosticsIfNecessary");
			this.InitializeCertificateDiagnosticsIfNecessary();
			this.LogServiceStateChangeInfo("InsertStartupNotification");
			StartupNotification.InsertStartupNotification(UMServiceBase.ServiceShortName);
			this.LogServiceStateChangeInfo("StartService completed");
		}

		private void DisposeModeChangedTimer()
		{
			if (this.modeChangedTimer != null)
			{
				this.modeChangedTimer.Dispose();
				this.modeChangedTimer = null;
			}
		}

		private void StartModeChangedTimer(UMStartupMode startupMode)
		{
			if (this.modeChangedTimer == null)
			{
				this.modeChangedTimer = new Timer(new TimerCallback(this.LogModeChangedEvent), null, 0, UMRecyclerConfig.AlertIntervalAfterStartupModeChanged * 1000);
			}
			this.ChangedMode = startupMode;
		}

		private void LogModeChangedEvent(object dummyObject)
		{
			lock (this)
			{
				if (this.modeChangedTimer != null)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMStartupModeChanged, null, new object[]
					{
						UMServiceBase.ServiceNameForEventLogging,
						LocalizedDescriptionAttribute.FromEnum(typeof(UMStartupMode), UMRecyclerConfig.UMStartupType),
						LocalizedDescriptionAttribute.FromEnum(typeof(UMStartupMode), this.ChangedMode)
					});
				}
			}
		}

		private void UpdateCertificateIfNeeded(UMStartupMode currentMode, string newCert)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceTracer, 0, "UMServiceBase.UpdateCertificate: Current mode {0}, and New thumbprint {1}.", new object[]
			{
				currentMode,
				newCert
			});
			if (currentMode != UMStartupMode.TCP && !string.IsNullOrEmpty(newCert) && !newCert.Equals(CertificateUtils.UMCertificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
			{
				CertificateUtils.UMCertificate = this.GetCertificateFromThumbprint(newCert);
				this.HandleCertChange();
				this.InitializeCertificateDiagnosticsIfNecessary();
			}
		}

		private void InitializeCertificateDiagnosticsIfNecessary()
		{
			if (this.certificateDiagnostics != null)
			{
				this.certificateDiagnostics.Dispose();
			}
			if (UmServiceGlobals.StartupMode != UMStartupMode.TCP)
			{
				this.certificateDiagnostics = this.CreateCertificateDiagnostics();
			}
		}

		protected void LogServiceStateChangeInfo(string info)
		{
			base.LogExWatsonTimeoutServiceStateChangeInfo(info + Environment.NewLine);
		}

		protected static volatile bool hasServiceStarted;

		protected static UMServiceBase umService;

		public ExDateTime OnEventStartTime;

		private static object mutex = new object();

		private DisposeTracker disposeTracker;

		private Timer modeChangedTimer;

		private CertificateDiagnostics certificateDiagnostics;

		public UMStartupMode ChangedMode;
	}
}
