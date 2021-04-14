using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Audio;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class AppConfig
	{
		private AppConfig()
		{
			this.appConfiguration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
			{
				ExeConfigFilename = Path.Combine(Utils.GetExchangeDirectory(), "bin\\MSExchangeUM.config")
			}, ConfigurationUserLevel.None);
		}

		public static AppConfig Instance
		{
			get
			{
				return AppConfig.instance.Value;
			}
		}

		public AppConfig.ServiceConfig Service
		{
			get
			{
				return this.serviceConfig.Value;
			}
		}

		public AppConfig.RecyclerConfig Recycler
		{
			get
			{
				return this.recyclerConfig.Value;
			}
		}

		public AppConfig.GrammarDirConfig GrammarDirectory
		{
			get
			{
				return this.grammarDirConfig.Value;
			}
		}

		public AppConfig.WaveDirConfig WaveDirectory
		{
			get
			{
				return this.waveDirConfig.Value;
			}
		}

		private static Lazy<AppConfig> AppConfigInitializer()
		{
			return new Lazy<AppConfig>(() => new AppConfig(), true);
		}

		internal static string GetSetting(string label)
		{
			string result = null;
			KeyValueConfigurationElement keyValueConfigurationElement = AppConfig.Instance.appConfiguration.AppSettings.Settings[label];
			if (keyValueConfigurationElement != null)
			{
				result = keyValueConfigurationElement.Value;
			}
			return result;
		}

		internal static void ClearCache()
		{
			AppConfig.instance = AppConfig.AppConfigInitializer();
		}

		internal const string ConfigFileName = "MSExchangeUM.config";

		internal const int DatacenterDaysBeforeCertExpiryForAlert = 7;

		internal const int DatacenterSubsequentAlertIntervalAfterFirstAlertForCert = 1;

		private static Lazy<AppConfig> instance = AppConfig.AppConfigInitializer();

		private Configuration appConfiguration;

		private Lazy<AppConfig.ServiceConfig> serviceConfig = new Lazy<AppConfig.ServiceConfig>(() => AppConfig.ServiceConfig.Load(), true);

		private Lazy<AppConfig.RecyclerConfig> recyclerConfig = new Lazy<AppConfig.RecyclerConfig>(() => AppConfig.RecyclerConfig.Load(), true);

		private Lazy<AppConfig.GrammarDirConfig> grammarDirConfig = new Lazy<AppConfig.GrammarDirConfig>(() => AppConfig.GrammarDirConfig.Load(), true);

		private Lazy<AppConfig.WaveDirConfig> waveDirConfig = new Lazy<AppConfig.WaveDirConfig>(() => AppConfig.WaveDirConfig.Load(AppConfig.Instance.Service), true);

		internal class ServiceConfig
		{
			private ServiceConfig()
			{
			}

			public bool EnableTemporaryTTS { get; private set; }

			public bool EnableTranscriptionWhitespace { get; private set; }

			public TimeSpan MessageTranscriptionTimeout { get; private set; }

			public TimeSpan TranscriptionMaximumMessageLength { get; private set; }

			public TimeSpan TranscriptionMaximumBacklogPerCore { get; private set; }

			public TimeSpan PickupDirectoryPollingPeriod { get; private set; }

			public int MaxMessagesPerCore { get; private set; }

			public TimeSpan CallAnswerMailboxDataTimeoutThreshold { get; private set; }

			public TimeSpan CallAnswerMailboxDataTimeout { get; private set; }

			public string FiniteStateMachinePath { get; private set; }

			public string PromptDirectory { get; private set; }

			public bool EnableSpeechRecognitionOverride { get; private set; }

			public double NormalizationLevelDB { get; private set; }

			public double NoiseFloorLevelDB { get; private set; }

			public bool EnableCallerIdDisplayNameResolution { get; private set; }

			public bool GenerateWatsonsForPipelineCleanup { get; private set; }

			public int LanguageAutoDetectionMinLength { get; private set; }

			public int LanguageAutoDetectionMaxLength { get; private set; }

			public G711Format G711EncodingFormat { get; private set; }

			public int PipelineScaleFactorCPU { get; private set; }

			public int PipelineScaleFactorNetworkBound { get; private set; }

			public int MaxRPCThreadsPerServer { get; private set; }

			public int MaxMessagesPerMailboxServer { get; private set; }

			public bool EnableRemoteGatewayAutomation { get; private set; }

			public string AutomationServiceAddress { get; private set; }

			public int AutomationServiceTcpPort { get; private set; }

			public PlatformType PlatformType { get; private set; }

			public TimeSpan PipelineStallCheckThreshold { get; private set; }

			public bool EnableWatsonOnPipelineStall { get; private set; }

			public bool CDRLoggingEnabled { get; private set; }

			public int MaxCDRMessagesInPipeline { get; private set; }

			public bool EnableG723 { get; private set; }

			public bool EnableRTAudio { get; private set; }

			public int TopNGrammarThreshold { get; private set; }

			public int MinimumRtpPort { get; private set; }

			public int MaximumRtpPort { get; private set; }

			public bool CallRejectionLoggingEnabled { get; private set; }

			public bool StatisticsLoggingEnabled { get; private set; }

			public int StatisticsLoggingMaxDirectorySize { get; private set; }

			public int StatisticsLoggingMaxFileSize { get; private set; }

			public bool IntraSiteLoadBalancingEnabled { get; private set; }

			public int MaxMobileSpeechRecoRequestsPerCore { get; private set; }

			public int RecipientStartThrottlingThresholdPercent { get; private set; }

			public int RecipientThrottlingPercent { get; private set; }

			public bool SkipCertPHeaderCheckforActiveMonitoring { get; private set; }

			public static AppConfig.ServiceConfig Load()
			{
				return new AppConfig.ServiceConfig
				{
					EnableTemporaryTTS = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableTemporaryTTS"), true),
					EnableTranscriptionWhitespace = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableTranscriptionWhitespace"), false),
					MessageTranscriptionTimeout = SafeConvert.ToTimeSpan(AppConfig.GetSetting("MessageTranscriptionTimeout"), TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(600.0), TimeSpan.FromSeconds(180.0)),
					TranscriptionMaximumMessageLength = SafeConvert.ToTimeSpan(AppConfig.GetSetting("TranscriptionMaximumMessageLength"), TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(600.0), TimeSpan.FromSeconds(75.0)),
					TranscriptionMaximumBacklogPerCore = SafeConvert.ToTimeSpan(AppConfig.GetSetting("TranscriptionMaximumBacklogPerCore"), TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(3600.0), TimeSpan.FromSeconds(300.0)),
					PickupDirectoryPollingPeriod = SafeConvert.ToTimeSpan(AppConfig.GetSetting("PickupDirectoryPollingPeriod"), TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(30.0)),
					MaxMessagesPerCore = SafeConvert.ToInt32(AppConfig.GetSetting("MaxMessagesPerCore"), 1, 1024, 100),
					CallAnswerMailboxDataTimeoutThreshold = SafeConvert.ToTimeSpan(AppConfig.GetSetting("CallAnswerMailboxDataTimeoutThreshold"), TimeSpan.FromMilliseconds(250.0), TimeSpan.FromMilliseconds(10000.0), TimeSpan.FromMilliseconds(4000.0)),
					CallAnswerMailboxDataTimeout = SafeConvert.ToTimeSpan(AppConfig.GetSetting("CallAnswerMailboxDataTimeout"), TimeSpan.FromMilliseconds(500.0), TimeSpan.FromMilliseconds(20000.0), TimeSpan.FromMilliseconds(6000.0)),
					FiniteStateMachinePath = SafeConvert.ToString(AppConfig.GetSetting("FiniteStateMachinePath"), "UnifiedMessaging\\root.fsm"),
					PromptDirectory = SafeConvert.ToString(AppConfig.GetSetting("PromptDirectory"), "UnifiedMessaging\\prompts"),
					EnableSpeechRecognitionOverride = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableSpeechRecognitionOverride"), false),
					NormalizationLevelDB = SafeConvert.ToDouble(AppConfig.GetSetting("NormalizationLevelDB"), -25.0, 0.0, -18.0),
					NoiseFloorLevelDB = SafeConvert.ToDouble(AppConfig.GetSetting("NoiseFloorLevelDB"), -100.0, 0.0, -78.0),
					EnableCallerIdDisplayNameResolution = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableCallerIdDisplayNameResolution"), true),
					GenerateWatsonsForPipelineCleanup = SafeConvert.ToBoolean(AppConfig.GetSetting("GenerateWatsonsForPipelineCleanup"), false),
					LanguageAutoDetectionMinLength = SafeConvert.ToInt32(AppConfig.GetSetting("LanguageAutoDetectionMinLength"), -1, 1048576, 64),
					LanguageAutoDetectionMaxLength = SafeConvert.ToInt32(AppConfig.GetSetting("LanguageAutoDetectionMaxLength"), 255, 1048576, 2048),
					G711EncodingFormat = SafeConvert.ToEnum<G711Format>(AppConfig.GetSetting("G711EncodingFormat"), G711Format.MULAW),
					PipelineScaleFactorCPU = SafeConvert.ToInt32(AppConfig.GetSetting("PipelineScaleFactorCPU"), 1, 64, 1),
					PipelineScaleFactorNetworkBound = SafeConvert.ToInt32(AppConfig.GetSetting("PipelineScaleFactorNetworkBound"), 1, 64, 4),
					MaxRPCThreadsPerServer = SafeConvert.ToInt32(AppConfig.GetSetting("MaxRPCThreadsPerServer"), 1, 64, 4),
					MaxMessagesPerMailboxServer = SafeConvert.ToInt32(AppConfig.GetSetting("MaxMessagesPerMailboxServer"), 1, 100, 100),
					EnableRemoteGatewayAutomation = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableRemoteGatewayAutomation"), false),
					AutomationServiceAddress = SafeConvert.ToString(AppConfig.GetSetting("AutomationServiceAddress"), null),
					AutomationServiceTcpPort = SafeConvert.ToInt32(AppConfig.GetSetting("AutomationServiceTcpPort"), 0, int.MaxValue, 7001),
					PlatformType = SafeConvert.ToEnum<PlatformType>(AppConfig.GetSetting("Platform"), PlatformType.MSS),
					PipelineStallCheckThreshold = SafeConvert.ToTimeSpan(AppConfig.GetSetting("PipelineStallCheckThreshold"), TimeSpan.Zero, TimeSpan.FromDays(30.0), TimeSpan.FromMinutes(30.0)),
					EnableWatsonOnPipelineStall = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableWatsonOnPipelineStall"), true),
					CDRLoggingEnabled = SafeConvert.ToBoolean(AppConfig.GetSetting("CDRLoggingEnabled"), true),
					MaxCDRMessagesInPipeline = SafeConvert.ToInt32(AppConfig.GetSetting("MaxCDRMessagesInPipeline"), 1, 1024, 100),
					EnableG723 = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableG723"), true),
					EnableRTAudio = SafeConvert.ToBoolean(AppConfig.GetSetting("EnableRTAudio"), true),
					TopNGrammarThreshold = SafeConvert.ToInt32(AppConfig.GetSetting("TopNGrammarThreshold"), 1, 1073741823, 20),
					MinimumRtpPort = SafeConvert.ToInt32(AppConfig.GetSetting("MinimumRtpPort"), 1025, 65535, 1025),
					MaximumRtpPort = SafeConvert.ToInt32(AppConfig.GetSetting("MaximumRtpPort"), 1025, 65535, 65535),
					CallRejectionLoggingEnabled = SafeConvert.ToBoolean(AppConfig.GetSetting("CallRejectionLoggingEnabled"), true),
					StatisticsLoggingEnabled = SafeConvert.ToBoolean(AppConfig.GetSetting("StatisticsLoggingEnabled"), true),
					StatisticsLoggingMaxDirectorySize = SafeConvert.ToInt32(AppConfig.GetSetting("StatisticsLoggingMaxDirectorySize"), 1, 10, 4),
					StatisticsLoggingMaxFileSize = SafeConvert.ToInt32(AppConfig.GetSetting("StatisticsLoggingMaxFileSize"), 1, 100, 10),
					IntraSiteLoadBalancingEnabled = SafeConvert.ToBoolean(AppConfig.GetSetting("IntraSiteLoadBalancingEnabled"), true),
					MaxMobileSpeechRecoRequestsPerCore = SafeConvert.ToInt32(AppConfig.GetSetting("MaxMobileSpeechRecoRequestsPerCore"), 1, 25, 10),
					RecipientStartThrottlingThresholdPercent = SafeConvert.ToInt32(AppConfig.GetSetting("RecipientStartThrottlingThresholdPercent"), 0, 100, 50),
					RecipientThrottlingPercent = SafeConvert.ToInt32(AppConfig.GetSetting("RecipientThrottlingPercent"), 0, 100, 10),
					SkipCertPHeaderCheckforActiveMonitoring = SafeConvert.ToBoolean(AppConfig.GetSetting("SkipCertPHeaderCheckforActiveMonitoring"), true)
				};
			}
		}

		internal class GrammarDirConfig
		{
			private GrammarDirConfig()
			{
			}

			public string GrammarDir { get; private set; }

			public Dictionary<CultureInfo, string> GrammarCultureToSubDirectoryMap { get; private set; }

			public static AppConfig.GrammarDirConfig Load()
			{
				AppConfig.GrammarDirConfig grammarDirConfig = new AppConfig.GrammarDirConfig();
				grammarDirConfig.GrammarCultureToSubDirectoryMap = new Dictionary<CultureInfo, string>();
				string exchangeDirectory = Utils.GetExchangeDirectory();
				grammarDirConfig.GrammarDir = Path.Combine(exchangeDirectory, "UnifiedMessaging\\grammars");
				return grammarDirConfig;
			}
		}

		internal class WaveDirConfig
		{
			private WaveDirConfig()
			{
			}

			public string WaveDir { get; private set; }

			public Dictionary<CultureInfo, string> PromptCultureToSubDirectoryMap { get; private set; }

			public static AppConfig.WaveDirConfig Load(AppConfig.ServiceConfig serviceConfig)
			{
				ValidateArgument.NotNull(serviceConfig, "serviceConfig");
				AppConfig.WaveDirConfig waveDirConfig = new AppConfig.WaveDirConfig();
				waveDirConfig.PromptCultureToSubDirectoryMap = new Dictionary<CultureInfo, string>();
				string exchangeDirectory = Utils.GetExchangeDirectory();
				waveDirConfig.WaveDir = Path.Combine(exchangeDirectory, serviceConfig.PromptDirectory);
				return waveDirConfig;
			}
		}

		internal class RecyclerConfig
		{
			private RecyclerConfig()
			{
			}

			public int WorkerSIPPort { get; private set; }

			public int MaxPrivateBytesPercent { get; private set; }

			public int MaxTempDirSize { get; private set; }

			public int RecycleInterval { get; private set; }

			public int HeartBeatInterval { get; private set; }

			public int MaxHeartBeatFailures { get; private set; }

			public int ResourceMonitorInterval { get; private set; }

			public int ThrashCountMaximum { get; private set; }

			public int StartupTime { get; private set; }

			public int MaxCallsBeforeRecycle { get; private set; }

			public int HeartBeatResponseTime { get; private set; }

			public int PingInterval { get; private set; }

			public int AlertIntervalAfterStartupModeChanged { get; private set; }

			public bool UseDataCenterActiveManagerRouting { get; private set; }

			public int DaysBeforeCertExpiryForAlert { get; private set; }

			public int SubsequentAlertIntervalAfterFirstAlertForCert { get; private set; }

			public string CertFileName { get; private set; }

			public static AppConfig.RecyclerConfig Load()
			{
				AppConfig.RecyclerConfig recyclerConfig = new AppConfig.RecyclerConfig();
				recyclerConfig.WorkerSIPPort = SafeConvert.ToInt32(AppConfig.GetSetting("WorkerSIPPort"), 1, int.MaxValue, 5065);
				recyclerConfig.MaxPrivateBytesPercent = SafeConvert.ToInt32(AppConfig.GetSetting("MaxPrivateBytesPercent"), 0, 100, 80);
				recyclerConfig.MaxTempDirSize = SafeConvert.ToInt32(AppConfig.GetSetting("MaxTempDirSize"), 0, int.MaxValue, 0);
				recyclerConfig.RecycleInterval = SafeConvert.ToInt32(AppConfig.GetSetting("RecycleInterval"), 0, int.MaxValue, 604800);
				recyclerConfig.HeartBeatInterval = SafeConvert.ToInt32(AppConfig.GetSetting("HeartBeatInterval"), 0, 600, 90);
				recyclerConfig.MaxHeartBeatFailures = SafeConvert.ToInt32(AppConfig.GetSetting("MaxHeartBeatFailures"), 0, 1, 1);
				recyclerConfig.ResourceMonitorInterval = SafeConvert.ToInt32(AppConfig.GetSetting("ResourceMonitorInterval"), 0, 3600, 600);
				recyclerConfig.ThrashCountMaximum = SafeConvert.ToInt32(AppConfig.GetSetting("ThrashCountMaximum"), 0, 100, 5);
				recyclerConfig.StartupTime = SafeConvert.ToInt32(AppConfig.GetSetting("StartupTime"), 120, 1200, 240);
				recyclerConfig.MaxCallsBeforeRecycle = SafeConvert.ToInt32(AppConfig.GetSetting("MaxCallsBeforeRecycle"), 0, 1048576, 50000);
				recyclerConfig.HeartBeatResponseTime = SafeConvert.ToInt32(AppConfig.GetSetting("HeartBeatResponseTime"), 0, 120, 60);
				recyclerConfig.PingInterval = SafeConvert.ToInt32(AppConfig.GetSetting("PingInterval"), 0, 3600, 120);
				recyclerConfig.AlertIntervalAfterStartupModeChanged = SafeConvert.ToInt32(AppConfig.GetSetting("AlertIntervalAfterStartupModeChanged"), 0, int.MaxValue, 600);
				if (CommonConstants.UseDataCenterCallRouting)
				{
					recyclerConfig.DaysBeforeCertExpiryForAlert = 7;
					recyclerConfig.SubsequentAlertIntervalAfterFirstAlertForCert = 1;
				}
				else
				{
					recyclerConfig.DaysBeforeCertExpiryForAlert = SafeConvert.ToInt32(AppConfig.GetSetting("DaysBeforeCertExpiryForAlert"), 1, 30, 30);
					recyclerConfig.SubsequentAlertIntervalAfterFirstAlertForCert = SafeConvert.ToInt32(AppConfig.GetSetting("SubsequentAlertIntervalAfterFirstAlertForCert"), 1, 30, 1);
				}
				recyclerConfig.CertFileName = SafeConvert.ToString(AppConfig.GetSetting("CertFileName"), "UnifiedMessaging\\UMServiceCertificate.cer");
				if (CommonConstants.UseDataCenterCallRouting)
				{
					recyclerConfig.UseDataCenterActiveManagerRouting = SafeConvert.ToBoolean(AppConfig.GetSetting("UseDataCenterActiveManagerRouting"), false);
				}
				return recyclerConfig;
			}
		}
	}
}
