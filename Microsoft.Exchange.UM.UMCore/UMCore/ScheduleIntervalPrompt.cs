using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ScheduleIntervalPrompt : VariablePrompt<TimeRange>
	{
		public ScheduleIntervalPrompt(bool showFirstDayOfInterval)
		{
			this.showFirstDayOfInterval = showFirstDayOfInterval;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"scheduleInterval",
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ScheduleIntervalPrompt returning ssmlstring: {0}.", new object[]
			{
				base.SbSsml.ToString()
			});
			return base.SbSsml.ToString();
		}

		protected override void InternalInitialize()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ScheduleIntervalPrompt initializing default SSML.", new object[0]);
			ArrayList prompts = GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForAA.DayTimeRange
			});
			DayOfWeekTimeContext varValue = this.showFirstDayOfInterval ? DayOfWeekTimeContext.WithDayAndTime(base.InitVal.StartTime) : DayOfWeekTimeContext.WithTimeOnly(base.InitVal.StartTime);
			VariablePrompt<DayOfWeekTimeContext>.SetActualPromptValues(prompts, "startDayTime", varValue);
			DayOfWeekTimeContext varValue2 = this.showFirstDayOfInterval ? DayOfWeekTimeContext.WithDayAndTime(base.InitVal.EndTime) : DayOfWeekTimeContext.WithTimeOnly(base.InitVal.EndTime);
			VariablePrompt<DayOfWeekTimeContext>.SetActualPromptValues(prompts, "endDayTime", varValue2);
			base.AddPrompts(prompts);
		}

		private bool showFirstDayOfInterval;
	}
}
