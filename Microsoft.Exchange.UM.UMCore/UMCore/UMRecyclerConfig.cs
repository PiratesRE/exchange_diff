using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMRecyclerConfig
	{
		private UMRecyclerConfig()
		{
		}

		internal static string TempFilePath
		{
			get
			{
				return UMRecyclerConfig.tempFilePath;
			}
		}

		internal static string CertFileName
		{
			get
			{
				return UMRecyclerConfig.certFileName;
			}
		}

		internal static string Tempdir
		{
			get
			{
				return UMRecyclerConfig.tempDir;
			}
		}

		internal static int DaysBeforeCertExpiryForAlert
		{
			get
			{
				return UMRecyclerConfig.daysBeforeCertExpiryForAlert;
			}
		}

		internal static int SubsequentAlertIntervalAfterFirstAlert
		{
			get
			{
				return UMRecyclerConfig.subsequentAlertIntervalAfterFirstAlert;
			}
		}

		internal static UMStartupMode UMStartupType
		{
			get
			{
				return UMRecyclerConfig.umStartupType;
			}
		}

		internal static int TcpListeningPort
		{
			get
			{
				return UMRecyclerConfig.tcpPort;
			}
		}

		internal static int TlsListeningPort
		{
			get
			{
				return UMRecyclerConfig.tlsPort;
			}
		}

		internal static double MaxPrivateBytesPercent
		{
			get
			{
				return UMRecyclerConfig.maxPrivateBytesPercent;
			}
			set
			{
				UMRecyclerConfig.maxPrivateBytesPercent = value;
			}
		}

		internal static ulong MaxTempDirSize
		{
			get
			{
				return UMRecyclerConfig.maxTempDirSize;
			}
			set
			{
				UMRecyclerConfig.maxTempDirSize = value;
			}
		}

		internal static int MaxCallsBeforeRecycle
		{
			get
			{
				return UMRecyclerConfig.maxCallsBeforeRecycle;
			}
			set
			{
				UMRecyclerConfig.maxCallsBeforeRecycle = value;
			}
		}

		internal static ulong RecycleInterval
		{
			get
			{
				return UMRecyclerConfig.recycleInterval;
			}
			set
			{
				UMRecyclerConfig.recycleInterval = value;
			}
		}

		internal static int Worker1SipPortNumber
		{
			get
			{
				return UMRecyclerConfig.worker1SipPortNumber;
			}
			set
			{
				UMRecyclerConfig.worker1SipPortNumber = value;
			}
		}

		internal static int Worker2SipPortNumber
		{
			get
			{
				return UMRecyclerConfig.worker2SipPortNumber;
			}
			set
			{
				UMRecyclerConfig.worker2SipPortNumber = value;
			}
		}

		internal static int HeartBeatInterval
		{
			get
			{
				return UMRecyclerConfig.heartBeatInterval;
			}
			set
			{
				UMRecyclerConfig.heartBeatInterval = value;
			}
		}

		internal static int MaxHeartBeatFailures
		{
			get
			{
				return UMRecyclerConfig.maxHeartBeatFailures;
			}
			set
			{
				UMRecyclerConfig.maxHeartBeatFailures = value;
			}
		}

		internal static int PingInterval
		{
			get
			{
				return UMRecyclerConfig.pingInterval;
			}
			set
			{
				UMRecyclerConfig.pingInterval = value;
			}
		}

		internal static int ResourceMonitorInterval
		{
			get
			{
				return UMRecyclerConfig.resourceMonitorInterval;
			}
			set
			{
				UMRecyclerConfig.resourceMonitorInterval = value;
			}
		}

		internal static int ThrashCountMaximum
		{
			get
			{
				return UMRecyclerConfig.thrashCountMaximum;
			}
			set
			{
				UMRecyclerConfig.thrashCountMaximum = value;
			}
		}

		internal static int StartupTime
		{
			get
			{
				return UMRecyclerConfig.startupTime;
			}
			set
			{
				UMRecyclerConfig.startupTime = value;
			}
		}

		internal static int RetireTime
		{
			get
			{
				return UMRecyclerConfig.retireTime;
			}
			set
			{
				UMRecyclerConfig.retireTime = value;
			}
		}

		internal static int HeartBeatResponseTime
		{
			get
			{
				return UMRecyclerConfig.heartBeatResponseTime;
			}
			set
			{
				UMRecyclerConfig.heartBeatResponseTime = value;
			}
		}

		internal static int AlertIntervalAfterStartupModeChanged
		{
			get
			{
				return UMRecyclerConfig.alertIntervalAfterStartupModeChanged;
			}
		}

		internal static bool UseDataCenterActiveManagerRouting
		{
			get
			{
				return UMRecyclerConfig.useDataCenterActiveManagerRouting;
			}
		}

		internal static void Init()
		{
			UMRecyclerConfig.Init(new UMServiceADSettings());
		}

		internal static void Init(UMADSettings adSettings)
		{
			try
			{
				AppConfig instance = AppConfig.Instance;
				UMRecyclerConfig.retireTime = 1800;
				UMRecyclerConfig.worker1SipPortNumber = instance.Recycler.WorkerSIPPort;
				UMRecyclerConfig.maxPrivateBytesPercent = (double)instance.Recycler.MaxPrivateBytesPercent;
				UMRecyclerConfig.maxTempDirSize = (ulong)((long)Math.Max(0, instance.Recycler.MaxTempDirSize));
				UMRecyclerConfig.recycleInterval = (ulong)((long)Math.Max(0, instance.Recycler.RecycleInterval));
				UMRecyclerConfig.heartBeatInterval = instance.Recycler.HeartBeatInterval;
				UMRecyclerConfig.maxHeartBeatFailures = instance.Recycler.MaxHeartBeatFailures;
				UMRecyclerConfig.resourceMonitorInterval = instance.Recycler.ResourceMonitorInterval;
				UMRecyclerConfig.thrashCountMaximum = instance.Recycler.ThrashCountMaximum;
				UMRecyclerConfig.startupTime = instance.Recycler.StartupTime;
				UMRecyclerConfig.maxCallsBeforeRecycle = instance.Recycler.MaxCallsBeforeRecycle;
				UMRecyclerConfig.heartBeatResponseTime = instance.Recycler.HeartBeatResponseTime;
				UMRecyclerConfig.pingInterval = instance.Recycler.PingInterval;
				UMRecyclerConfig.subsequentAlertIntervalAfterFirstAlert = instance.Recycler.SubsequentAlertIntervalAfterFirstAlertForCert;
				UMRecyclerConfig.daysBeforeCertExpiryForAlert = instance.Recycler.DaysBeforeCertExpiryForAlert;
				UMRecyclerConfig.certFileName = Path.Combine(Utils.GetExchangeDirectory(), instance.Recycler.CertFileName);
				UMRecyclerConfig.alertIntervalAfterStartupModeChanged = instance.Recycler.AlertIntervalAfterStartupModeChanged;
				UMRecyclerConfig.useDataCenterActiveManagerRouting = instance.Recycler.UseDataCenterActiveManagerRouting;
				UMRecyclerConfig.DetermineUMStartupMode(adSettings);
				UMRecyclerConfig.tempFilePath = Path.Combine(Utils.GetExchangeDirectory(), "UnifiedMessaging\\temp\\UMTempFiles");
				UMRecyclerConfig.tempDir = "UnifiedMessaging\\temp\\UMTempFiles";
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "Failed to Initialize ; error = {0}", new object[]
				{
					ex.ToString()
				});
				throw new UnableToInitializeResourceException("MSExchangeUM.config", ex);
			}
		}

		private static void DetermineUMStartupMode(UMADSettings adSettings)
		{
			UMRecyclerConfig.tcpPort = adSettings.SipTcpListeningPort;
			UMRecyclerConfig.tlsPort = adSettings.SipTlsListeningPort;
			UMRecyclerConfig.umStartupType = adSettings.UMStartupMode;
			UMRecyclerConfig.worker2SipPortNumber = UMRecyclerConfig.worker1SipPortNumber + 2;
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "UMRecyclerConfig.DetermineUMStartupMode: TcpListeningPort = {0} TlsListeningPort = {1} StartupMode = {2}", new object[]
			{
				UMRecyclerConfig.tcpPort,
				UMRecyclerConfig.tlsPort,
				UMRecyclerConfig.umStartupType
			});
		}

		private static double maxPrivateBytesPercent;

		private static ulong maxTempDirSize;

		private static ulong recycleInterval;

		private static int heartBeatInterval;

		private static int maxHeartBeatFailures;

		private static int worker1SipPortNumber;

		private static int worker2SipPortNumber;

		private static int resourceMonitorInterval;

		private static int thrashCountMaximum;

		private static int startupTime;

		private static int retireTime;

		private static int pingInterval;

		private static int alertIntervalAfterStartupModeChanged;

		private static int maxCallsBeforeRecycle;

		private static int heartBeatResponseTime;

		private static string tempFilePath;

		private static string certFileName;

		private static string tempDir;

		private static int tcpPort = -1;

		private static int tlsPort = -1;

		private static UMStartupMode umStartupType;

		private static int subsequentAlertIntervalAfterFirstAlert;

		private static int daysBeforeCertExpiryForAlert;

		private static bool useDataCenterActiveManagerRouting;
	}
}
