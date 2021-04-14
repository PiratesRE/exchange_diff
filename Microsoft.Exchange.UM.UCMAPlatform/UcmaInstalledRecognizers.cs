using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal static class UcmaInstalledRecognizers
	{
		public static bool TryGetRecognizerId(SpeechRecognitionEngineType engineType, CultureInfo culture, out string recognizerId)
		{
			ValidateArgument.NotNull(culture, "culture");
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, 0, "Entering UcmaInstalledRecognizers.TryGetRecognizerId - Engine type='{0}', Culture='{1}'", new object[]
			{
				engineType,
				culture
			});
			UcmaInstalledRecognizers.InitializeSupportedLanguages();
			recognizerId = null;
			switch (engineType)
			{
			case SpeechRecognitionEngineType.CmdAndControl:
				if (UcmaInstalledRecognizers.lazyCmdAndControlLanguages.ContainsKey(culture))
				{
					recognizerId = UcmaInstalledRecognizers.lazyCmdAndControlLanguages[culture];
				}
				break;
			case SpeechRecognitionEngineType.Transcription:
				if (UcmaInstalledRecognizers.lazyTranscriptionLanguages.ContainsKey(culture))
				{
					recognizerId = UcmaInstalledRecognizers.lazyTranscriptionLanguages[culture];
				}
				break;
			default:
				ExAssert.RetailAssert(false, "Invalid speech recognition engine type - '{0}'", new object[]
				{
					engineType
				});
				break;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, 0, "UcmaInstalledRecognizers.TryGetRecognizerId - Recognizer id ='{0}'", new object[]
			{
				(recognizerId == null) ? "<null>" : recognizerId
			});
			return recognizerId != null;
		}

		public static string GetRecognizerId(SpeechRecognitionEngineType engineType, CultureInfo culture)
		{
			ValidateArgument.NotNull(culture, "culture");
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, 0, "Entering UcmaInstalledRecognizers.GetRecognizerId - Engine type='{0}', Culture='{1}'", new object[]
			{
				engineType,
				culture
			});
			string text = null;
			bool condition = UcmaInstalledRecognizers.TryGetRecognizerId(engineType, culture, out text);
			ExAssert.RetailAssert(condition, "Didn't find engine of engine type '{0}' and culture '{1}'", new object[]
			{
				engineType,
				culture
			});
			ExAssert.RetailAssert(!string.IsNullOrEmpty(text), "recognizerId = '{0}'", new object[]
			{
				text ?? "<null>"
			});
			return text;
		}

		public static bool IsTranscriptionLanguageSupported(CultureInfo culture)
		{
			ValidateArgument.NotNull(culture, "culture");
			UcmaInstalledRecognizers.InitializeSupportedLanguages();
			return UcmaInstalledRecognizers.lazyTranscriptionLanguages.ContainsKey(culture);
		}

		public static IEnumerable<CultureInfo> SupportedTranscriptionLanguages
		{
			get
			{
				UcmaInstalledRecognizers.InitializeSupportedLanguages();
				return UcmaInstalledRecognizers.lazyTranscriptionLanguages.Keys;
			}
		}

		private static void InitializeSupportedLanguages()
		{
			if (UcmaInstalledRecognizers.lazyCmdAndControlLanguages == null)
			{
				lock (UcmaInstalledRecognizers.staticLock)
				{
					if (UcmaInstalledRecognizers.lazyCmdAndControlLanguages == null)
					{
						ICollection<RecognizerInfo> collection = SpeechRecognitionEngine.InstalledRecognizers();
						UcmaInstalledRecognizers.lazyCmdAndControlLanguages = new Dictionary<CultureInfo, string>(collection.Count);
						UcmaInstalledRecognizers.lazyTranscriptionLanguages = new Dictionary<CultureInfo, string>(collection.Count);
						foreach (RecognizerInfo recognizerInfo in collection)
						{
							if (recognizerInfo.AdditionalInfo.Keys.Contains("Telephony"))
							{
								if (recognizerInfo.AdditionalInfo.Keys.Contains("Transcription"))
								{
									CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, 0, "Detected TRANS engine for culture '{0}'", new object[]
									{
										recognizerInfo.Culture
									});
									UcmaInstalledRecognizers.lazyTranscriptionLanguages.Add(recognizerInfo.Culture, recognizerInfo.Id);
								}
								else
								{
									CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, 0, "Detected TELE engine for culture '{0}'", new object[]
									{
										recognizerInfo.Culture
									});
									UcmaInstalledRecognizers.lazyCmdAndControlLanguages.Add(recognizerInfo.Culture, recognizerInfo.Id);
								}
							}
						}
					}
				}
			}
		}

		private static object staticLock = new object();

		private static Dictionary<CultureInfo, string> lazyCmdAndControlLanguages;

		private static Dictionary<CultureInfo, string> lazyTranscriptionLanguages;
	}
}
