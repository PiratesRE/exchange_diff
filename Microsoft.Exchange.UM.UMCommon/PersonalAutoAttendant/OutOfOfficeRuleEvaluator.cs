using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class OutOfOfficeRuleEvaluator : IRuleEvaluator
	{
		public OutOfOfficeRuleEvaluator(OutOfOfficeStatusEnum condition)
		{
			this.oofCondition = condition;
		}

		public bool Evaluate(IDataLoader dataLoader)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "OutOfOfficeStatusEvaluator:Evaluate() Condition={0}", new object[]
			{
				this.oofCondition
			});
			if (this.oofCondition == OutOfOfficeStatusEnum.None)
			{
				return true;
			}
			UserOofSettings userOofSettings = null;
			bool flag = false;
			dataLoader.GetUserOofSettings(out userOofSettings, out flag);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "OutOfOfficeStatusEvaluator:Evaluate() UM-OOF = {0}", new object[]
			{
				flag
			});
			if (flag && this.oofCondition == OutOfOfficeStatusEnum.Oof)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "OutOfOfficeStatusEvaluator:Evaluate() Returning True", new object[0]);
				return true;
			}
			if (userOofSettings == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "OutOfOfficeStatusEvaluator:Evaluate() OWA OOF settings == null. Returning false", new object[0]);
				return false;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "OutOfOfficeStatusEvaluator:Evaluate() OofState = {0}", new object[]
			{
				userOofSettings.OofState
			});
			bool flag2 = userOofSettings.OofState != OofState.Disabled;
			if (userOofSettings.Scheduled)
			{
				DateTime utcNow = DateTime.UtcNow;
				flag2 = (utcNow >= userOofSettings.StartTime && utcNow <= userOofSettings.EndTime);
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "OutOfOfficeStatusEvaluator:Evaluate() Now(UTC): \"{0}\" Start(UTC): \"{1}\" End(UTC): \"{2}\" IsOWAOof = {3}", new object[]
				{
					utcNow.ToString("R", CultureInfo.InvariantCulture),
					userOofSettings.StartTime.ToString("R", CultureInfo.InvariantCulture),
					userOofSettings.EndTime.ToString("R", CultureInfo.InvariantCulture),
					flag2
				});
			}
			bool flag3 = flag2 || flag;
			bool flag4 = false;
			if (this.oofCondition == OutOfOfficeStatusEnum.Oof && flag3)
			{
				flag4 = true;
			}
			else if (this.oofCondition == OutOfOfficeStatusEnum.NotOof && !flag3)
			{
				flag4 = true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "OutOfOfficeStatusEvaluator:Evaluate() returning {0}", new object[]
			{
				flag4
			});
			return flag4;
		}

		private OutOfOfficeStatusEnum oofCondition;
	}
}
