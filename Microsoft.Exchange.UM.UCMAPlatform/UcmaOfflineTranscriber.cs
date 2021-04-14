using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaOfflineTranscriber : BaseUMOfflineTranscriber
	{
		protected UcmaOfflineTranscriber(SpeechRecognitionEngine transcriptionEngine, CultureInfo transcriptionLanguage)
		{
			this.transcriptionEngine = transcriptionEngine;
			this.transcriptionLanguage = transcriptionLanguage;
			this.transcriptionEngine.SpeechRecognized += this.OnSpeechRecognized;
			this.transcriptionEngine.RecognizeCompleted += this.OnRecognizeCompleted;
		}

		internal override event EventHandler<BaseUMOfflineTranscriber.TranscribeCompletedEventArgs> TranscribeCompleted;

		internal static bool TryCreate(CultureInfo transcriptionLanguage, out BaseUMOfflineTranscriber transcriber)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Entering UcmaOfflineTranscriber.TryCreate", new object[0]);
			transcriber = null;
			string text = null;
			if (UcmaInstalledRecognizers.TryGetRecognizerId(SpeechRecognitionEngineType.Transcription, transcriptionLanguage, out text))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Found a transcription engine for {0}", new object[]
				{
					transcriptionLanguage
				});
				transcriber = new UcmaOfflineTranscriber(new SpeechRecognitionEngine(text), transcriptionLanguage);
			}
			else
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, 0, "Couldn't find a transcription engine for {0}", new object[]
				{
					transcriptionLanguage
				});
				transcriber = null;
			}
			return null != transcriber;
		}

		internal override void TranscribeFile(string audioFilePath)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering UcmaOfflineTranscriber.TranscribeFile", new object[0]);
			this.transcriptionEngine.SetInputToWaveFile(audioFilePath);
			this.PrepareCustomGrammars();
			string text = Path.Combine(Util.GetTranscriptionGrammarDir(this.transcriptionLanguage), "TSR-LM.cfp");
			Uri uri = new Uri("file:///" + this.customGrammarDir + Path.DirectorySeparatorChar);
			Grammar grammar = new Grammar(text, string.Empty, uri);
			this.transcriptionEngine.LoadGrammar(grammar);
			this.transcriptionEngine.UpdateRecognizerSetting("EngineThreadPriority", -1);
			this.transcriptionEngine.UpdateRecognizerSetting("AccuracyOverride", 100);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Calling RecognizeAsync on {0}", new object[]
			{
				audioFilePath
			});
			this.transcriptionEngine.RecognizeAsync(1);
		}

		internal override void CancelTranscription()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering UcmaOfflineTranscriber.CancelTranscription", new object[0]);
			this.transcriptionEngine.RecognizeAsyncCancel();
		}

		internal override string TestHook_GenerateCustomGrammars()
		{
			this.PrepareCustomGrammars();
			return this.customGrammarDir.FilePath;
		}

		internal override List<KeyValuePair<string, int>> FilterWordsInLexion(List<KeyValuePair<string, int>> rawList, int maxNumberToKeep)
		{
			string text = Path.Combine(Util.GetTranscriptionGrammarDir(this.transcriptionLanguage), "TSR-LM.cfp");
			Grammar grammar = new Grammar(text);
			bool flag = false;
			List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(maxNumberToKeep);
			List<KeyValuePair<string, int>> result;
			try
			{
				this.transcriptionEngine.LoadGrammar(grammar);
				flag = true;
				int num = 0;
				foreach (KeyValuePair<string, int> item in rawList)
				{
					Pronounceable pronounceable = grammar.IsPronounceable(item.Key);
					if (1 == pronounceable || pronounceable == null)
					{
						list.Add(item);
						if (++num >= maxNumberToKeep)
						{
							break;
						}
					}
				}
				result = list;
			}
			finally
			{
				if (flag)
				{
					this.transcriptionEngine.UnloadGrammar(grammar);
				}
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering UcmaOfflineTranscriber.Dispose", new object[0]);
				this.TranscribeCompleted = null;
				if (this.transcriptionEngine != null)
				{
					this.transcriptionEngine.SpeechRecognized -= this.OnSpeechRecognized;
					this.transcriptionEngine.RecognizeCompleted -= this.OnRecognizeCompleted;
					this.transcriptionEngine.Dispose();
					this.transcriptionEngine = null;
				}
				if (this.customGrammarDir != null)
				{
					this.customGrammarDir.Dispose();
					this.customGrammarDir = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UcmaOfflineTranscriber>(this);
		}

		private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs eventArgs)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering UcmaOfflineTranscriber.OnSpeechRecognized", new object[0]);
			this.transcriptionResults.Add(new UcmaTranscriptionResult(eventArgs.Result));
		}

		private void OnRecognizeCompleted(object sender, RecognizeCompletedEventArgs rcea)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Entering UcmaOfflineTranscriber.OnSpeechRecognized", new object[0]);
			BaseUMOfflineTranscriber.TranscribeCompletedEventArgs e = new BaseUMOfflineTranscriber.TranscribeCompletedEventArgs((rcea.Error == null) ? this.transcriptionResults : new List<IUMTranscriptionResult>(), rcea.Error, rcea.Cancelled, rcea.UserState);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Recognition completed: Error = {0}, Cancelled = {1}", new object[]
			{
				rcea.Error,
				rcea.Cancelled
			});
			this.transcriptionEngine.SetInputToNull();
			this.TranscribeCompleted(this, e);
		}

		private void PrepareCustomGrammars()
		{
			this.customGrammarDir = TempFileFactory.CreateTempDir();
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Created temp dir {0} for containing the custom grammars for current transcription", new object[]
			{
				this.customGrammarDir.FilePath
			});
			string transcriptionGrammarDir = Util.GetTranscriptionGrammarDir(this.transcriptionLanguage);
			foreach (string text in Directory.GetFiles(transcriptionGrammarDir, "*.grxml"))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Copying transcrption grammar {0} to custom grammar dir {1}", new object[]
				{
					text,
					this.customGrammarDir.FilePath
				});
				File.Copy(text, Path.Combine(this.customGrammarDir.FilePath, Path.GetFileName(text)));
			}
			List<ContactInfo> list = new List<ContactInfo>(2);
			List<ContactInfo> list2 = new List<ContactInfo>(1);
			if (base.CallerInfo != null)
			{
				list.Add(base.CallerInfo);
				list2.Add(base.CallerInfo);
			}
			list.Add(base.CalleeInfo);
			List<CustomGrammarBase> list3 = new List<CustomGrammarBase>(2);
			list3.Add(new PersonInfoCustomGrammar(this.transcriptionLanguage, list));
			if (base.TopN != null)
			{
				list3.Add(new TopNWordsCustomGrammar(this.transcriptionLanguage, base.TopN.GetFilteredList(this)));
			}
			foreach (CustomGrammarBase customGrammarBase in new List<CustomGrammarBase>(3)
			{
				new PersonNameCustomGrammar(this.transcriptionLanguage, list),
				new GenAppCustomGrammar(this.transcriptionLanguage, list3)
			})
			{
				customGrammarBase.WriteCustomGrammar(this.customGrammarDir.FilePath);
			}
		}

		private SpeechRecognitionEngine transcriptionEngine;

		private CultureInfo transcriptionLanguage;

		private List<IUMTranscriptionResult> transcriptionResults = new List<IUMTranscriptionResult>();

		private ITempFile customGrammarDir;
	}
}
