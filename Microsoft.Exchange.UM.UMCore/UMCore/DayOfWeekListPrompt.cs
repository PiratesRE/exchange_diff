using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DayOfWeekListPrompt : VariablePrompt<List<ExDateTime>>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"dayOfWeekList",
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DayOfWeekListPrompt returning ssmlstring: {0}.", new object[]
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
			this.days = base.InitVal.ToArray();
			int num = this.days[this.days.Length - 1].DayOfWeek - this.days[0].DayOfWeek;
			num = ((num < 0) ? (num + 7) : num);
			if (num == this.days.Length - 1 && this.days.Length > 2)
			{
				if (this.days.Length != 7)
				{
					this.AddRangePrompt();
					return;
				}
			}
			else
			{
				this.AddOneByOnePrompt();
			}
		}

		private void AddRangePrompt()
		{
			ArrayList prompts = GlobCfg.DefaultPromptForAAHelper.Build(null, base.Culture, new PromptConfigBase[]
			{
				GlobCfg.DefaultPromptsForAA.DayRange
			});
			VariablePrompt<DayOfWeekTimeContext>.SetActualPromptValues(prompts, "startDay", DayOfWeekTimeContext.WithDayOnly(this.days[0]));
			VariablePrompt<DayOfWeekTimeContext>.SetActualPromptValues(prompts, "endDay", DayOfWeekTimeContext.WithDayOnly(this.days[this.days.Length - 1]));
			base.AddPrompts(prompts);
		}

		private void AddOneByOnePrompt()
		{
			ArrayList arrayList = new ArrayList();
			foreach (ExDateTime dt in this.days)
			{
				DayOfWeekTimePrompt dayOfWeekTimePrompt = new DayOfWeekTimePrompt();
				dayOfWeekTimePrompt.Initialize(base.Config, base.Culture, DayOfWeekTimeContext.WithDayOnly(dt));
				arrayList.Add(dayOfWeekTimePrompt);
			}
			base.AddPrompts(arrayList);
		}

		private ExDateTime[] days;
	}
}
