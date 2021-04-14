using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.ApplicationLogic.UM;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TranscriptionData : ITranscriptionData
	{
		internal TranscriptionData(RecoResultType recognitionResult, RecoErrorType recognitionError, CultureInfo language, List<IUMTranscriptionResult> transcriptionResults) : this(recognitionResult, recognitionError, language, transcriptionResults, null, null)
		{
		}

		internal TranscriptionData(RecoResultType recognitionResult, RecoErrorType recognitionError, CultureInfo language, List<IUMTranscriptionResult> transcriptionResults, List<int> testHookParagraphs, List<int> testHookSentences)
		{
			this.RecognitionResult = recognitionResult;
			this.RecognitionError = recognitionError;
			this.Language = language;
			this.config = LocConfig.Instance[language].Transcription;
			float num = 0f;
			float num2 = 0f;
			foreach (IUMTranscriptionResult iumtranscriptionResult in transcriptionResults)
			{
				this.recognizedFeatures.AddRange(iumtranscriptionResult.GetRecognizedFeatures(this.recognizedWords.Count));
				this.recognizedWords.AddRange(iumtranscriptionResult.GetRecognizedWords());
				float num3 = (float)iumtranscriptionResult.AudioDuration.TotalMilliseconds;
				num += num3 * iumtranscriptionResult.Confidence;
				num2 += num3;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "TranscriptionResult:{0} confidenceDuration:{1} confidenceSum:{2}", new object[]
				{
					iumtranscriptionResult,
					num2,
					num
				});
				this.totalWords += iumtranscriptionResult.TotalWords;
				this.customWords += iumtranscriptionResult.CustomWords;
				this.topNWords += iumtranscriptionResult.TopNWords;
			}
			this.Confidence = ((num2 > 0f) ? (num / num2) : 0f);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Confidence(final):{0} ConfidenceBand(final):{1} ", new object[]
			{
				this.Confidence,
				this.ConfidenceBand
			});
			if (this.recognizedWords.Count == 0)
			{
				if (this.RecognitionResult == RecoResultType.Attempted)
				{
					this.RecognitionResult = RecoResultType.Skipped;
					this.RecognitionError = RecoErrorType.AudioQualityPoor;
				}
				else if (this.RecognitionResult == RecoResultType.Partial)
				{
					this.RecognitionResult = RecoResultType.Skipped;
					this.RecognitionError = RecoErrorType.Throttled;
				}
			}
			else if (this.ConfidenceBand == ConfidenceBandType.Low)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Low confidence {0} detected.", new object[]
				{
					this.Confidence
				});
				this.RecognitionResult = RecoResultType.Skipped;
				this.RecognitionError = RecoErrorType.AudioQualityPoor;
				this.recognizedFeatures.Clear();
				this.recognizedWords.Clear();
			}
			this.recognizedParagraphs = new BitArray(this.recognizedWords.Count);
			this.recognizedSentences = new BitArray(this.recognizedWords.Count);
			if (AppConfig.Instance.Service.EnableTranscriptionWhitespace)
			{
				if (this.recognizedWords.Count > 0 && testHookParagraphs != null && testHookSentences != null)
				{
					foreach (int index in testHookParagraphs)
					{
						this.recognizedParagraphs.Set(index, true);
					}
					using (List<int>.Enumerator enumerator3 = testHookSentences.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							int index2 = enumerator3.Current;
							this.recognizedSentences.Set(index2, true);
						}
						goto IL_333;
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Reformatting the transcription text using whitespacing logic", new object[0]);
				this.ReformatText();
			}
			IL_333:
			this.GenerateXML();
		}

		public List<IUMRecognizedWord> RecognizedWords
		{
			get
			{
				return this.recognizedWords;
			}
		}

		public RecoResultType RecognitionResult { get; private set; }

		public float Confidence { get; private set; }

		public CultureInfo Language { get; private set; }

		public ConfidenceBandType ConfidenceBand
		{
			get
			{
				if ((double)this.Confidence > LocConfig.Instance[this.Language].Transcription.HighConfidence)
				{
					return ConfidenceBandType.High;
				}
				if ((double)this.Confidence > LocConfig.Instance[this.Language].Transcription.LowConfidence)
				{
					return ConfidenceBandType.Medium;
				}
				return ConfidenceBandType.Low;
			}
		}

		public XmlDocument TranscriptionXml { get; private set; }

		public RecoErrorType RecognitionError { get; private set; }

		public string ErrorInformation { get; private set; }

		public string GetTrailingSpaces(int currentWordIndex)
		{
			IUMRecognizedWord iumrecognizedWord = this.recognizedWords[currentWordIndex];
			IUMRecognizedWord iumrecognizedWord2 = (currentWordIndex + 1 >= this.recognizedWords.Count) ? null : this.recognizedWords[currentWordIndex + 1];
			if (iumrecognizedWord2 != null && (iumrecognizedWord2.DisplayAttributes & UMDisplayAttributes.ConsumeLeadingSpaces) != UMDisplayAttributes.None)
			{
				return string.Empty;
			}
			if ((iumrecognizedWord.DisplayAttributes & UMDisplayAttributes.OneTrailingSpace) != UMDisplayAttributes.None)
			{
				return " ";
			}
			if ((iumrecognizedWord.DisplayAttributes & UMDisplayAttributes.TwoTrailingSpaces) != UMDisplayAttributes.None)
			{
				return "  ";
			}
			if ((iumrecognizedWord.DisplayAttributes & UMDisplayAttributes.ZeroTrailingSpaces) != UMDisplayAttributes.None)
			{
				return string.Empty;
			}
			if (iumrecognizedWord.DisplayAttributes == UMDisplayAttributes.None)
			{
				return string.Empty;
			}
			throw new UnexpectedSwitchValueException(iumrecognizedWord.DisplayAttributes.ToString());
		}

		internal void UpdatePerfCounters()
		{
			TranscriptionCountersInstance instance = TranscriptionCounters.GetInstance(this.Language.Name);
			switch (this.RecognitionResult)
			{
			case RecoResultType.Attempted:
				Util.IncrementCounter(instance.VoiceMessagesProcessed);
				lock (TranscriptionData.perLanguageAverageConfidence)
				{
					string name = this.Language.Name;
					Average average;
					if (!TranscriptionData.perLanguageAverageConfidence.ContainsKey(name))
					{
						average = new Average();
						TranscriptionData.perLanguageAverageConfidence[name] = average;
					}
					else
					{
						average = TranscriptionData.perLanguageAverageConfidence[name];
					}
					Util.SetCounter(instance.AverageConfidence, average.Update((long)(this.Confidence * 100f)));
				}
				if ((double)this.Confidence <= LocConfig.Instance[this.Language].Transcription.LowConfidence)
				{
					Util.IncrementCounter(instance.VoiceMessagesProcessedWithLowConfidence);
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_TranscriptionWordCounts, null, new object[]
				{
					this.totalWords,
					this.customWords,
					this.topNWords
				});
				UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMTranscriptionThrottling.ToString());
				return;
			case RecoResultType.Skipped:
				if (RecoErrorType.MessageTooLong != this.RecognitionError && RecoErrorType.Throttled == this.RecognitionError)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_TranscriptionNotAttemptedDueToThrottling, null, new object[0]);
					Util.IncrementCounter(instance.VoiceMessagesNotProcessedBecauseOfLowAvailabilityOfResources);
					UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMTranscriptionThrottling.ToString());
				}
				Util.IncrementCounter(AvailabilityCounters.PercentageTranscriptionFailures);
				return;
			case RecoResultType.Partial:
				Util.IncrementCounter(AvailabilityCounters.PercentageTranscriptionFailures);
				Util.IncrementCounter(instance.VoiceMessagesPartiallyProcessedBecauseOfLowAvailabilityOfResources);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_TranscriptionAttemptedButCancelled, null, new object[0]);
				UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMTranscriptionThrottling.ToString());
				return;
			default:
				return;
			}
		}

		internal void UpdateStatistics(PipelineStatisticsLogger.PipelineStatisticsLogRow pipelineStatisticsLogRow)
		{
			pipelineStatisticsLogRow.TranscriptionLanguage = this.Language;
			pipelineStatisticsLogRow.TranscriptionResultType = this.RecognitionResult;
			pipelineStatisticsLogRow.TranscriptionErrorType = this.RecognitionError;
			pipelineStatisticsLogRow.TranscriptionConfidence = this.Confidence;
			pipelineStatisticsLogRow.TranscriptionTotalWords = this.totalWords;
			pipelineStatisticsLogRow.TranscriptionCustomWords = this.customWords;
			pipelineStatisticsLogRow.TranscriptionTopNWords = this.topNWords;
		}

		private void GenerateXML()
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.Schemas = VoiceMailPreviewSchema.SchemaSet;
			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
			string namespaceURI = "http://schemas.microsoft.com/exchange/um/2010/evm";
			XmlElement xmlElement = xmlDocument.CreateElement("ASR", namespaceURI);
			xmlElement.SetAttribute("lang", this.Language.ToString());
			xmlElement.SetAttribute("confidence", Convert.ToString(this.Confidence, CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("confidenceBand", this.ConfidenceBand.ToString().ToLowerInvariant());
			xmlElement.SetAttribute("recognitionResult", this.RecognitionResult.ToString().ToLowerInvariant());
			xmlElement.SetAttribute("recognitionError", this.RecognitionError.ToString().ToLowerInvariant());
			xmlElement.SetAttribute("schemaVersion", "1.0.0.0");
			xmlElement.SetAttribute("productVersion", Util.GetProductVersion());
			xmlElement.SetAttribute("productID", "925712");
			xmlElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			xmlDocument.AppendChild(xmlElement);
			XmlElement xmlElement2 = xmlDocument.CreateElement("Information", namespaceURI);
			xmlElement2.SetAttribute("lang", this.Language.Name);
			xmlElement2.SetAttribute("linkText", Strings.LearnMore.ToString(this.Language));
			xmlElement2.SetAttribute("linkURL", "http://go.microsoft.com/fwlink/?LinkId=150048");
			xmlElement2.InnerText = Strings.InformationText.ToString(this.Language);
			xmlDocument.DocumentElement.AppendChild(xmlElement2);
			List<IUMRecognizedFeature>.Enumerator enumerator = this.recognizedFeatures.GetEnumerator();
			IUMRecognizedFeature iumrecognizedFeature = enumerator.MoveNext() ? enumerator.Current : null;
			XmlNode xmlNode = xmlDocument.DocumentElement;
			for (int i = 0; i < this.recognizedWords.Count; i++)
			{
				if (iumrecognizedFeature != null && iumrecognizedFeature.FirstWordIndex == i)
				{
					XmlElement xmlElement3 = xmlDocument.CreateElement("Feature", namespaceURI);
					xmlElement3.SetAttribute("class", iumrecognizedFeature.Name);
					if (iumrecognizedFeature.Name.Equals("Contact", StringComparison.OrdinalIgnoreCase))
					{
						StoreObjectId storeObjectId = StoreId.EwsIdToStoreObjectId(iumrecognizedFeature.Value);
						string value = Convert.ToBase64String(storeObjectId.ProviderLevelItemId);
						xmlElement3.SetAttribute("reference", value);
						xmlElement3.SetAttribute("reference2", iumrecognizedFeature.Value);
					}
					else
					{
						xmlElement3.SetAttribute("reference", iumrecognizedFeature.Value);
					}
					xmlDocument.DocumentElement.AppendChild(xmlElement3);
					xmlNode = xmlElement3;
				}
				IUMRecognizedWord iumrecognizedWord = this.recognizedWords[i];
				bool flag = i >= this.recognizedWords.Count - 1;
				bool flag2 = !flag && this.recognizedSentences.Get(i + 1);
				bool flag3 = !flag && this.recognizedParagraphs.Get(i + 1);
				bool flag4 = flag2 || flag3;
				string text = null;
				if (flag4)
				{
					text = string.Format(CultureInfo.InvariantCulture, "n{0}.5", new object[]
					{
						i
					});
				}
				else if (!flag)
				{
					text = string.Format(CultureInfo.InvariantCulture, "n{0}", new object[]
					{
						i + 1
					});
				}
				XmlElement xmlElement4 = xmlDocument.CreateElement("Text", namespaceURI);
				xmlElement4.SetAttribute("id", string.Format(CultureInfo.InvariantCulture, "n{0}", new object[]
				{
					i
				}));
				if (text != null)
				{
					xmlElement4.SetAttribute("nx", text);
				}
				xmlElement4.SetAttribute("c", Convert.ToString(iumrecognizedWord.Confidence, CultureInfo.InvariantCulture));
				xmlElement4.SetAttribute("ts", iumrecognizedWord.AudioPosition.ToString());
				xmlElement4.SetAttribute("te", (iumrecognizedWord.AudioPosition + iumrecognizedWord.AudioDuration).ToString());
				xmlElement4.SetAttribute("be", "1");
				xmlElement4.InnerText = iumrecognizedWord.Text + this.GetTrailingSpaces(i);
				xmlNode.AppendChild(xmlElement4);
				if (iumrecognizedFeature != null && iumrecognizedFeature.FirstWordIndex + iumrecognizedFeature.CountOfWords == i + 1)
				{
					xmlNode = xmlDocument.DocumentElement;
					iumrecognizedFeature = (enumerator.MoveNext() ? enumerator.Current : null);
				}
				if (flag4)
				{
					string value2 = flag3 ? "high" : "low";
					XmlElement xmlElement5 = xmlDocument.CreateElement("Break", namespaceURI);
					xmlElement5.SetAttribute("id", text);
					if (!flag)
					{
						xmlElement5.SetAttribute("nx", string.Format(CultureInfo.InvariantCulture, "n{0}", new object[]
						{
							i + 1
						}));
					}
					xmlElement5.SetAttribute("c", "1");
					xmlElement5.SetAttribute("ts", (iumrecognizedWord.AudioPosition + iumrecognizedWord.AudioDuration).ToString());
					xmlElement5.SetAttribute("te", (iumrecognizedWord.AudioPosition + iumrecognizedWord.AudioDuration).ToString());
					xmlElement5.SetAttribute("wt", value2);
					xmlElement5.SetAttribute("be", "1");
					xmlNode.AppendChild(xmlElement5);
				}
			}
			enumerator.Dispose();
			xmlDocument.Validate(delegate(object sender, ValidationEventArgs e)
			{
				string formatString = "Invalid XML generated for EVM. If you hit this it means we're generating XML that is not per our spec. We have external partners that depend on this spec, so please make sure you know what you are doing when trying to fix issue. Validation error = {0}";
				ExAssert.RetailAssert(false, formatString, new object[]
				{
					e.Message
				});
			});
			this.TranscriptionXml = xmlDocument;
		}

		private void ReformatText()
		{
			int num = 0;
			int num2 = this.config.FirstSentenceInNewLine ? 1 : 0;
			TimeSpan t = TimeSpan.Zero;
			TimeSpan t2 = TimeSpan.Zero;
			int i = 0;
			int num3 = 0;
			IUMRecognizedFeature iumrecognizedFeature = (num3 < this.recognizedFeatures.Count) ? this.recognizedFeatures[num3] : null;
			while (i < this.recognizedWords.Count)
			{
				IUMRecognizedWord iumrecognizedWord = this.recognizedWords[i];
				TimeSpan t3 = iumrecognizedWord.AudioPosition - (t + t2);
				if (t3 > this.config.SilenceThreshold && i > 0 && !this.recognizedParagraphs.Get(i))
				{
					num++;
					this.recognizedSentences.Set(i, true);
					if (num - num2 % 3 == 0)
					{
						this.recognizedParagraphs.Set(i, true);
					}
				}
				int num4 = 1;
				if (iumrecognizedFeature != null && i == iumrecognizedFeature.FirstWordIndex)
				{
					num4 = Math.Max(1, iumrecognizedFeature.CountOfWords);
					iumrecognizedFeature = ((++num3 < this.recognizedFeatures.Count) ? this.recognizedFeatures[num3] : null);
				}
				i += num4;
				int num5 = i - 1;
				if (num5 >= 0 && num5 < this.recognizedWords.Count)
				{
					t = this.recognizedWords[num5].AudioPosition;
					t2 = this.recognizedWords[num5].AudioDuration;
				}
			}
			this.AddLastSentenceParagraph();
			for (int j = 0; j < this.recognizedWords.Count; j++)
			{
				if (this.recognizedParagraphs.Get(j))
				{
					this.recognizedSentences.Set(j, false);
					this.CapitalizeNextVisible(j);
					this.RemoveTrailingSpaceFromPreviousVisible(j);
				}
				if (this.recognizedSentences.Get(j))
				{
					if (this.config.CapStartOfNewSentence)
					{
						this.CapitalizeNextVisible(j);
					}
					this.RemoveTrailingSpaceFromPreviousVisible(j);
				}
			}
			this.CapitalizeNextVisible(0);
			this.RemoveTrailingSpaceFromPreviousVisible(this.recognizedWords.Count);
		}

		private void AddLastSentenceParagraph()
		{
			if (this.config.LastSentenceInNewLine && this.recognizedSentences.Count > 1)
			{
				for (int i = this.recognizedSentences.Length - 1; i >= 0; i--)
				{
					if (this.recognizedSentences.Get(i))
					{
						this.recognizedParagraphs.Set(i, true);
						return;
					}
				}
			}
		}

		private void CapitalizeNextVisible(int i)
		{
			i = Math.Max(0, i);
			for (int j = i; j < this.recognizedWords.Count; j++)
			{
				IUMRecognizedWord iumrecognizedWord = this.recognizedWords[j];
				iumrecognizedWord.Text = this.Language.TextInfo.ToTitleCase(iumrecognizedWord.Text);
				if (iumrecognizedWord.Text != null && !string.IsNullOrEmpty(iumrecognizedWord.Text.Trim()))
				{
					return;
				}
			}
		}

		private void RemoveTrailingSpaceFromPreviousVisible(int i)
		{
			i = Math.Min(this.recognizedWords.Count, i);
			for (int j = i - 1; j > 0; j--)
			{
				IUMRecognizedWord iumrecognizedWord = this.recognizedWords[j];
				iumrecognizedWord.DisplayAttributes = UMDisplayAttributes.None;
				if (iumrecognizedWord.Text != null && !string.IsNullOrEmpty(iumrecognizedWord.Text.Trim()))
				{
					return;
				}
			}
		}

		private static Dictionary<string, Average> perLanguageAverageConfidence = new Dictionary<string, Average>();

		private List<IUMRecognizedWord> recognizedWords = new List<IUMRecognizedWord>();

		private List<IUMRecognizedFeature> recognizedFeatures = new List<IUMRecognizedFeature>();

		private BitArray recognizedParagraphs;

		private BitArray recognizedSentences;

		private int totalWords;

		private int customWords;

		private int topNWords;

		private LocConfig.TranscriptionConfig config;
	}
}
