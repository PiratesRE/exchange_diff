using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ScheduleGroupListPrompt : VariablePrompt<List<ScheduleGroup>>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"scheduleGroupList",
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "ScheduleGroupListPrompt returning ssmlstring: {0}.", new object[]
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
			foreach (ScheduleGroup group in base.InitVal)
			{
				this.AddScheduleGroupPrompt(group);
			}
		}

		private void AddScheduleGroupPrompt(ScheduleGroup group)
		{
			ScheduleGroupPrompt scheduleGroupPrompt = new ScheduleGroupPrompt();
			scheduleGroupPrompt.Initialize(base.Config, base.Culture, group);
			base.AddPrompt(scheduleGroupPrompt);
			base.SbSsml.Append("<break time=\"400ms\"/>");
		}
	}
}
