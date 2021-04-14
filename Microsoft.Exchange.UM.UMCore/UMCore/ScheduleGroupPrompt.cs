using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ScheduleGroupPrompt : VariablePrompt<ScheduleGroup>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"scheduleGroup",
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ScheduleGroupPrompt returning ssmlstring: {0}.", new object[]
			{
				base.SbSsml.ToString()
			});
			return base.SbSsml.ToString();
		}

		protected override void InternalInitialize()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ScheduleGroupPrompt initializing default SSML.", new object[0]);
			this.scheduleGroup = base.InitVal;
			string spokenScheduleGroupFormat = ScheduleGroupPrompt.GetSpokenScheduleGroupFormat(base.Culture);
			foreach (char c in spokenScheduleGroupFormat)
			{
				char c2 = c;
				if (c2 != 'd')
				{
					if (c2 == 'i')
					{
						using (List<TimeRange>.Enumerator enumerator = this.scheduleGroup.ScheduleIntervals.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TimeRange si = enumerator.Current;
								this.AddScheduleIntervalPrompt(si);
							}
							goto IL_BA;
						}
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, null, "ScheduleGroup ignoring invalided specifier '{0}'", new object[]
					{
						c
					});
				}
				else
				{
					this.AddDayOfWeekListPrompt();
				}
				IL_BA:;
			}
		}

		private static string GetSpokenScheduleGroupFormat(CultureInfo c)
		{
			return PromptConfigBase.PromptResourceManager.GetString(ScheduleGroupPrompt.SpokenScheduleGroupFormatId, c);
		}

		private void AddScheduleIntervalPrompt(TimeRange si)
		{
			ScheduleIntervalPrompt scheduleIntervalPrompt = new ScheduleIntervalPrompt(false);
			scheduleIntervalPrompt.Initialize(base.Config, base.Culture, si);
			base.AddPrompt(scheduleIntervalPrompt);
		}

		private void AddDayOfWeekListPrompt()
		{
			DayOfWeekListPrompt dayOfWeekListPrompt = new DayOfWeekListPrompt();
			dayOfWeekListPrompt.Initialize(base.Config, base.Culture, this.scheduleGroup.DaysOfWeek);
			base.AddPrompt(dayOfWeekListPrompt);
		}

		private static readonly string SpokenScheduleGroupFormatId = "SpokenScheduleGroupFormat";

		private ScheduleGroup scheduleGroup;
	}
}
