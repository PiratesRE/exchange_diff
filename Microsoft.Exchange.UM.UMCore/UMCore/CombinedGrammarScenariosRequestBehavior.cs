using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class CombinedGrammarScenariosRequestBehavior : MobileSpeechRecoRequestBehavior
	{
		public CombinedGrammarScenariosRequestBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timezone) : base(id, culture, userObjectGuid, tenantGuid)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering CombinedGrammarScenariosRequestBehavior constructor", new object[0]);
			this.InitializeScenarioBehaviors(id, culture, userObjectGuid, tenantGuid, timezone);
			this.InitializeMaxAlternates();
		}

		protected List<MobileSpeechRecoRequestBehavior> ScenarioRequestBehaviors { get; set; }

		protected abstract void InitializeScenarioBehaviors(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timezone);

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
				return this.maxAlternates;
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
			ValidateArgument.NotNull(this.ScenarioRequestBehaviors, "RequestBehaviorGrammarsToUse");
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering CombinedGrammarScenariosRequestBehavior.PrepareGrammars", new object[0]);
			List<UMGrammar> list = new List<UMGrammar>();
			foreach (MobileSpeechRecoRequestBehavior mobileSpeechRecoRequestBehavior in this.ScenarioRequestBehaviors)
			{
				List<UMGrammar> collection = mobileSpeechRecoRequestBehavior.PrepareGrammars();
				list.AddRange(collection);
			}
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "CombinedGrammarScenariosRequestBehavior.PrepareGrammars: Done with Preparing Grammars", new object[0]);
			return list;
		}

		public override string ProcessRecoResults(List<IMobileRecognitionResult> results)
		{
			ValidateArgument.NotNull(results, "results");
			ValidateArgument.NotNull(this.ScenarioRequestBehaviors, "ScenarioRequestBehaviors");
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering CombinedGrammarScenariosRequestBehavior.ProcessRecoResults", new object[0]);
			if (results.Count > 0)
			{
				foreach (MobileSpeechRecoRequestBehavior mobileSpeechRecoRequestBehavior in this.ScenarioRequestBehaviors)
				{
					if (mobileSpeechRecoRequestBehavior.CanProcessResultType(results[0].MobileScenarioResultType))
					{
						MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Found a request behavior to processRecoResults", new object[0]);
						return mobileSpeechRecoRequestBehavior.ProcessRecoResults(results);
					}
				}
				return this.GenerateEmptyXmlResult(results);
			}
			return this.GenerateEmptyXmlResult(results);
		}

		private string GenerateEmptyXmlResult(List<IMobileRecognitionResult> results)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Cannot find a request behavior to process result. Generating empty xml result", new object[0]);
			List<string> requiredTags = new List<string>();
			return base.ConvertResultsToXml(results, requiredTags);
		}

		private void InitializeMaxAlternates()
		{
			this.maxAlternates = 0;
			foreach (MobileSpeechRecoRequestBehavior mobileSpeechRecoRequestBehavior in this.ScenarioRequestBehaviors)
			{
				this.maxAlternates += mobileSpeechRecoRequestBehavior.MaxAlternates;
			}
		}

		private const int MaxProcessingTimeValue = 60000;

		private int maxAlternates;
	}
}
