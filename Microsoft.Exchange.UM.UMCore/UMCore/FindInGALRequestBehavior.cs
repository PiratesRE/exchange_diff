using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FindInGALRequestBehavior : PeopleSearchRequestBehavior
	{
		public FindInGALRequestBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid) : base(id, culture, userObjectGuid, tenantGuid)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering FindInGALRequestBehavior constructor", new object[0]);
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
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering GALRequestBehavior.PrepareGrammars", new object[0]);
			List<UMGrammar> list = new List<UMGrammar>();
			ADRecipient adrecipient = base.GetADRecipient();
			DirectoryGrammarHandler directoryGrammarHandler = DirectoryGrammarHandler.CreateHandler(adrecipient.OrganizationId);
			directoryGrammarHandler.PrepareGrammarAsync(adrecipient, base.Culture);
			SearchGrammarFile searchGrammarFile = directoryGrammarHandler.WaitForPrepareGrammarCompletion();
			if (searchGrammarFile == null)
			{
				PIIMessage pii = PIIMessage.Create(PIIType._User, adrecipient.DistinguishedName);
				MobileSpeechRecoTracer.TraceDebug(this, base.Id, pii, "Error retrieving grammar for recipient='_User', grammar='{0}'", new object[]
				{
					directoryGrammarHandler
				});
				throw new SpeechGrammarFetchErrorException(directoryGrammarHandler.ToString());
			}
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Grammar path='{0}', Base URI='{1}'", new object[]
			{
				searchGrammarFile.FilePath,
				(searchGrammarFile.BaseUri != null) ? searchGrammarFile.BaseUri.ToString() : "<null>"
			});
			list.Add(new UMGrammar(searchGrammarFile.FilePath, "MobilePeopleSearch", base.Culture, searchGrammarFile.BaseUri, directoryGrammarHandler.DeleteFileAfterUse));
			return list;
		}

		public override string ProcessRecoResults(List<IMobileRecognitionResult> results)
		{
			ValidateArgument.NotNull(results, "results");
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering GALRequestBehavior.ProcessRecoResults", new object[0]);
			List<string> requiredTags = new List<string>
			{
				"SMTP",
				"ObjectGuid"
			};
			return base.ConvertResultsToXml(results, requiredTags);
		}

		private const int MaxAlternatesValue = 5;

		private const int MaxProcessingTimeValue = 60000;
	}
}
