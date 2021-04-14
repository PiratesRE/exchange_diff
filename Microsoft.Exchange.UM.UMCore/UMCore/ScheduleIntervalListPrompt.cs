using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ScheduleIntervalListPrompt : VariablePrompt<List<TimeRange>>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"scheduleIntervalList",
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ScheduleIntervalListPrompt returning ssmlstring: {0}.", new object[]
			{
				base.SbSsml.ToString()
			});
			return base.SbSsml.ToString();
		}

		protected override void InternalInitialize()
		{
			if (base.InitVal == null || base.InitVal.Count == 0)
			{
				return;
			}
			foreach (TimeRange s in base.InitVal)
			{
				this.AddScheduleIntervalPrompt(s);
			}
		}

		private void AddScheduleIntervalPrompt(TimeRange s)
		{
			ScheduleIntervalPrompt scheduleIntervalPrompt = new ScheduleIntervalPrompt(true);
			scheduleIntervalPrompt.Initialize(base.Config, base.Culture, s);
			base.AddPrompt(scheduleIntervalPrompt);
			base.SbSsml.Append("<break time=\"400ms\"/>");
		}
	}
}
