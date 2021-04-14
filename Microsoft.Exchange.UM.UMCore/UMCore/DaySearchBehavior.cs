using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DaySearchBehavior : MowaStaticGrammarRecognitionBehaviorBase
	{
		public DaySearchBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timeZone) : base(id, culture, userObjectGuid, tenantGuid, timeZone)
		{
		}

		public override string MowaGrammarRuleName
		{
			get
			{
				return "DaySearch";
			}
		}

		public override List<string> TagsToProcess
		{
			get
			{
				return DaySearchBehavior.tagsToProcess;
			}
		}

		protected override MobileSpeechRecoResultType[] SupportedResultTypes
		{
			get
			{
				return DaySearchBehavior.supportedResultTypes;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DaySearchBehavior()
		{
			MobileSpeechRecoResultType[] array = new MobileSpeechRecoResultType[1];
			DaySearchBehavior.supportedResultTypes = array;
		}

		private static readonly List<string> tagsToProcess = new List<string>
		{
			"Day",
			"Month",
			"Year",
			"RecoEvent"
		};

		private static readonly MobileSpeechRecoResultType[] supportedResultTypes;
	}
}
