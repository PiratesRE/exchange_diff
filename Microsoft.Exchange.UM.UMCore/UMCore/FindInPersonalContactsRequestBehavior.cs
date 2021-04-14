using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FindInPersonalContactsRequestBehavior : PeopleSearchRequestBehavior
	{
		public FindInPersonalContactsRequestBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid) : base(id, culture, userObjectGuid, tenantGuid)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering FindInPersonalContactsRequestBehavior constructor", new object[0]);
			this.recipient = base.GetADRecipient();
		}

		public override SpeechRecognitionEngineType EngineType
		{
			get
			{
				return SpeechRecognitionEngineType.CmdAndControl;
			}
		}

		public override int MaxAlternates
		{
			get
			{
				return 5;
			}
		}

		public override int MaxProcessingTime
		{
			get
			{
				return 60000;
			}
		}

		public override List<UMGrammar> PrepareGrammars()
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering FindInPersonalContactsRequestBehavior.PrepareGrammars", new object[0]);
			List<UMGrammar> list = new List<UMGrammar>();
			using (NonBlockingReader nonBlockingReader = new NonBlockingReader(new NonBlockingReader.Operation(this.PopulateGrammarFile), this, TimeSpan.FromMilliseconds(6000.0), new NonBlockingReader.TimeoutCallback(this.TimedOutPopulateGrammarFile)))
			{
				nonBlockingReader.StartAsyncOperation();
				if (nonBlockingReader.WaitForCompletion())
				{
					if (this.exceptionThrown != null)
					{
						Exception ex = new PersonalContactsSpeechGrammarErrorException(this.recipient.PrimarySmtpAddress.ToString(), this.exceptionThrown);
						MobileSpeechRecoTracer.TraceError(this, base.Id, "An exception occurred in PopulateGrammarFiler '{0}'", new object[]
						{
							ex
						});
						throw ex;
					}
					list.Add(new UMGrammar(this.grammarFile.FilePath, "MobilePeopleSearch", base.Culture, this.grammarFile.BaseUri, true));
				}
			}
			return list;
		}

		public override string ProcessRecoResults(List<IMobileRecognitionResult> results)
		{
			ValidateArgument.NotNull(results, "results");
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering FindInPersonalContactsRequestBehavior.ProcessRecoResults", new object[0]);
			List<string> requiredTags = new List<string>
			{
				"PersonId",
				"GALLinkID"
			};
			return base.ConvertResultsToXml(results, requiredTags);
		}

		private void PopulateGrammarFile(object state)
		{
			try
			{
				MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering FindInPersonalContactsRequestBehavior.PopulateGrammarFile", new object[0]);
				this.exceptionThrown = null;
				using (UMMailboxRecipient ummailboxRecipient = UMRecipient.Factory.FromADRecipient<UMMailboxRecipient>(this.recipient))
				{
					this.grammarFile = new MowaPersonalContactsGrammarFile(ummailboxRecipient, base.Culture);
				}
				MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Grammar path='{0}', Base URI='{1}'", new object[]
				{
					this.grammarFile.FilePath,
					(this.grammarFile.BaseUri != null) ? this.grammarFile.BaseUri.ToString() : "<null>"
				});
			}
			catch (Exception ex)
			{
				MobileSpeechRecoTracer.TraceError(this, base.Id, "Exception in Populate Grammar file '{0}'", new object[]
				{
					ex
				});
				this.exceptionThrown = ex;
			}
		}

		private void TimedOutPopulateGrammarFile(object state)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering FindInPersonalContactsRequestBehavior.TimedOutPopulateGrammarFile. Timeout retrieving grammar for recipient='{0}'", new object[]
			{
				base.UserObjectGuid
			});
			throw new PersonalContactsSpeechGrammarTimeoutException(this.recipient.PrimarySmtpAddress.ToString());
		}

		private const int MaxAlternatesValue = 5;

		private const int MaxProcessingTimeValue = 60000;

		private const int MaxTimeoutForGrammarGeneration = 6000;

		private MowaPersonalContactsGrammarFile grammarFile;

		private ADRecipient recipient;

		private Exception exceptionThrown;
	}
}
