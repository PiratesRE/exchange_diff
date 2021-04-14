using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DatePrompt : VariablePrompt<ExDateTime>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"date",
				base.Config.PromptName,
				string.Empty,
				this.date.ToString("M", base.Culture)
			});
		}

		internal override string ToSsml()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Date prompt returning ssml string: {0}.", new object[]
			{
				this.ssmlString
			});
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.date = base.InitVal;
			this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, this.date, SpokenDateTimeFormatter.GetSpokenDateFormat(base.Culture), this.date.ToString("M", base.Culture));
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DatePrompt successfully intialized date '{0}' to ssml '{1}.", new object[]
			{
				this.date,
				this.ssmlString
			});
		}

		internal const string RecordedMonthFileTemplate = "Month-{0}.wav";

		internal const string RecordedDayFileTemplate = "Day-{0}.wav";

		private ExDateTime date;

		private string ssmlString;
	}
}
