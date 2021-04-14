using System;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PeopleSearchRequestBehavior : MobileSpeechRecoRequestBehavior
	{
		public PeopleSearchRequestBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid) : base(id, culture, userObjectGuid, tenantGuid)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering PeopleSearchRequestBehavior constructor", new object[0]);
		}

		protected override MobileSpeechRecoResultType[] SupportedResultTypes
		{
			get
			{
				return PeopleSearchRequestBehavior.supportedResultTypes;
			}
		}

		protected override bool ShouldAcceptBasedOnSmartConfidenceThreshold(IUMRecognitionPhrase phrase, MobileSpeechRecoResultType resultType)
		{
			switch (resultType)
			{
			case MobileSpeechRecoResultType.FindPeople:
				return phrase.ShouldAcceptBasedOnSmartConfidence(MobileSpeechRecoRequestBehavior.GetKeywordsFromGrammar("grFindPersonByNameMobile", base.Culture));
			case MobileSpeechRecoResultType.EmailPeople:
				return phrase.ShouldAcceptBasedOnSmartConfidence(MobileSpeechRecoRequestBehavior.GetKeywordsFromGrammar("grEmailPersonByNameMobile", base.Culture));
			default:
				return base.ShouldAcceptBasedOnSmartConfidenceThreshold(phrase, resultType);
			}
		}

		private static readonly MobileSpeechRecoResultType[] supportedResultTypes = new MobileSpeechRecoResultType[]
		{
			MobileSpeechRecoResultType.FindPeople,
			MobileSpeechRecoResultType.EmailPeople
		};
	}
}
