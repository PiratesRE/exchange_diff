using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TimeRangePrompt : VariablePrompt<TimeRange>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"timeRange",
				base.Config.PromptName,
				string.Empty,
				this.range.ToString(base.Culture)
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimeRangePrompt returning ssmlstring: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.range = base.InitVal;
			this.IntializeSSML();
		}

		private void IntializeSSML()
		{
			if (!this.TryGetTimeRangeFileSSML(out this.ssmlString))
			{
				this.InitializeDefaultSSML();
			}
		}

		private void InitializeDefaultSSML()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimeRangePrompt initializing default SSML.", new object[0]);
			this.ssmlString = string.Empty;
			PromptConfigBase timeRange = GlobCfg.DefaultPrompts.TimeRange;
			ArrayList arrayList = new ArrayList();
			timeRange.AddPrompts(arrayList, null, base.Culture);
			bool flag = (this.range.EndTime - this.range.StartTime).TotalHours > 12.0;
			bool flag2 = this.range.EndTime.Date != this.range.StartTime.Date;
			foreach (object obj in arrayList)
			{
				Prompt prompt = (Prompt)obj;
				TimePrompt timePrompt = prompt as TimePrompt;
				if (timePrompt != null)
				{
					if (string.Compare(timePrompt.PromptName, "startTime", StringComparison.OrdinalIgnoreCase) == 0)
					{
						timePrompt.Initialize(this.range.StartTime, flag2, flag || flag2);
					}
					if (string.Compare(timePrompt.PromptName, "endTime", StringComparison.OrdinalIgnoreCase) == 0)
					{
						timePrompt.Initialize(this.range.EndTime, flag2, flag || flag2);
					}
				}
				this.ssmlString += prompt.ToSsml();
			}
		}

		private bool TryGetTimeRangeFileSSML(out string ssml)
		{
			ssml = null;
			if ((this.range.EndTime - this.range.StartTime).TotalMinutes > 120.0)
			{
				return false;
			}
			string value = SpokenDateTimeFormatter.NormalizeHour(base.Culture, this.range.StartTime);
			string value2 = SpokenDateTimeFormatter.NormalizeMinutes(this.range.StartTime);
			string value3 = SpokenDateTimeFormatter.NormalizeHour(base.Culture, this.range.EndTime);
			string value4 = SpokenDateTimeFormatter.NormalizeMinutes(this.range.EndTime);
			StringBuilder stringBuilder = new StringBuilder();
			if (CommonUtil.Is24HourTimeFormat(base.Culture.DateTimeFormat.ShortTimePattern))
			{
				stringBuilder.Append("24-");
			}
			stringBuilder.Append(value);
			stringBuilder.Append(value2);
			stringBuilder.Append("-");
			stringBuilder.Append(value3);
			stringBuilder.Append(value4);
			stringBuilder.Append(".wav");
			string text = Path.Combine(Util.WavPathFromCulture(base.Culture), stringBuilder.ToString());
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimeRangePrompt looking for file '{0}'.", new object[]
			{
				text
			});
			if (!File.Exists(text))
			{
				return false;
			}
			ssml = string.Format(CultureInfo.InvariantCulture, "<audio src=\"{0}\" />", new object[]
			{
				text
			});
			return true;
		}

		private TimeRange range;

		private string ssmlString;
	}
}
