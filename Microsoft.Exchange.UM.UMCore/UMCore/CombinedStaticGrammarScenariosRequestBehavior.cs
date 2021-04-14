using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CombinedStaticGrammarScenariosRequestBehavior : CombinedGrammarScenariosRequestBehavior
	{
		public CombinedStaticGrammarScenariosRequestBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timeZone) : base(id, culture, userObjectGuid, tenantGuid, timeZone)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering CombinedStaticGrammarScenariosRequestBehavior constructor", new object[0]);
		}

		protected override void InitializeScenarioBehaviors(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timezone)
		{
			MobileSpeechRecoTracer.TraceDebug(this, base.Id, "Entering CombinedStaticScenariosRequestBehavior.InitializeScenarioBehaviors", new object[0]);
			base.ScenarioRequestBehaviors = new List<MobileSpeechRecoRequestBehavior>();
			base.ScenarioRequestBehaviors.Add(new DaySearchBehavior(Guid.NewGuid(), culture, userObjectGuid, tenantGuid, timezone));
			base.ScenarioRequestBehaviors.Add(new DateTimeAndDurationRecognitionBehavior(Guid.NewGuid(), culture, userObjectGuid, tenantGuid, timezone));
		}

		protected override MobileSpeechRecoResultType[] SupportedResultTypes
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
