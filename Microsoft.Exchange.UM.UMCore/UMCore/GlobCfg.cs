using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class GlobCfg
	{
		private GlobCfg()
		{
		}

		internal static string ConfigFile { get; private set; }

		internal static bool EnableCallerIdDisplayNameResolution { get; private set; }

		internal static bool GenerateWatsonsForPipelineCleanup { get; private set; }

		internal static string ExchangeDirectory { get; private set; }

		internal static bool AllowTemporaryTTS { get; set; }

		internal static List<CultureInfo> VuiCultures { get; private set; }

		internal static bool AsrOverride { get; set; }

		internal static int MaxMobileSpeechRecoRequestsPerCore { get; set; }

		internal static double NormalizationLevel { get; private set; }

		internal static double NoiseFloorLevel { get; private set; }

		internal static TimeSpan VoiceMessagePollingTime { get; private set; }

		internal static int MaxNonCDRMessagesPendingInPipeline { get; private set; }

		internal static TimeSpan CallAnswerMailboxDataDownloadTimeout { get; private set; }

		internal static TimeSpan CallAnswerMailboxDataDownloadTimeoutThreshold { get; private set; }

		internal static bool EnableRemoteGWAutomation { get; private set; }

		internal static IPAddress UMAutomationServerAddress { get; private set; }

		internal static int UMAutomationServerTCPPort { get; private set; }

		internal static int LanguageAutoDetectionMinLength { get; private set; }

		internal static int LanguageAutoDetectionMaxLength { get; private set; }

		internal static G711Format G711Format { get; private set; }

		internal static TimeSpan MessageTranscriptionTimeout { get; private set; }

		internal static TimeSpan TranscriptionMaximumMessageLength { get; private set; }

		internal static TimeSpan TranscriptionMaximumBacklogPerCore { get; private set; }

		internal static GlobCfg.DefaultPromptHelper DefaultPrompts { get; private set; }

		internal static GlobCfg.DefaultPromptForAAHelper DefaultPromptsForAA { get; private set; }

		internal static GlobCfg.DefaultPromptForPreviewHelper DefaultPromptsForPreview { get; private set; }

		internal static GlobCfg.DefaultGrammarHelper DefaultGrammars { get; private set; }

		internal static string ProductVersion { get; private set; }

		internal static void Init()
		{
			lock (GlobCfg.lockObj)
			{
				try
				{
					GlobCfg.InternalInit();
				}
				catch (Exception ex)
				{
					if (!GrayException.IsGrayException(ex))
					{
						throw;
					}
					throw new ConfigurationException(ex.Message);
				}
			}
		}

		internal static ReplyForwardType SubjectToReplyForwardType(string subject)
		{
			if (string.IsNullOrEmpty(subject))
			{
				return ReplyForwardType.None;
			}
			int num = Math.Min(subject.Length, 4);
			int num2 = -1;
			string key = string.Empty;
			for (int i = 0; i < num; i++)
			{
				if (subject[i] == ':')
				{
					if (i + 1 < subject.Length && subject[i + 1] == ' ')
					{
						num2 = i + 1;
						break;
					}
				}
				else if (!char.IsLetter(subject[i]))
				{
					break;
				}
			}
			if (num2 > 0)
			{
				key = subject.Substring(0, num2 + 1).ToLowerInvariant().Trim();
			}
			ReplyForwardType result = ReplyForwardType.None;
			if (GlobCfg.replyForwardPrefixMap.TryGetValue(key, out result))
			{
				return result;
			}
			return ReplyForwardType.None;
		}

		private static void InternalInit()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "Initializing global configuration", new object[0]);
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "GlobCfg.BuildReplyForwardPrefixMap()", new object[0]);
			GlobCfg.BuildReplyForwardPrefixMap();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "GlobCfg.BuildAsrCultureList()", new object[0]);
			GlobCfg.BuildAsrCultureList();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "GlobCfg.ReadAllParametersForWorkerProcess()", new object[0]);
			GlobCfg.ReadAllParametersForWorkerProcess();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "GlobCfg.EnsureMailboxTimeouts()", new object[0]);
			GlobCfg.EnsureMailboxTimeouts();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "GlobCfg.RemoveInvalidPromptCultures()", new object[0]);
			GlobCfg.RemoveInvalidPromptCultures();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "DefaultPromptHelper.Create()", new object[0]);
			GlobCfg.DefaultPrompts = GlobCfg.DefaultPromptHelper.Create();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "DefaultPromptForAAHelper.Create()", new object[0]);
			GlobCfg.DefaultPromptsForAA = GlobCfg.DefaultPromptForAAHelper.Create();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "DefaultPromptForPreviewHelper.Create()", new object[0]);
			GlobCfg.DefaultPromptsForPreview = GlobCfg.DefaultPromptForPreviewHelper.Create();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "DefaultGrammarHelper.Create()", new object[0]);
			GlobCfg.DefaultGrammars = GlobCfg.DefaultGrammarHelper.Create();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "GlobCfg.InitializeG723()", new object[0]);
			GlobCfg.InitializeG723();
			CallIdTracer.TraceDebug(ExTraceGlobals.ServiceStartTracer, 0, "Initialize global configuration done.", new object[0]);
		}

		private static void ReadAllParametersForWorkerProcess()
		{
			AppConfig instance = AppConfig.Instance;
			GlobCfg.ExchangeDirectory = Utils.GetExchangeDirectory();
			GlobCfg.ConfigFile = Path.Combine(GlobCfg.ExchangeDirectory, instance.Service.FiniteStateMachinePath);
			GlobCfg.AllowTemporaryTTS = instance.Service.EnableTemporaryTTS;
			GlobCfg.AsrOverride = instance.Service.EnableSpeechRecognitionOverride;
			GlobCfg.VoiceMessagePollingTime = instance.Service.PickupDirectoryPollingPeriod;
			GlobCfg.EnableRemoteGWAutomation = instance.Service.EnableRemoteGatewayAutomation;
			GlobCfg.UMAutomationServerTCPPort = instance.Service.AutomationServiceTcpPort;
			GlobCfg.MessageTranscriptionTimeout = instance.Service.MessageTranscriptionTimeout;
			GlobCfg.TranscriptionMaximumMessageLength = instance.Service.TranscriptionMaximumMessageLength;
			GlobCfg.TranscriptionMaximumBacklogPerCore = instance.Service.TranscriptionMaximumBacklogPerCore;
			GlobCfg.EnableCallerIdDisplayNameResolution = instance.Service.EnableCallerIdDisplayNameResolution;
			GlobCfg.GenerateWatsonsForPipelineCleanup = instance.Service.GenerateWatsonsForPipelineCleanup;
			GlobCfg.LanguageAutoDetectionMinLength = instance.Service.LanguageAutoDetectionMinLength;
			GlobCfg.LanguageAutoDetectionMaxLength = instance.Service.LanguageAutoDetectionMaxLength;
			GlobCfg.G711Format = instance.Service.G711EncodingFormat;
			GlobCfg.CallAnswerMailboxDataDownloadTimeout = instance.Service.CallAnswerMailboxDataTimeout;
			GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold = instance.Service.CallAnswerMailboxDataTimeoutThreshold;
			GlobCfg.MaxMobileSpeechRecoRequestsPerCore = instance.Service.MaxMobileSpeechRecoRequestsPerCore;
			GlobCfg.NoiseFloorLevel = AudioNormalizer.ConvertDbToEnergyRms(instance.Service.NoiseFloorLevelDB);
			GlobCfg.NormalizationLevel = AudioNormalizer.ConvertDbToEnergyRms(instance.Service.NormalizationLevelDB);
			GlobCfg.UMAutomationServerAddress = ((instance.Service.AutomationServiceAddress == null) ? Utils.GetLocalIPAddress() : IPAddress.Parse(instance.Service.AutomationServiceAddress));
			GlobCfg.ProductVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
			GlobCfg.MaxNonCDRMessagesPendingInPipeline = instance.Service.MaxMessagesPerMailboxServer * 3 + instance.Service.MaxMessagesPerCore * Environment.ProcessorCount;
		}

		private static void RemoveInvalidPromptCultures()
		{
			List<CultureInfo> list = new List<CultureInfo>();
			foreach (CultureInfo cultureInfo in UmCultures.GetSupportedPromptCultures())
			{
				try
				{
					Util.WavPathFromCulture(cultureInfo);
				}
				catch (ResourceDirectoryNotFoundException ex)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_LangPackDirectoryNotFound, null, new object[]
					{
						cultureInfo,
						ex.Message
					});
					list.Add(cultureInfo);
				}
			}
			foreach (CultureInfo culture in list)
			{
				UmCultures.InvalidatePromptCulture(culture);
			}
		}

		private static void BuildReplyForwardPrefixMap()
		{
			GlobCfg.replyForwardPrefixMap = new Dictionary<string, ReplyForwardType>();
			foreach (CultureInfo cultureInfo in UmCultures.GetSupportedClientCultures())
			{
				string text = ClientStrings.ItemForward.ToString(cultureInfo).ToLower(cultureInfo).Trim();
				string text2 = ClientStrings.ItemReply.ToString(cultureInfo).ToLower(cultureInfo).Trim();
				if (!string.Equals(text, text2, StringComparison.OrdinalIgnoreCase))
				{
					if (!GlobCfg.replyForwardPrefixMap.ContainsKey(text2))
					{
						GlobCfg.replyForwardPrefixMap.Add(text2, ReplyForwardType.Reply);
					}
					if (!GlobCfg.replyForwardPrefixMap.ContainsKey(text))
					{
						GlobCfg.replyForwardPrefixMap.Add(text, ReplyForwardType.Forward);
					}
				}
			}
		}

		private static void BuildAsrCultureList()
		{
			GlobCfg.VuiCultures = UmCultures.GetSupportedPromptCultures();
		}

		private static void EnsureMailboxTimeouts()
		{
			if (GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold >= GlobCfg.CallAnswerMailboxDataDownloadTimeout)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.ServiceStartTracer, 0, "Added callAnswerMailboxDataDownloadTimeoutThreshold={0} should be less than  callAnswerMailboxDataDownloadTimeout='{1}'. Will use defaults", new object[]
				{
					GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold,
					GlobCfg.CallAnswerMailboxDataDownloadTimeout
				});
				GlobCfg.CallAnswerMailboxDataDownloadTimeout = TimeSpan.FromMilliseconds(6000.0);
				GlobCfg.CallAnswerMailboxDataDownloadTimeoutThreshold = TimeSpan.FromMilliseconds(4000.0);
			}
		}

		private static void InitializeG723()
		{
			Platform.Utilities.InitializeG723Support();
		}

		private const int MaxMailboxServerFailuresToTolerate = 3;

		private static object lockObj = new object();

		private static Dictionary<CultureInfo, string> promptCultureToSubDirectoryMap = new Dictionary<CultureInfo, string>();

		private static Dictionary<CultureInfo, string> grammarCultureToSubDirectoryMap = new Dictionary<CultureInfo, string>();

		private static Dictionary<string, ReplyForwardType> replyForwardPrefixMap = new Dictionary<string, ReplyForwardType>();

		internal class TranscriptionSettings
		{
			internal float HighConfidence { get; set; }

			internal float LowConfidence { get; set; }
		}

		internal class DefaultPromptHelper
		{
			private DefaultPromptHelper()
			{
			}

			internal PromptConfigBase Mumble1 { get; private set; }

			internal PromptConfigBase Mumble2 { get; private set; }

			internal PromptConfigBase Silence1 { get; private set; }

			internal PromptConfigBase Silence2 { get; private set; }

			internal PromptConfigBase SpeechError { get; private set; }

			internal PromptConfigBase InvalidCommand { get; private set; }

			internal PromptConfigBase DtmfFallback { get; private set; }

			internal PromptConfigBase SorryTryAgainLater { get; private set; }

			internal PromptConfigBase SorrySystemError { get; private set; }

			internal PromptConfigBase InvalidKey { get; private set; }

			internal PromptConfigBase NotImplemented { get; private set; }

			internal PromptConfigBase Repeat { get; private set; }

			internal PromptConfigBase TimeRange { get; private set; }

			internal PromptConfigBase AreYouThere { get; private set; }

			internal PromptConfigBase GoodBye { get; private set; }

			internal PromptConfigBase GoodByeConfirmation { get; private set; }

			internal PromptConfigBase SilenceOneSecond { get; private set; }

			internal PromptConfigBase[] MaxCallSecondsExceeded { get; private set; }

			internal PromptConfigBase[] ComfortNoise { get; private set; }

			internal PromptConfigBase VoicemailGreeting { get; private set; }

			internal PromptConfigBase AwayGreeting { get; private set; }

			internal static ArrayList BuildWithValues(CultureInfo culture, GlobCfg.DefaultPromptHelper.AddPromptWithValue f, params PromptConfigBase[] configs)
			{
				ArrayList arrayList = new ArrayList();
				foreach (PromptConfigBase promptConfigBase in configs)
				{
					if (!f(arrayList, promptConfigBase, culture))
					{
						promptConfigBase.AddPrompts(arrayList, null, culture);
					}
				}
				return arrayList;
			}

			internal static ArrayList Build(ActivityManager m, CultureInfo culture, params PromptConfigBase[] configs)
			{
				ArrayList arrayList = new ArrayList();
				foreach (PromptConfigBase promptConfigBase in configs)
				{
					promptConfigBase.AddPrompts(arrayList, m, culture);
				}
				return arrayList;
			}

			internal static GlobCfg.DefaultPromptHelper Create()
			{
				GlobCfg.DefaultPromptHelper defaultPromptHelper = new GlobCfg.DefaultPromptHelper();
				defaultPromptHelper.Mumble1 = PromptConfigBase.Create("vuiDefaultMumble1", "statement", "NOT repeat");
				defaultPromptHelper.Mumble2 = PromptConfigBase.Create("vuiDefaultMumble2", "statement", "NOT repeat");
				defaultPromptHelper.Silence1 = PromptConfigBase.Create("vuiDefaultSilence1", "statement", "NOT repeat");
				defaultPromptHelper.Silence2 = PromptConfigBase.Create("vuiDefaultSilence2", "statement", "NOT repeat");
				defaultPromptHelper.SpeechError = PromptConfigBase.Create("vuiDefaultSpeechError", "statement", "NOT repeat");
				defaultPromptHelper.InvalidCommand = PromptConfigBase.Create("vuiDefaultInvalidCommand", "statement", "NOT repeat");
				defaultPromptHelper.DtmfFallback = PromptConfigBase.Create("vuiDtmfFallback", "statement", "NOT repeat");
				defaultPromptHelper.SorryTryAgainLater = PromptConfigBase.Create("tuiSorryTryAgainLater", "statement", string.Empty);
				defaultPromptHelper.SorrySystemError = PromptConfigBase.Create("tuiSystemError", "statement", string.Empty);
				defaultPromptHelper.NotImplemented = PromptConfigBase.Create("tuiFeatureNotAvailable", "statement", string.Empty);
				defaultPromptHelper.Repeat = PromptConfigBase.Create("vuiRepeating", "statement", string.Empty);
				defaultPromptHelper.AreYouThere = PromptConfigBase.Create("tuiAreYouThere", "statement", string.Empty);
				defaultPromptHelper.GoodBye = PromptConfigBase.Create("vuiGlobalGoodbye", "statement", string.Empty);
				defaultPromptHelper.GoodByeConfirmation = PromptConfigBase.Create("vuiGoodbyeConfirmation", "statement", string.Empty);
				defaultPromptHelper.SilenceOneSecond = PromptConfigBase.Create("1s", "silence", string.Empty);
				defaultPromptHelper.MaxCallSecondsExceeded = new PromptConfigBase[]
				{
					PromptConfigBase.Create("tuiMaxCallDurationMet", "statement", string.Empty),
					PromptConfigBase.Create("tuiGoodbye", "statement", string.Empty)
				};
				VariablePromptConfig<DigitPrompt, string> variablePromptConfig = new VariablePromptConfig<DigitPrompt, string>("lastInput", "digit", string.Empty, null);
				defaultPromptHelper.InvalidKey = PromptConfigBase.Create("tuiInvalidKeys", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig
				});
				VariablePromptConfig<TimePrompt, ExDateTime> variablePromptConfig2 = new VariablePromptConfig<TimePrompt, ExDateTime>("startTime", "time", string.Empty, null);
				VariablePromptConfig<TimePrompt, ExDateTime> variablePromptConfig3 = new VariablePromptConfig<TimePrompt, ExDateTime>("endTime", "time", string.Empty, null);
				defaultPromptHelper.TimeRange = PromptConfigBase.Create("tuiDefaultTimeRange", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig2,
					variablePromptConfig3
				});
				defaultPromptHelper.ComfortNoise = new PromptConfigBase[]
				{
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("comfortnoise.wav", "wave", string.Empty),
					PromptConfigBase.Create("tuiHBODelay", "statement", string.Empty),
					PromptConfigBase.Create("tuiSystemError", "statement", string.Empty)
				};
				PromptConfigBase promptConfigBase = PromptConfigBase.Create("userName", "name", string.Empty);
				defaultPromptHelper.VoicemailGreeting = PromptConfigBase.Create("tuiSystemGreeting", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase
				});
				defaultPromptHelper.AwayGreeting = PromptConfigBase.Create("tuiSystemOofGreeting", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase
				});
				return defaultPromptHelper;
			}

			internal delegate bool AddPromptWithValue(ArrayList prompts, PromptConfigBase pConfig, CultureInfo c);
		}

		internal class DefaultPromptForAAHelper
		{
			private DefaultPromptForAAHelper()
			{
			}

			internal PromptConfigBase DayRange { get; private set; }

			internal PromptConfigBase DayTimeRange { get; private set; }

			internal PromptConfigBase Everyday { get; private set; }

			internal PromptConfigBase OpeningHours { get; private set; }

			internal PromptConfigBase OpeningHoursStandard { get; private set; }

			internal PromptConfigBase WeAreAlwaysClosed { get; private set; }

			internal PromptConfigBase WeAreAlwaysOpen { get; private set; }

			internal PromptConfigBase BusinessAddressNotSet { get; private set; }

			internal PromptConfigBase AABusinessHoursWelcome { get; private set; }

			internal PromptConfigBase AAAfterHoursWelcome { get; private set; }

			internal PromptConfigBase PleaseChooseFrom { get; private set; }

			internal PromptConfigBase PleaseSayTheName { get; private set; }

			internal PromptConfigBase BusinessAddress { get; private set; }

			internal PromptConfigBase BusinessHours { get; private set; }

			internal PromptConfigBase AAWelcomeWithBusinessName { get; private set; }

			internal PromptConfigBase CallSomeone { get; private set; }

			internal PromptConfigBase CustomMenu { get; private set; }

			internal PromptConfigBase[] CustomMenuConfig { get; private set; }

			internal static ArrayList BuildWithValues(CultureInfo culture, GlobCfg.DefaultPromptForAAHelper.AddPromptWithValue f, params PromptConfigBase[] configs)
			{
				ArrayList arrayList = new ArrayList();
				foreach (PromptConfigBase promptConfigBase in configs)
				{
					if (!f(arrayList, promptConfigBase, culture))
					{
						promptConfigBase.AddPrompts(arrayList, null, culture);
					}
				}
				return arrayList;
			}

			internal static ArrayList Build(ActivityManager m, CultureInfo culture, params PromptConfigBase[] configs)
			{
				ArrayList arrayList = new ArrayList();
				foreach (PromptConfigBase promptConfigBase in configs)
				{
					promptConfigBase.AddPrompts(arrayList, m, culture);
				}
				return arrayList;
			}

			internal static GlobCfg.DefaultPromptForAAHelper Create()
			{
				GlobCfg.DefaultPromptForAAHelper defaultPromptForAAHelper = new GlobCfg.DefaultPromptForAAHelper();
				PromptConfigBase promptConfigBase = PromptConfigBase.Create("businessName", "name", string.Empty);
				defaultPromptForAAHelper.AAWelcomeWithBusinessName = PromptConfigBase.Create("tuiAABusinessNameWelcome", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase
				});
				defaultPromptForAAHelper.AABusinessHoursWelcome = PromptConfigBase.Create("tuiAABusinessHoursWelcome", "statement", string.Empty);
				defaultPromptForAAHelper.AAAfterHoursWelcome = PromptConfigBase.Create("tuiAAAfterHoursWelcome", "statement", string.Empty);
				defaultPromptForAAHelper.PleaseChooseFrom = PromptConfigBase.Create("vuiAADsearch_No_Custom_Yes_main", "statement", string.Empty);
				defaultPromptForAAHelper.PleaseSayTheName = PromptConfigBase.Create("vuiAA_Custom_Yes_Dsearch_Yes_main", "statement", string.Empty);
				defaultPromptForAAHelper.CustomMenu = PromptConfigBase.Create("AAContext", "aaCustomMenu", string.Empty);
				defaultPromptForAAHelper.CallSomeone = PromptConfigBase.Create("tuiCallSomeone", "statement", string.Empty);
				defaultPromptForAAHelper.WeAreAlwaysClosed = PromptConfigBase.Create("tuiWeAreClosed", "statement", string.Empty);
				defaultPromptForAAHelper.WeAreAlwaysOpen = PromptConfigBase.Create("tuiWeAreOpen", "statement", string.Empty);
				PromptConfigBase promptConfigBase2 = PromptConfigBase.Create("varTimeZone", "timeZone", string.Empty);
				PromptConfigBase promptConfigBase3 = PromptConfigBase.Create("varScheduleGroupList", "scheduleGroupList", string.Empty);
				defaultPromptForAAHelper.Everyday = PromptConfigBase.Create("tuiEveryday", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase3,
					promptConfigBase2
				});
				defaultPromptForAAHelper.OpeningHours = PromptConfigBase.Create("tuiOurOpeningHoursAre", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase3,
					promptConfigBase2
				});
				PromptConfigBase promptConfigBase4 = PromptConfigBase.Create("varScheduleIntervalList", "scheduleIntervalList", string.Empty);
				defaultPromptForAAHelper.OpeningHoursStandard = PromptConfigBase.Create("tuiOurOpeningHoursAre", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase4,
					promptConfigBase2
				});
				PromptConfigBase promptConfigBase5 = PromptConfigBase.Create("businessAddress", "address", string.Empty);
				defaultPromptForAAHelper.BusinessAddress = PromptConfigBase.Create("tuiWeAreLocatedAt", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase5
				});
				PromptConfigBase promptConfigBase6 = PromptConfigBase.Create("selectedMenu", "text", string.Empty);
				defaultPromptForAAHelper.BusinessAddressNotSet = PromptConfigBase.Create("tuiLocationNotSet", "statement", string.Empty, new PromptConfigBase[]
				{
					promptConfigBase6
				});
				VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext> variablePromptConfig = new VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext>("startDay", "dayOfWeekTime", string.Empty, null);
				VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext> variablePromptConfig2 = new VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext>("endDay", "dayOfWeekTime", string.Empty, null);
				defaultPromptForAAHelper.DayRange = PromptConfigBase.Create("tuiDefaultDayRange", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig,
					variablePromptConfig2
				});
				VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext> variablePromptConfig3 = new VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext>("startDayTime", "dayOfWeekTime", string.Empty, null);
				VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext> variablePromptConfig4 = new VariablePromptConfig<DayOfWeekTimePrompt, DayOfWeekTimeContext>("endDayTime", "dayOfWeekTime", string.Empty, null);
				defaultPromptForAAHelper.DayTimeRange = PromptConfigBase.Create("tuiDefaultTimeRange", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig3,
					variablePromptConfig4
				});
				VariablePromptConfig<TextPrompt, string> variablePromptConfig5 = new VariablePromptConfig<TextPrompt, string>("departmentName", "text", string.Empty, null);
				defaultPromptForAAHelper.CustomMenuConfig = new PromptConfigBase[12];
				defaultPromptForAAHelper.CustomMenuConfig[1] = PromptConfigBase.Create("tuiPAATransferToPhone1", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[2] = PromptConfigBase.Create("tuiPAATransferToPhone2", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[3] = PromptConfigBase.Create("tuiPAATransferToPhone3", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[4] = PromptConfigBase.Create("tuiPAATransferToPhone4", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[5] = PromptConfigBase.Create("tuiPAATransferToPhone5", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[6] = PromptConfigBase.Create("tuiPAATransferToPhone6", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[7] = PromptConfigBase.Create("tuiPAATransferToPhone7", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[8] = PromptConfigBase.Create("tuiPAATransferToPhone8", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[9] = PromptConfigBase.Create("tuiPAATransferToPhone9", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				defaultPromptForAAHelper.CustomMenuConfig[11] = PromptConfigBase.Create("tuiAACustomizedMenuTimeOut", "statement", string.Empty, new PromptConfigBase[]
				{
					variablePromptConfig5
				});
				return defaultPromptForAAHelper;
			}

			internal delegate bool AddPromptWithValue(ArrayList prompts, PromptConfigBase pConfig, CultureInfo c);
		}

		internal class DefaultPromptForPreviewHelper
		{
			private DefaultPromptForPreviewHelper()
			{
			}

			internal PromptConfigBase AABusinessHours { get; private set; }

			internal PromptConfigBase AABusinessLocation { get; private set; }

			internal PromptConfigBase AAWelcomeGreeting { get; private set; }

			internal PromptConfigBase AACustomMenu { get; private set; }

			internal PromptConfigBase MbxVoicemailGreeting { get; private set; }

			internal PromptConfigBase MbxAwayGreeting { get; private set; }

			internal PromptConfigBase AACustomPrompt { get; private set; }

			internal PromptConfigBase MbxCustomGreeting { get; private set; }

			internal static ArrayList BuildWithValues(CultureInfo culture, GlobCfg.DefaultPromptForPreviewHelper.AddPromptWithValue f, params PromptConfigBase[] configs)
			{
				ArrayList arrayList = new ArrayList();
				foreach (PromptConfigBase promptConfigBase in configs)
				{
					if (!f(arrayList, promptConfigBase, culture))
					{
						promptConfigBase.AddPrompts(arrayList, null, culture);
					}
				}
				return arrayList;
			}

			internal static ArrayList Build(ActivityManager m, CultureInfo culture, params PromptConfigBase[] configs)
			{
				ArrayList arrayList = new ArrayList();
				foreach (PromptConfigBase promptConfigBase in configs)
				{
					promptConfigBase.AddPrompts(arrayList, m, culture);
				}
				return arrayList;
			}

			internal static GlobCfg.DefaultPromptForPreviewHelper Create()
			{
				return new GlobCfg.DefaultPromptForPreviewHelper
				{
					AABusinessHours = PromptConfigBase.Create("businessSchedule", "businessHours", string.Empty),
					AABusinessLocation = PromptConfigBase.Create("aaLocationContext", "aaBusinessLocation", string.Empty),
					AAWelcomeGreeting = PromptConfigBase.Create("AAContext", "aaWelcomeGreeting", string.Empty),
					AACustomMenu = PromptConfigBase.Create("AAContext", "aaCustomMenu", string.Empty),
					AACustomPrompt = PromptConfigBase.Create("customPrompt", "varwave", string.Empty),
					MbxVoicemailGreeting = PromptConfigBase.Create("userName", "mbxVoicemailGreeting", string.Empty),
					MbxAwayGreeting = PromptConfigBase.Create("userName", "mbxAwayGreeting", string.Empty),
					MbxCustomGreeting = PromptConfigBase.Create("customGreeting", "tempwave", string.Empty)
				};
			}

			internal delegate bool AddPromptWithValue(ArrayList prompts, PromptConfigBase pConfig, CultureInfo c);
		}

		internal class DefaultGrammarHelper
		{
			private DefaultGrammarHelper()
			{
			}

			internal UMGrammarConfig Help { get; private set; }

			internal UMGrammarConfig Repeat { get; private set; }

			internal UMGrammarConfig Goodbye { get; private set; }

			internal UMGrammarConfig GoodbyeConfirmation { get; private set; }

			internal UMGrammarConfig MainMenuShortcut { get; private set; }

			internal static GlobCfg.DefaultGrammarHelper Create()
			{
				return new GlobCfg.DefaultGrammarHelper
				{
					Help = UMGrammarConfig.Create("common.grxml", "static", string.Empty, "help", string.Empty, null),
					Repeat = UMGrammarConfig.Create("common.grxml", "static", string.Empty, "repeat", string.Empty, null),
					Goodbye = UMGrammarConfig.Create("common.grxml", "static", string.Empty, "goodbye", string.Empty, null),
					GoodbyeConfirmation = UMGrammarConfig.Create("common.grxml", "static", string.Empty, "yesNo", string.Empty, null),
					MainMenuShortcut = UMGrammarConfig.Create("common.grxml", "static", string.Empty, "mainMenuShortcut", string.Empty, null)
				};
			}
		}
	}
}
