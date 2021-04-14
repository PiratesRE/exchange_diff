using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class FreeBusyRuleEvaluator : IRuleEvaluator
	{
		public FreeBusyRuleEvaluator(FreeBusyStatusEnum freeBusy)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "FreeBusyRuleEvaluator:ctor(FB = {0})", new object[]
			{
				freeBusy.ToString()
			});
			this.freeBusyCondition = freeBusy;
		}

		public bool Evaluate(IDataLoader dataLoader)
		{
			if (this.freeBusyCondition == FreeBusyStatusEnum.None)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "FreeBusyRuleEvaluator:Evaluate() no conditions defined. Returning true", new object[0]);
				return true;
			}
			FreeBusyStatusEnum freeBusyStatusEnum = FreeBusyStatusEnum.None;
			dataLoader.GetFreeBusyInformation(out freeBusyStatusEnum);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "FreeBusyRuleEvaluator:Evaluate() input status = {0} condition value = {1}", new object[]
			{
				freeBusyStatusEnum,
				this.freeBusyCondition
			});
			return (this.freeBusyCondition & freeBusyStatusEnum) != FreeBusyStatusEnum.None;
		}

		private FreeBusyStatusEnum freeBusyCondition;
	}
}
