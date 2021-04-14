using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Mapi;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UmServiceGlobals
	{
		private UmServiceGlobals()
		{
		}

		internal static TimeSpan ComponentStoptime
		{
			get
			{
				return UmServiceGlobals.componentStoptime;
			}
		}

		internal static OnWorkerProcessRetiredEventHandler OnWorkerProcessRetired
		{
			get
			{
				return UmServiceGlobals.onWorkerProcessRetired;
			}
			set
			{
				UmServiceGlobals.onWorkerProcessRetired = value;
			}
		}

		internal static bool WorkerProcessIsRetiring
		{
			get
			{
				return UmServiceGlobals.workerProcessIsRetiring;
			}
		}

		internal static UMStartupMode StartupMode
		{
			get
			{
				return UmServiceGlobals.startupMode;
			}
			set
			{
				UmServiceGlobals.startupMode = value;
			}
		}

		internal static bool ArePerfCountersEnabled
		{
			get
			{
				return UmServiceGlobals.arePerfCountersEnabled;
			}
			set
			{
				UmServiceGlobals.arePerfCountersEnabled = value;
			}
		}

		internal static BaseUMVoipPlatform VoipPlatform
		{
			get
			{
				return UmServiceGlobals.voipPlatform;
			}
		}

		internal static ADNotificationsManager ADNotifications
		{
			get
			{
				UmServiceGlobals.notifManager = ADNotificationsManager.Instance;
				return UmServiceGlobals.notifManager;
			}
		}

		internal static bool ShuttingDown
		{
			get
			{
				return UmServiceGlobals.shutDown;
			}
			set
			{
				UmServiceGlobals.shutDown = true;
			}
		}

		internal static void UmInitialize(int sipPort)
		{
			ProcessLog.WriteLine("UmServiceGlobals::UmInitialize", new object[0]);
			ProcessLog.WriteLine("UmInitialize: Initialize GlobalConfiguration", new object[0]);
			GlobCfg.Init();
			Directory.CreateDirectory(Utils.VoiceMailFilePath);
			Directory.CreateDirectory(Utils.UMBadMailFilePath);
			ProcessLog.WriteLine("UmInitialize: Initialize RecyclerConfiguration", new object[0]);
			UMRecyclerConfig.Init();
			ProcessLog.WriteLine("UmInitialize: Initialize State Machine", new object[0]);
			UmServiceGlobals.globManagerConfig = new GlobalActivityManager.ConfigClass();
			UmServiceGlobals.globManagerConfig.Load(GlobCfg.ConfigFile);
			ProcessLog.WriteLine("UmInitialize: Initialize RPC Server", new object[0]);
			ExRpcModule.Bind();
			ProcessLog.WriteLine("UmInitialize: Initialize Outdialing Diagnostics", new object[0]);
			OutdialingDiagnostics.ValidateProperties();
			ProcessLog.WriteLine("UmInitialize: Initialize SipPeerManager", new object[0]);
			SipPeerManager.Initialize(true, new UMServiceADSettings());
			ProcessLog.WriteLine("UmInitialize: Register for mailbox failures", new object[0]);
			MailboxSessionEstablisher.OnMailboxConnectionAttempted += UmServiceGlobals.MailboxSessionEstablisher_OnMailboxConnectionAttempted;
			CallIdTracer.TracePfd(ExTraceGlobals.ServiceStartTracer, 0, "PFD UMS {0} - Initializing VoIP.", new object[]
			{
				11834
			});
			UmServiceGlobals.voipPlatform = Platform.Builder.CreateVoipPlatform();
			ProcessLog.WriteLine("UmInitialize: Initialize VOIP Platform", new object[0]);
			UmServiceGlobals.voipPlatform.Initialize(sipPort, new UMCallSessionHandler<EventArgs>(UmServiceGlobals.CallHandler));
			ProcessLog.WriteLine("UmInitialize: Initialize SIP Peers", new object[0]);
			UmServiceGlobals.StartUMComponents(StartupStage.WPInitialization);
			ProcessLog.WriteLine("UmInitialize: Success", new object[0]);
		}

		private static void MailboxSessionEstablisher_OnMailboxConnectionAttempted(object sender, MailboxConnectionArgs e)
		{
			Util.IncrementCounter(AvailabilityCounters.PercentageFailedMailboxAccess_Base, 1L);
			if (!e.SuccessfulConnection)
			{
				Util.IncrementCounter(AvailabilityCounters.PercentageFailedMailboxAccess, 1L);
			}
			Util.SetCounter(AvailabilityCounters.RecentPercentageFailedMailboxAccess, (long)UmServiceGlobals.recentMailboxFailures.Update(e.SuccessfulConnection));
		}

		internal static void UmUninitialize()
		{
			ProcessLog.WriteLine("UmServiceGlobals::UmUnInitialize", new object[0]);
			UmServiceGlobals.shutDown = true;
			List<AutoResetEvent> list = new List<AutoResetEvent>();
			for (int i = UmServiceGlobals.umComponents.Length - 1; i >= 0; i--)
			{
				IUMAsyncComponent iumasyncComponent = UmServiceGlobals.umComponents[i];
				if (iumasyncComponent.IsInitialized)
				{
					ProcessLog.WriteLine("Stopping Component : " + iumasyncComponent.Name, new object[0]);
					iumasyncComponent.StopAsync();
					list.Add(iumasyncComponent.StoppedEvent);
				}
			}
			bool flag = WaitHandle.WaitAll(list.ToArray(), UmServiceGlobals.ComponentStoptime, false);
			if (flag)
			{
				ProcessLog.WriteLine("All Components shutdown in properTime. Cleaning up and exiting", new object[0]);
				foreach (IUMAsyncComponent iumasyncComponent2 in UmServiceGlobals.umComponents)
				{
					ProcessLog.WriteLine("CleanupAfterStopped for Component : " + iumasyncComponent2.Name, new object[0]);
					iumasyncComponent2.CleanupAfterStopped();
				}
				return;
			}
			ProcessLog.WriteLine("Some Components didn't shutdown in properTime. Hence exiting the process", new object[0]);
			Utils.KillThisProcess();
		}

		internal static void UmRetire(BaseUMVoipPlatform.FinalCallEndedDelegate finalCallEndedDelegate)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStopTracer, 0, "UMServiceGlobals::UmRetire", new object[0]);
			UmServiceGlobals.workerProcessIsRetiring = true;
			if (UmServiceGlobals.OnWorkerProcessRetired != null)
			{
				UmServiceGlobals.OnWorkerProcessRetired();
			}
			if (UmServiceGlobals.VoipPlatform != null)
			{
				UmServiceGlobals.VoipPlatform.Retire(finalCallEndedDelegate);
			}
		}

		internal static void InitializeCurrentCallsPerformanceCounters()
		{
			GeneralCounters.CurrentCalls.RawValue = 0L;
			GeneralCounters.CurrentUnauthenticatedPilotNumberCalls.RawValue = 0L;
			GeneralCounters.CurrentVoicemailCalls.RawValue = 0L;
			GeneralCounters.CurrentFaxCalls.RawValue = 0L;
			GeneralCounters.CurrentSubscriberAccessCalls.RawValue = 0L;
			GeneralCounters.CurrentAutoAttendantCalls.RawValue = 0L;
			GeneralCounters.CurrentPlayOnPhoneCalls.RawValue = 0L;
			GeneralCounters.CurrentPromptEditingCalls.RawValue = 0L;
		}

		internal static void InitializeCallAnswerQueuedMessagesPerformanceCounters()
		{
			if (!UmServiceGlobals.ArePerfCountersEnabled)
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(Utils.VoiceMailFilePath);
			FileInfo[] files = directoryInfo.GetFiles("*.txt");
			if (files != null)
			{
				AvailabilityCounters.TotalQueuedMessages.RawValue = (long)files.Length;
				return;
			}
			AvailabilityCounters.TotalQueuedMessages.RawValue = 0L;
		}

		internal static void InitializePerformanceCounters()
		{
			try
			{
				Utils.InitializePerformanceCounters(typeof(AvailabilityCounters));
				AvailabilityCounters.UMPipelineSLA.RawValue = 100L;
				Utils.InitializePerformanceCounters(typeof(CallAnswerCounters));
				Utils.InitializePerformanceCounters(typeof(FaxCounters));
				Utils.InitializePerformanceCounters(typeof(GeneralCounters));
				Utils.InitializePerformanceCounters(typeof(PerformanceCounters));
				Utils.InitializePerformanceCounters(typeof(SubscriberAccessCounters));
				UmServiceGlobals.ArePerfCountersEnabled = true;
				CallIdTracer.TracePfd(ExTraceGlobals.ServiceStartTracer, 0, "PFD UMS {0} - Perfcounters initialized successfully. ArePerfCountersEnabled value = {1}", new object[]
				{
					10810,
					UmServiceGlobals.ArePerfCountersEnabled
				});
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.ServiceStartTracer, 0, "Failed to initialize perfmon counters, perf data will not be available. Error: {0}", new object[]
				{
					ex.ToString()
				});
			}
		}

		internal static void StartUMComponents(StartupStage stage)
		{
			foreach (IUMAsyncComponent iumasyncComponent in UmServiceGlobals.umComponents)
			{
				ProcessLog.WriteLine(string.Concat(new object[]
				{
					"Stage:",
					stage,
					"  Starting Component : ",
					iumasyncComponent.Name
				}), new object[0]);
				iumasyncComponent.StartNow(stage);
			}
		}

		private static void CallHandler(BaseUMCallSession voiceObject, EventArgs e)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, 0, "Received call", new object[0]);
			ActivityManager activityManager = UmServiceGlobals.globManagerConfig.CreateActivityManager();
			activityManager.Start(voiceObject, null);
		}

		private static BaseUMVoipPlatform voipPlatform;

		private static PercentageBooleanSlidingCounter recentMailboxFailures = PercentageBooleanSlidingCounter.CreateFailureCounter(1000, TimeSpan.FromHours(1.0));

		private static bool shutDown;

		private static GlobalActivityManager.ConfigClass globManagerConfig;

		private static bool arePerfCountersEnabled;

		private static UMStartupMode startupMode = UMStartupMode.TCP;

		private static TimeSpan componentStoptime = new TimeSpan(0, 1, 30);

		private static ADNotificationsManager notifManager;

		private static bool workerProcessIsRetiring;

		private static OnWorkerProcessRetiredEventHandler onWorkerProcessRetired;

		private static IUMAsyncComponent[] umComponents = new IUMAsyncComponent[]
		{
			UmServiceGlobals.SyncComponents.Instance,
			PipelineDispatcher.Instance,
			UMServerRpcComponent.Instance,
			UMPlayOnPhoneRpcServerComponent.Instance,
			UMPartnerMessageRpcServerComponent.Instance,
			UMPromptPreviewRpcServerComponent.Instance,
			UMRecipientTasksServerComponent.Instance,
			UMServerPingRpcServerComponent.Instance,
			UMMwiDeliveryRpcServer.Instance,
			MobileSpeechRecoDispatcher.Instance,
			MobileSpeechRecoRpcServerComponent.Instance,
			CacheCleaner.Instance
		};

		internal class SyncComponents : IUMAsyncComponent
		{
			public AutoResetEvent StoppedEvent
			{
				get
				{
					return this.syncUMComponentStoppedEvent;
				}
			}

			public bool IsInitialized
			{
				get
				{
					return true;
				}
			}

			public string Name
			{
				get
				{
					return base.GetType().Name;
				}
			}

			internal static UmServiceGlobals.SyncComponents Instance
			{
				get
				{
					return UmServiceGlobals.SyncComponents.instance;
				}
			}

			public void StartNow(StartupStage stage)
			{
				if (stage == StartupStage.WPInitialization)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "{0} starting in stage {1}", new object[]
					{
						this.Name,
						stage
					});
					CallIdTracer.TracePfd(ExTraceGlobals.ServiceStartTracer, 0, "PFD UMS {0} - Registering Interface for Incoming Calls.", new object[]
					{
						13882
					});
					CallIdTracer.TracePfd(ExTraceGlobals.ServiceStartTracer, 0, "PFD UMS {0} - Attempting to Start Fax Job Manager.", new object[]
					{
						15930
					});
					ProcessLog.WriteLine("MediaMethods.InitializeACM()", new object[0]);
					MediaMethods.InitializeACM();
					CallStatisticsLogger.Instance.Init();
					PipelineStatisticsLogger.Instance.Init();
					MobileSpeechRequestStatisticsLogger.Instance.Init();
					CallPerformanceLogger.Instance.Init();
					CallRejectionLogger.Instance.Init();
					OffensiveWordsFilter.Init();
					ProcessLog.WriteLine("UmInitialize: Initialize Incoming Call Listener", new object[0]);
					UmServiceGlobals.voipPlatform.Start();
					TempFileFactory.StartCleanUpTimer();
					return;
				}
				if (stage == StartupStage.WPActivation)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "{0} starting in stage {1}", new object[]
					{
						this.Name,
						stage
					});
					UmServiceGlobals.InitializeCallAnswerQueuedMessagesPerformanceCounters();
					ProcessLog.WriteLine("Initialize: Initialized CA performance counters.", new object[0]);
					CurrentCallsCounterHelper.Instance.Init();
					ProcessLog.WriteLine("Initialize: Initialized call counter helper.", new object[0]);
				}
			}

			public void StopAsync()
			{
				this.syncUMComponentStoppedEvent.Reset();
				ProcessLog.WriteLine("UmUnInitialize: Uninitialize AD Notifications", new object[0]);
				if (UmServiceGlobals.notifManager != null)
				{
					UmServiceGlobals.notifManager.Dispose();
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStopTracer, 0, "in UmUninitialize: Shutting VoipPlatform", new object[0]);
				if (UmServiceGlobals.voipPlatform != null)
				{
					UmServiceGlobals.voipPlatform.Shutdown();
				}
				CallStatisticsLogger.Instance.Dispose();
				PipelineStatisticsLogger.Instance.Dispose();
				MobileSpeechRequestStatisticsLogger.Instance.Dispose();
				CallPerformanceLogger.Instance.Dispose();
				CallRejectionLogger.Instance.Dispose();
				ProcessLog.WriteLine("UmUnInitialize: Uninitialize Call counters", new object[0]);
				CurrentCallsCounterHelper.Instance.Shutdown();
				this.syncUMComponentStoppedEvent.Set();
				TempFileFactory.StopCleanUpTimer();
			}

			public void CleanupAfterStopped()
			{
				this.syncUMComponentStoppedEvent.Close();
			}

			private static UmServiceGlobals.SyncComponents instance = new UmServiceGlobals.SyncComponents();

			private AutoResetEvent syncUMComponentStoppedEvent = new AutoResetEvent(false);
		}
	}
}
