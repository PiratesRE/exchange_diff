using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaMobileRecognizer : DisposableBase, IMobileRecognizer, IDisposable
	{
		protected UcmaMobileRecognizer(Guid requestId, SpeechRecognitionEngine engine)
		{
			MobileSpeechRecoTracer.TraceDebug(this, requestId, "Entering UcmaMobileRecognizer constructor", new object[0]);
			this.requestId = requestId;
			this.speechRecognitionEngine = engine;
			this.recoResults = new List<IMobileRecognitionResult>(1);
			this.speechRecognitionEngine.LoadGrammarCompleted += this.OnLoadGrammarCompleted;
			this.speechRecognitionEngine.RecognizeCompleted += this.OnRecognizeCompleted;
			this.speechRecognitionEngine.SpeechDetected += this.OnSpeechDetected;
		}

		public static bool TryCreate(Guid requestId, CultureInfo culture, SpeechRecognitionEngineType engineType, int maxAlternates, out IMobileRecognizer recognizer)
		{
			ValidateArgument.NotNull(culture, "culture");
			MobileSpeechRecoTracer.TraceDebug(null, requestId, "Entering UcmaMobileRecognizer.TryCreate culture='{0}', engineType='{1}'", new object[]
			{
				culture,
				engineType
			});
			recognizer = null;
			string text = null;
			if (UcmaInstalledRecognizers.TryGetRecognizerId(engineType, culture, out text))
			{
				MobileSpeechRecoTracer.TraceDebug(null, requestId, "Found an engine of type '{0}' and culture '{1}, recognizerId = '{2}'", new object[]
				{
					engineType,
					culture,
					text
				});
				SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine(text);
				speechRecognitionEngine.MaxAlternates = maxAlternates;
				UcmaMobileRecognizer.UpdateRecognizerSettings(speechRecognitionEngine, engineType);
				recognizer = new UcmaMobileRecognizer(requestId, speechRecognitionEngine);
			}
			else
			{
				MobileSpeechRecoTracer.TraceError(null, requestId, "Couldn't find an engine of type '{0}' and culture '{1}'", new object[]
				{
					engineType,
					culture
				});
			}
			return recognizer != null;
		}

		public void LoadGrammars(List<UMGrammar> grammars)
		{
			ValidateArgument.NotNull(grammars, "grammars");
			ExAssert.RetailAssert(grammars.Count > 0, "grammars.Count = '{0}'", new object[]
			{
				grammars.Count
			});
			MobileSpeechRecoTracer.TraceDebug(this, this.requestId, "Entering UcmaMobileRecognizer.LoadGrammars", new object[0]);
			foreach (UMGrammar umgrammar in grammars)
			{
				MobileSpeechRecoTracer.TracePerformance(this, this.requestId, "UcmaMobileRecognizer.LoadGrammars grammar path='{0}', grammar rule = '{1}', script = '{2}'", new object[]
				{
					umgrammar.Path,
					umgrammar.RuleName,
					umgrammar.Script
				});
				Grammar grammar = UcmaUtils.CreateGrammar(umgrammar);
				grammar.Name = umgrammar.Path;
				this.speechRecognitionEngine.LoadGrammarAsync(grammar);
			}
		}

		public void RecognizeAsync(byte[] audioBytes, RecognizeCompletedDelegate callback)
		{
			ValidateArgument.NotNull(audioBytes, "audioBytes");
			ValidateArgument.NotNull(callback, "callback");
			MobileSpeechRecoTracer.TraceDebug(this, this.requestId, "Entering UcmaMobileRecognizer.RecognizeAsync", new object[0]);
			this.recognizeCompletedCallback = callback;
			this.audioStream = this.CreateAudioStream(audioBytes);
			this.speechRecognitionEngine.SetInputToWaveStream(this.audioStream);
			MobileSpeechRecoTracer.TracePerformance(this, this.requestId, "UcmaMobileRecognizer.RecognizeAsync starting recognition", new object[0]);
			bool flag = false;
			FaultInjectionUtils.FaultInjectChangeValue<bool>(4112919869U, ref flag);
			if (flag)
			{
				Thread.Sleep(120000);
			}
			this.speechRecognitionEngine.RecognizeAsync(1);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				MobileSpeechRecoTracer.TraceDebug(this, this.requestId, "Entering UcmaMobileRecognizer.Dispose", new object[0]);
				if (this.speechRecognitionEngine != null)
				{
					this.speechRecognitionEngine.LoadGrammarCompleted -= this.OnLoadGrammarCompleted;
					this.speechRecognitionEngine.RecognizeCompleted -= this.OnRecognizeCompleted;
					this.speechRecognitionEngine.SpeechDetected -= this.OnSpeechDetected;
					this.speechRecognitionEngine.Dispose();
					this.speechRecognitionEngine = null;
				}
				if (this.audioStream != null)
				{
					this.audioStream.Dispose();
					this.audioStream = null;
				}
				this.recognizeCompletedCallback = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UcmaMobileRecognizer>(this);
		}

		private static void FixWaveChunkLength(BinaryWriter writer)
		{
			int value = (int)(writer.BaseStream.Length - (writer.BaseStream.Position + 4L));
			writer.Write(value);
		}

		private static void UpdateRecognizerSettings(SpeechRecognitionEngine engine, SpeechRecognitionEngineType engineType)
		{
			switch (engineType)
			{
			case SpeechRecognitionEngineType.CmdAndControl:
				engine.UpdateRecognizerSetting("AssumeCFGFromTrustedSource", 1);
				return;
			case SpeechRecognitionEngineType.Transcription:
				throw new NotSupportedException();
			default:
				ExAssert.RetailAssert(false, "Invalid speech recognition engine type - {0}", new object[]
				{
					engineType
				});
				return;
			}
		}

		private string GetFileIdForLogging()
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			stringBuilder.Append(this.requestId.ToString("N", CultureInfo.InvariantCulture));
			stringBuilder.Append("-");
			stringBuilder.Append(this.speechRecognitionEngine.RecognizerInfo.Culture);
			foreach (Grammar grammar in this.speechRecognitionEngine.Grammars)
			{
				stringBuilder.Append("-");
				stringBuilder.Append(grammar.RuleName);
			}
			return stringBuilder.ToString();
		}

		private MemoryStream CreateAudioStream(byte[] audioBytes)
		{
			using (MemoryStream memoryStream = new MemoryStream(audioBytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						if (memoryStream.Length >= 48L)
						{
							memoryStream.Position = 4L;
							if (binaryReader.ReadUInt32() == 4294967295U)
							{
								memoryStream.Position = 4L;
								UcmaMobileRecognizer.FixWaveChunkLength(binaryWriter);
								memoryStream.Position = 44L;
								UcmaMobileRecognizer.FixWaveChunkLength(binaryWriter);
							}
						}
					}
				}
			}
			this.LogAudioBytes(audioBytes);
			return new MemoryStream(audioBytes);
		}

		private void LogAudioBytes(byte[] audioBytes)
		{
			string text = string.Empty;
			try
			{
				if (UcmaAudioLogging.MobileSpeechRecoAudioLoggingEnabled)
				{
					string text2 = Path.Combine(Utils.TempPath, "MobileReco");
					Directory.CreateDirectory(text2);
					text = this.GetFileIdForLogging();
					string path = string.Format(CultureInfo.InvariantCulture, "{0}.wav", new object[]
					{
						text
					});
					string text3 = Path.Combine(text2, path);
					MobileSpeechRecoTracer.TraceDebug(this, this.requestId, "Audio file path ='{0}'", new object[]
					{
						text3
					});
					using (FileStream fileStream = new FileStream(text3, FileMode.Create, FileAccess.Write))
					{
						fileStream.Write(audioBytes, 0, audioBytes.Length);
					}
				}
			}
			catch (Exception ex)
			{
				MobileSpeechRecoTracer.TraceError(this, this.requestId, "Error logging audio fileId='{0}', exception='{1}'", new object[]
				{
					text,
					ex
				});
			}
		}

		private void LogSpeechResults(Exception speechError, bool speechDetected)
		{
			Exception ex = null;
			try
			{
				if (UcmaAudioLogging.MobileSpeechRecoAudioLoggingEnabled)
				{
					string text = Path.Combine(Utils.TempPath, "MobileReco");
					Directory.CreateDirectory(text);
					string path = string.Format(CultureInfo.InvariantCulture, "{0}.sml", new object[]
					{
						this.GetFileIdForLogging()
					});
					using (FileStream fileStream = new FileStream(Path.Combine(text, path), FileMode.Create, FileAccess.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							if (this.recoResults.Count > 0)
							{
								streamWriter.Write(((UcmaMobileRecognitionResult)this.recoResults[0]).ToSml());
							}
							else
							{
								streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
								{
									speechError
								}));
							}
							streamWriter.Close();
						}
					}
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				if (ex != null)
				{
					MobileSpeechRecoTracer.TraceError(this, this.requestId, "Error logging reco result requestId='{0}', exception='{1}'", new object[]
					{
						this.requestId,
						ex
					});
				}
			}
		}

		private void OnRecognizeCompleted(object sender, RecognizeCompletedEventArgs rcea)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.requestId, "Entering UcmaMobileRecognizer.OnRecognizeCompleted", new object[0]);
			MobileSpeechRecoTracer.TracePerformance(this, this.requestId, "Recognition completed Error='{0}', Canceled='{1}'", new object[]
			{
				rcea.Error,
				rcea.Cancelled
			});
			if (rcea.Result != null)
			{
				this.recoResults.Add(new UcmaMobileRecognitionResult(this.requestId, rcea.Result));
			}
			this.speechRecognitionEngine.SetInputToNull();
			this.LogSpeechResults(rcea.Error, this.speechDetected);
			if (this.recognizeCompletedCallback != null)
			{
				this.recognizeCompletedCallback(this.recoResults, this.loadGrammarError ?? rcea.Error, this.speechDetected);
				this.recognizeCompletedCallback = null;
			}
		}

		private void OnLoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs lcea)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.requestId, "Entering UcmaMobileRecognizer.OnLoadGrammarCompleted", new object[0]);
			MobileSpeechRecoTracer.TracePerformance(this, this.requestId, "Load grammar completed Grammar='{0}', Error='{1}', Canceled='{2}'", new object[]
			{
				lcea.Grammar.Name,
				lcea.Error,
				lcea.Cancelled
			});
			this.loadGrammarError = lcea.Error;
			if (lcea.Error != null)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MobileSpeechRecoLoadGrammarFailure, null, new object[]
				{
					this.requestId,
					lcea.Grammar.Name,
					CommonUtil.ToEventLogString(lcea.Error.Message)
				});
			}
		}

		private void OnSpeechDetected(object sender, SpeechDetectedEventArgs eventArgs)
		{
			MobileSpeechRecoTracer.TraceDebug(this, this.requestId, "Entering UcmaMobileRecognizer.OnSpeechDetected", new object[0]);
			this.speechDetected = true;
		}

		private const int FaultInjectionRecognitionDelay = 120000;

		private const string LogMobileRecoTempFileDir = "MobileReco";

		private readonly Guid requestId;

		private SpeechRecognitionEngine speechRecognitionEngine;

		private RecognizeCompletedDelegate recognizeCompletedCallback;

		private MemoryStream audioStream;

		private List<IMobileRecognitionResult> recoResults;

		private bool speechDetected;

		private Exception loadGrammarError;
	}
}
