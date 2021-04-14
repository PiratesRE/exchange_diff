using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaMobileRecognitionResult : UcmaRecognitionPhrase, IMobileRecognitionResult, IUMRecognitionPhrase
	{
		public UcmaMobileRecognitionResult(Guid requestId, RecognitionResult recognitionResult) : base(recognitionResult)
		{
			ValidateArgument.NotNull(recognitionResult, "recognitionResult");
			MobileSpeechRecoTracer.TraceDebug(this, requestId, "Entering UcmaMobileRecognitionResult constructor", new object[0]);
			List<IUMRecognitionPhrase> list = new List<IUMRecognitionPhrase>(recognitionResult.Alternates.Count);
			foreach (RecognizedPhrase recognizedPhrase in recognitionResult.Alternates)
			{
				if (recognizedPhrase.Confidence >= 0.25f)
				{
					list.Add(new UcmaRecognitionPhrase(recognizedPhrase));
				}
			}
			list.Sort(RecognitionPhraseComparer.StaticInstance);
			if (list.Count >= 1)
			{
				IUMRecognitionPhrase iumrecognitionPhrase = list[0];
				string text = (string)iumrecognitionPhrase["ResultType"];
				MobileSpeechRecoTracer.TraceDebug(this, requestId, "Highest confidence UcmaMobileRecognitionResult alternate text= '{0}', confidence='{1}', resultType='{2}'", new object[]
				{
					iumrecognitionPhrase.Text,
					iumrecognitionPhrase.Confidence,
					text
				});
				if (!string.IsNullOrEmpty(text))
				{
					if (!EnumValidator.TryParse<MobileSpeechRecoResultType>(text, EnumParseOptions.IgnoreCase, out this.mobileScenarioResultType))
					{
						this.LogErrorTraceAndAssertOnPhrase(requestId, "Could not retrieve a valid resultType to the following UcmaMobileRecognitionResult", iumrecognitionPhrase);
					}
				}
				else
				{
					this.LogErrorTraceAndAssertOnPhrase(requestId, "Could not find resultType to the following UcmaMobileRecognitionResult", iumrecognitionPhrase);
				}
			}
			else
			{
				MobileSpeechRecoTracer.TraceDebug(this, requestId, "No alternates found for this Reco, setting resultType to None", new object[0]);
				this.mobileScenarioResultType = MobileSpeechRecoResultType.None;
			}
			this.InitializeAlternatesList(requestId, list);
		}

		public MobileSpeechRecoResultType MobileScenarioResultType
		{
			get
			{
				return this.mobileScenarioResultType;
			}
		}

		public List<IUMRecognitionPhrase> GetRecognitionResults()
		{
			return this.alternatesList;
		}

		private void InitializeAlternatesList(Guid requestId, List<IUMRecognitionPhrase> recoResultsAlternates)
		{
			if (recoResultsAlternates.Count == 0)
			{
				this.alternatesList = new List<IUMRecognitionPhrase>();
				return;
			}
			this.alternatesList = new List<IUMRecognitionPhrase>(recoResultsAlternates.Count);
			foreach (IUMRecognitionPhrase iumrecognitionPhrase in recoResultsAlternates)
			{
				string text = (string)iumrecognitionPhrase["ResultType"];
				if (!string.IsNullOrEmpty(text))
				{
					MobileSpeechRecoResultType mobileSpeechRecoResultType;
					if (EnumValidator.TryParse<MobileSpeechRecoResultType>(text, EnumParseOptions.IgnoreCase, out mobileSpeechRecoResultType))
					{
						MobileSpeechRecoTracer.TraceDebug(this, requestId, "UcmaMobileRecognitionResult alternate text='{0}', confidence='{1}', ResultType='{2}'", new object[]
						{
							iumrecognitionPhrase.Text,
							iumrecognitionPhrase.Confidence,
							text
						});
						if (mobileSpeechRecoResultType == this.mobileScenarioResultType)
						{
							MobileSpeechRecoTracer.TraceDebug(this, requestId, "UcmaMobileRecognitionResult alternate text='{0}', confidence='{1}', ResultType='{2}' will be added to the result list", new object[]
							{
								iumrecognitionPhrase.Text,
								iumrecognitionPhrase.Confidence,
								mobileSpeechRecoResultType.ToString()
							});
							this.alternatesList.Add(iumrecognitionPhrase);
						}
						else
						{
							MobileSpeechRecoTracer.TraceDebug(this, requestId, "UcmaMobileRecognitionResult alternate text='{0}', confidence='{1}', resultType='{2}' not added to list", new object[]
							{
								iumrecognitionPhrase.Text,
								iumrecognitionPhrase.Confidence,
								mobileSpeechRecoResultType.ToString()
							});
						}
					}
					else
					{
						this.LogErrorTraceAndAssertOnPhrase(requestId, "Could not retrieve a valid resultType to the following UcmaMobileRecognitionResult", iumrecognitionPhrase);
					}
				}
				else
				{
					this.LogErrorTraceAndAssertOnPhrase(requestId, "Could not find resultType to the following UcmaMobileRecognitionResult", iumrecognitionPhrase);
				}
			}
		}

		private void LogErrorTraceAndAssertOnPhrase(Guid requestId, string errorMessage, IUMRecognitionPhrase phrase)
		{
			MobileSpeechRecoTracer.TraceError(this, requestId, errorMessage + " alternate text= '{0}', confidence='{1}', resultType='{2}'", new object[]
			{
				phrase.Text,
				phrase.Confidence,
				phrase["ResultType"]
			});
			ExAssert.RetailAssert(false, errorMessage + "Alternate text='{0}' resultType='{1}'", new object[]
			{
				phrase.Text,
				phrase["ResultType"]
			});
		}

		private readonly MobileSpeechRecoResultType mobileScenarioResultType;

		private List<IUMRecognitionPhrase> alternatesList;
	}
}
