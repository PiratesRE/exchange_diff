using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class MowaStaticGrammarRecognitionBehaviorBase : MobileSpeechRecoRequestBehavior
	{
		private protected ExTimeZone TimeZone { protected get; private set; }

		public MowaStaticGrammarRecognitionBehaviorBase(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timeZone) : base(id, culture, userObjectGuid, tenantGuid)
		{
			this.TimeZone = timeZone;
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
				return 1;
			}
		}

		public override int MaxProcessingTime
		{
			get
			{
				return 20000;
			}
		}

		public abstract string MowaGrammarRuleName { get; }

		public abstract List<string> TagsToProcess { get; }

		public override List<UMGrammar> PrepareGrammars()
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering MowaStaticGrammarRecognitionBehaviorBase.PrepareGrammars.", new object[0]);
			List<UMGrammar> list = new List<UMGrammar>();
			UMGrammarConfig umgrammarConfig = new StaticUmGrammarConfig("Mowascenarios.grxml", this.MowaGrammarRuleName, string.Empty, null);
			UMGrammar grammar = umgrammarConfig.GetGrammar(null, base.Culture);
			ExDateTime exDateTime = new ExDateTime(this.TimeZone, DateTime.UtcNow);
			int num = (int)(-(int)exDateTime.Bias.TotalMinutes);
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Client time zone offset (minutes) is {0}.", new object[]
			{
				num
			});
			grammar.Script = string.Format(CultureInfo.InvariantCulture, MowaStaticGrammarRecognitionBehaviorBase.InitializationScriptTemplate, new object[]
			{
				num
			});
			list.Add(grammar);
			return list;
		}

		public override string ProcessRecoResults(List<IMobileRecognitionResult> results)
		{
			ValidateArgument.NotNull(results, "results");
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering MowaStaticGrammarRecognitionBehaviorBase.ProcessRecoResults", new object[0]);
			return base.ConvertResultsToXml(results, this.TagsToProcess);
		}

		protected override bool ShouldAcceptBasedOnSmartConfidenceThreshold(IUMRecognitionPhrase phrase, MobileSpeechRecoResultType resultType)
		{
			switch (resultType)
			{
			case MobileSpeechRecoResultType.DaySearch:
				return phrase.ShouldAcceptBasedOnSmartConfidence(MobileSpeechRecoRequestBehavior.GetKeywordsFromGrammar("grCalendarDaySearch", base.Culture));
			case MobileSpeechRecoResultType.AppointmentCreation:
				return phrase.ShouldAcceptBasedOnSmartConfidence(MobileSpeechRecoRequestBehavior.GetKeywordsFromGrammar("grCalendarDayNewAppointment", base.Culture));
			default:
				return base.ShouldAcceptBasedOnSmartConfidenceThreshold(phrase, resultType);
			}
		}

		private const int MaxAlternatesValue = 1;

		private const int MaxProcessingTimeValue = 20000;

		private static readonly string InitializationScriptTemplate = "var st = new Date(); $.ClientToday = new Date(st.getTime() - 60000 * ({0} - st.getTimezoneOffset()));";
	}
}
