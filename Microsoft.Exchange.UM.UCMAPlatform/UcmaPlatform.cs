using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Signaling;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;
using Microsoft.Speech.Synthesis;
using Microsoft.Win32;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaPlatform : Platform
	{
		public static void ValidateRealTimeUri(string uri)
		{
			try
			{
				new RealTimeAddress(uri);
			}
			catch (ArgumentException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, null, "RealTimeUri is invalid.  {0}", new object[]
				{
					ex
				});
				throw new RealTimeException(ex.Message, ex);
			}
		}

		public override BaseUMVoipPlatform CreateVoipPlatform()
		{
			return new UcmaVoipPlatform();
		}

		public override BaseCallRouterPlatform CreateCallRouterVoipPlatform(LocalizedString serviceName, LocalizedString serverName, UMADSettings config)
		{
			return new UcmaCallRouterPlatform(serviceName, serverName, config);
		}

		public override PlatformSipUri CreateSipUri(string uri)
		{
			return new UcmaSipUri(uri);
		}

		public override bool TryCreateSipUri(string uriString, out PlatformSipUri sipUri)
		{
			return UcmaSipUri.TryParse(uriString, out sipUri);
		}

		public override PlatformSipUri CreateSipUri(SipUriScheme scheme, string user, string host)
		{
			return new UcmaSipUri(scheme, user, host);
		}

		public override PlatformSignalingHeader CreateSignalingHeader(string name, string value)
		{
			return new UcmaSignalingHeader(name, value);
		}

		public override bool TryCreateOfflineTranscriber(CultureInfo transcriptionLanguage, out BaseUMOfflineTranscriber transcriber)
		{
			return UcmaOfflineTranscriber.TryCreate(transcriptionLanguage, out transcriber);
		}

		public override bool TryCreateMobileRecognizer(Guid requestId, CultureInfo culture, SpeechRecognitionEngineType engineType, int maxAlternates, out IMobileRecognizer recognizer)
		{
			return UcmaMobileRecognizer.TryCreate(requestId, culture, engineType, maxAlternates, out recognizer);
		}

		public override bool IsTranscriptionLanguageSupported(CultureInfo transcriptionLanguage)
		{
			return UcmaInstalledRecognizers.IsTranscriptionLanguageSupported(transcriptionLanguage);
		}

		public override IEnumerable<CultureInfo> SupportedTranscriptionLanguages
		{
			get
			{
				return UcmaInstalledRecognizers.SupportedTranscriptionLanguages;
			}
		}

		public override void CompileGrammar(string grxmlGrammarPath, string compiledGrammarPath, CultureInfo culture)
		{
			using (ITempFile tempFile = TempFileFactory.CreateTempGrammarFile())
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this.GetHashCode(), "Compiling to CFG {0} --> {1}", new object[]
				{
					grxmlGrammarPath,
					tempFile.FilePath
				});
				using (Stream stream = new FileStream(tempFile.FilePath, FileMode.Create))
				{
					SrgsGrammarCompiler.Compile(grxmlGrammarPath, stream);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this.GetHashCode(), "Compiling to CFP {0} --> {1}", new object[]
				{
					tempFile.FilePath,
					compiledGrammarPath
				});
				using (GrammarInfo grammarInfo = new GrammarInfo(tempFile.FilePath))
				{
					using (SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine(UcmaInstalledRecognizers.GetRecognizerId(SpeechRecognitionEngineType.CmdAndControl, culture)))
					{
						using (Stream stream2 = new FileStream(compiledGrammarPath, FileMode.Create))
						{
							grammarInfo.ExtraParts.AddEnginePart(speechRecognitionEngine);
							grammarInfo.Save(stream2);
						}
					}
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this.GetHashCode(), "Compliation Complete!", new object[0]);
		}

		public override void CheckGrammarEntryFormat(string wordToCheck)
		{
			new SrgsText(wordToCheck);
		}

		public override ITempWavFile SynthesizePromptsToPcmWavFile(ArrayList prompts)
		{
			ITempWavFile tempWavFile = TempFileFactory.CreateTempWavFile();
			bool flag = false;
			using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
			{
				PromptBuilder promptBuilder = new PromptBuilder();
				foreach (object obj in prompts)
				{
					Prompt prompt = (Prompt)obj;
					promptBuilder.AppendSsmlMarkup(prompt.ToSsml());
				}
				try
				{
					SpeechAudioFormatInfo speechAudioFormatInfo = new SpeechAudioFormatInfo(WaveFormat.Pcm8WaveFormat.SamplesPerSec, 16, 1);
					speechSynthesizer.SetOutputToWaveFile(tempWavFile.FilePath, speechAudioFormatInfo);
					speechSynthesizer.Speak(promptBuilder);
					speechSynthesizer.SetOutputToNull();
					flag = true;
				}
				catch (Exception ex)
				{
					if (ex is IOException || ex is COMException || ex is FormatException || ex is ArgumentException || ex is InvalidOperationException)
					{
						throw new PromptSynthesisException(ex.Message, ex);
					}
					throw;
				}
				finally
				{
					if (!flag)
					{
						tempWavFile.Dispose();
						tempWavFile = null;
					}
				}
			}
			return tempWavFile;
		}

		public override void RecycleServiceDependencies()
		{
		}

		public override void InitializeG723Support()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this.GetHashCode(), "AppConfig EnableG723 = {0}", new object[]
			{
				AppConfig.Instance.Service.EnableG723
			});
			int num = AppConfig.Instance.Service.EnableG723 ? 0 : 1;
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Unified Managed Communications API\\AudioVideo", "DisableG723", 0);
			int num2 = (value is int) ? ((int)value) : 0;
			if (num != num2)
			{
				Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Unified Managed Communications API\\AudioVideo", "DisableG723", num, RegistryValueKind.DWord);
				CallIdTracer.TraceDebug(ExTraceGlobals.VoipPlatformTracer, this.GetHashCode(), "Set {0} to {1}", new object[]
				{
					"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Unified Managed Communications API\\AudioVideo@DisableG723",
					num
				});
			}
		}

		private const string G723RegistryKeyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Unified Managed Communications API\\AudioVideo";

		private const string G723RegistryValueName = "DisableG723";

		private static object staticLock = new object();
	}
}
