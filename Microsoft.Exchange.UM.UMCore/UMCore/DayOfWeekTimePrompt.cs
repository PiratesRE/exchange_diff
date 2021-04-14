using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DayOfWeekTimePrompt : VariablePrompt<DayOfWeekTimeContext>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"dayOfWeekTime",
				base.Config.PromptName,
				string.Empty,
				this.logString
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DayOfWeekTimePrompt returning ssml string: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			if (base.InitVal == null)
			{
				return;
			}
			if (base.InitVal.ShowDay && base.InitVal.ShowTime)
			{
				this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, base.InitVal.DateTime, SpokenDateTimeFormatter.GetSpokenDayTimeFormat(base.Culture), this.GetWrittenDayTimeFormat());
				this.logString = this.GetWrittenDayTimeFormat();
			}
			else if (base.InitVal.ShowDay)
			{
				this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, base.InitVal.DateTime, "d", base.InitVal.DateTime.ToString("dddd", base.Culture));
				this.logString = base.InitVal.DateTime.ToString("dddd", base.Culture);
			}
			else if (base.InitVal.ShowTime)
			{
				this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, base.InitVal.DateTime, SpokenDateTimeFormatter.GetSpokenTimeFormatBusinessHours(base.Culture), this.GetWrittenTimeFormat());
				this.logString = this.GetWrittenTimeFormat();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DayOfWeekPrompt successfully intialized date '{0}' to ssml '{1}.", new object[]
			{
				base.InitVal.DateTime,
				this.ssmlString
			});
		}

		private string GetWrittenTimeFormat()
		{
			string empty = string.Empty;
			if (CommonUtil.Is24HourTimeFormat(base.Culture.DateTimeFormat.ShortTimePattern) || base.InitVal.DateTime.Minute != 0)
			{
				return base.InitVal.DateTime.ToString("t", base.Culture);
			}
			if (base.InitVal.DateTime.Hour == 0)
			{
				return Strings.TwelveMidnight.ToString(base.Culture);
			}
			if (base.InitVal.DateTime.Hour == 12)
			{
				return Strings.TwelveNoon.ToString(base.Culture);
			}
			return base.InitVal.DateTime.ToString(Strings.WrittenTimeWithZeroMinutesFormat.ToString(base.Culture), base.Culture);
		}

		private string GetWrittenDayTimeFormat()
		{
			return Strings.WeekDayShortTime(base.InitVal.DateTime.ToString("dddd", base.Culture), this.GetWrittenTimeFormat()).ToString(base.Culture);
		}

		internal const string RecordedDayOfWeekFileTemplate = "DayOfWeek-{0}.wav";

		private string ssmlString;

		private string logString;
	}
}
