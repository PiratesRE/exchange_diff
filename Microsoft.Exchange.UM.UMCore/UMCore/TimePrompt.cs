using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TimePrompt : VariablePrompt<ExDateTime>
	{
		public TimePrompt()
		{
			base.InitVal = ExDateTime.MinValue;
		}

		internal TimePrompt(string promptName, CultureInfo culture, ExDateTime value) : base(promptName, culture, value)
		{
		}

		protected ExDateTime Time
		{
			get
			{
				return this.time;
			}
			set
			{
				this.time = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"time",
				base.Config.PromptName,
				string.Empty,
				this.time.ToString("t", base.Culture)
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Time prompt returning ssmlstring: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		internal void Initialize(ExDateTime dt, bool shouldAddDate, bool shouldAddMeridian)
		{
			this.time = dt;
			if (shouldAddDate && this.time.Date != ExDateTime.Today)
			{
				this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, this.time, SpokenDateTimeFormatter.GetSpokenDateTimeFormat(base.Culture), Strings.ShortTimeMonthDay(this.time.ToString("t", base.Culture), this.time.ToString("M", base.Culture)).ToString(base.Culture));
			}
			else if (shouldAddMeridian)
			{
				this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, this.time, SpokenDateTimeFormatter.GetSpokenTimeFormat(base.Culture), this.time.ToString("t", base.Culture));
			}
			else
			{
				this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, this.time, SpokenDateTimeFormatter.GetSpokenShortTimeFormat(base.Culture), this.time.ToString("t", base.Culture));
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TimePrompt successfully intialized time '{0}' with ssml '{1}'.", new object[]
			{
				this.time,
				this.ssmlString
			});
		}

		protected override void InternalInitialize()
		{
			this.Initialize(base.InitVal, true, true);
		}

		internal const string RecordedHourFileTemplate = "Hours-{0}.wav";

		internal const string RecordedMinuteFileTemplate = "Minutes-{0}.wav";

		internal const string RecordedMeridianFileTemplate = "Meridian-{0}.wav";

		internal const string TwentyFourHourPrefix = "24-";

		internal const string Recorded24HourFileTemplate = "24-Hours-{0}.wav";

		private ExDateTime time;

		private string ssmlString;
	}
}
