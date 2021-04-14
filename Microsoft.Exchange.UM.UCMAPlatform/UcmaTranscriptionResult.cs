using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaTranscriptionResult : IUMTranscriptionResult
	{
		internal UcmaTranscriptionResult(RecognitionResult transcriptionResult)
		{
			this.confidence = transcriptionResult.Confidence;
			this.rawWords = transcriptionResult.Words;
			this.audioPosition = transcriptionResult.Audio.AudioPosition;
			this.audioDuration = transcriptionResult.Audio.Duration;
			this.debugText = transcriptionResult.Text;
			this.semanticRoot = UcmaTranscriptionResult.BuildSemanticRoot(transcriptionResult);
			this.replacementWords = UcmaTranscriptionResult.BuildReplacementWordList(transcriptionResult.ReplacementWordUnits, this.semanticRoot, this.rawWords);
		}

		public float Confidence
		{
			get
			{
				return this.confidence;
			}
		}

		public TimeSpan AudioDuration
		{
			get
			{
				return this.audioDuration;
			}
		}

		public int TotalWords
		{
			get
			{
				return this.rawWords.Count;
			}
		}

		public int CustomWords
		{
			get
			{
				if (this.lazyCustomWords == null)
				{
					this.lazyCustomWords = new int?(this.GetWordCount("customGrammarWords"));
				}
				return this.lazyCustomWords.Value;
			}
		}

		public int TopNWords
		{
			get
			{
				if (this.lazyTopNWords == null)
				{
					this.lazyTopNWords = new int?(this.GetWordCount("topNWords"));
				}
				return this.lazyTopNWords.Value;
			}
		}

		public List<IUMRecognizedWord> GetRecognizedWords()
		{
			List<IUMRecognizedWord> list = new List<IUMRecognizedWord>();
			TimeSpan transcriptionResultAudioPosition = this.audioPosition;
			int i = 0;
			using (List<UcmaReplacementText>.Enumerator enumerator = this.replacementWords.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UcmaReplacementText ucmaReplacementText = enumerator.Current;
					while (i < ucmaReplacementText.FirstWordIndex)
					{
						RecognizedWordUnit recognizedWordUnit = this.rawWords[i++];
						list.Add(new UcmaRecognizedWordUnit(recognizedWordUnit, transcriptionResultAudioPosition));
					}
					list.Add(new UcmaRecognizedWordUnit(ucmaReplacementText, this.rawWords, transcriptionResultAudioPosition));
					i += ucmaReplacementText.CountOfWords;
				}
				goto IL_A6;
			}
			IL_86:
			RecognizedWordUnit recognizedWordUnit2 = this.rawWords[i++];
			list.Add(new UcmaRecognizedWordUnit(recognizedWordUnit2, transcriptionResultAudioPosition));
			IL_A6:
			if (i >= this.rawWords.Count)
			{
				return list;
			}
			goto IL_86;
		}

		public List<IUMRecognizedFeature> GetRecognizedFeatures(int firstWordOffset)
		{
			List<IUMRecognizedFeature> list = new List<IUMRecognizedFeature>();
			try
			{
				foreach (KeyValuePair<string, SemanticValue> fragment in this.semanticRoot)
				{
					UcmaRecognizedFeature item;
					if (UcmaRecognizedFeature.TryCreate(fragment, firstWordOffset, this.replacementWords, out item))
					{
						list.Add(item);
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Exception accessing semantics {0}", new object[]
				{
					ex
				});
			}
			return list;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Confidence:{0} DurationMSEC:{1} Text:{2}", new object[]
			{
				this.Confidence,
				this.AudioDuration,
				this.debugText
			});
		}

		private static List<UcmaReplacementText> BuildReplacementWordList(Collection<ReplacementText> speechReplacements, SemanticValue semanticRoot, ReadOnlyCollection<RecognizedWordUnit> words)
		{
			List<UcmaReplacementText> result = UcmaTranscriptionResult.CloneReplacementWords(speechReplacements);
			List<UcmaReplacementText> list = UcmaTranscriptionResult.CreatePhoneReplacementText(semanticRoot, words);
			foreach (UcmaReplacementText phoneText in list)
			{
				result = UcmaTranscriptionResult.InsertMissingPhoneText(phoneText, result);
			}
			return result;
		}

		private static List<UcmaReplacementText> CloneReplacementWords(Collection<ReplacementText> speechReplacements)
		{
			List<UcmaReplacementText> list = new List<UcmaReplacementText>(speechReplacements.Count);
			foreach (ReplacementText replacementText in speechReplacements)
			{
				UcmaReplacementText item = new UcmaReplacementText(replacementText.Text, replacementText.DisplayAttributes, replacementText.FirstWordIndex, replacementText.CountOfWords);
				list.Add(item);
			}
			return list;
		}

		private static List<UcmaReplacementText> CreatePhoneReplacementText(SemanticValue root, ReadOnlyCollection<RecognizedWordUnit> words)
		{
			List<UcmaReplacementText> list = new List<UcmaReplacementText>();
			try
			{
				foreach (KeyValuePair<string, SemanticValue> keyValuePair in root)
				{
					string text = UcmaRecognizedFeature.ParseName(keyValuePair.Value);
					if (text.Equals("PhoneNumber", StringComparison.OrdinalIgnoreCase))
					{
						string text2 = UcmaRecognizedFeature.ParsePhoneNumberSemanticValue(keyValuePair.Value);
						int num = UcmaRecognizedFeature.ParseWordCount(keyValuePair.Value);
						int num2 = UcmaRecognizedFeature.ParseFirstWordIndex(keyValuePair.Value);
						int index = num2 + num - 1;
						DisplayAttributes displayAttributes = 16 & words[num2].DisplayAttributes;
						displayAttributes |= words[index].DisplayAttributes;
						list.Add(new UcmaReplacementText(text2, displayAttributes, num2, num));
					}
				}
				list.Sort((UcmaReplacementText lhs, UcmaReplacementText rhs) => lhs.FirstWordIndex.CompareTo(rhs.FirstWordIndex));
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, null, "Exception accessing semantics {0}", new object[]
				{
					ex
				});
			}
			return list;
		}

		private static List<UcmaReplacementText> InsertMissingPhoneText(UcmaReplacementText phoneText, List<UcmaReplacementText> replacementWords)
		{
			bool flag = true;
			int i;
			for (i = 0; i < replacementWords.Count; i++)
			{
				UcmaReplacementText ucmaReplacementText = replacementWords[i];
				if (ucmaReplacementText.FirstWordIndex == phoneText.FirstWordIndex)
				{
					PIIMessage[] data = new PIIMessage[]
					{
						PIIMessage.Create(PIIType._PII, phoneText.Text),
						PIIMessage.Create(PIIType._PII, ucmaReplacementText.Text)
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, null, data, "Ignoring phone semantic value _PII1 because there is already replacement text _PII2 for it.", new object[0]);
					flag = false;
					break;
				}
				if (ucmaReplacementText.FirstWordIndex > phoneText.FirstWordIndex)
				{
					break;
				}
			}
			if (flag)
			{
				PIIMessage data2 = PIIMessage.Create(PIIType._PII, phoneText.Text);
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, null, data2, "Adding phone semantic value _PII at index '{0}' because no other suitable replacement text was found.", new object[]
				{
					i
				});
				replacementWords.Insert(i, phoneText);
			}
			return replacementWords;
		}

		private static SemanticValue BuildSemanticRoot(RecognitionResult transcriptionResult)
		{
			SemanticValue result = null;
			try
			{
				result = transcriptionResult.Semantics["Fragments"];
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, null, "Exception accessing semantics {0}", new object[]
				{
					ex
				});
				result = new SemanticValue(string.Empty);
			}
			return result;
		}

		private int GetWordCount(string semanticTarget)
		{
			int result = 0;
			try
			{
				foreach (KeyValuePair<string, SemanticValue> keyValuePair in this.semanticRoot)
				{
					this.GetWordCount(keyValuePair, semanticTarget, ref result);
				}
			}
			catch (InvalidOperationException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Exception accessing semantics {0}", new object[]
				{
					ex
				});
			}
			return result;
		}

		private void GetWordCount(KeyValuePair<string, SemanticValue> semanticRoot, string semanticTarget, ref int count)
		{
			if (semanticRoot.Value == null || semanticRoot.Value.Count == 0)
			{
				return;
			}
			if (semanticRoot.Value.ContainsKey(semanticTarget))
			{
				SemanticValue semanticValue = semanticRoot.Value["_attributes"];
				string s = (string)semanticValue["CountOfWords"].Value;
				count += int.Parse(s, CultureInfo.InvariantCulture);
			}
			foreach (KeyValuePair<string, SemanticValue> keyValuePair in semanticRoot.Value)
			{
				this.GetWordCount(keyValuePair, semanticTarget, ref count);
			}
		}

		private readonly float confidence;

		private readonly string debugText;

		private readonly TimeSpan audioPosition;

		private readonly TimeSpan audioDuration;

		private List<UcmaReplacementText> replacementWords;

		private SemanticValue semanticRoot;

		private ReadOnlyCollection<RecognizedWordUnit> rawWords;

		private int? lazyCustomWords;

		private int? lazyTopNWords;
	}
}
