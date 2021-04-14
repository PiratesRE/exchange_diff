using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SimpleTimePrompt : VariablePrompt<ExDateTime>
	{
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
			return this.ssmlString;
		}

		protected override void InternalInitialize()
		{
			this.time = base.InitVal;
			this.ssmlString = SpokenDateTimeFormatter.ToSsml(base.Culture, this.time, SpokenDateTimeFormatter.GetSpokenTimeFormat(base.Culture), this.time.ToString("t", base.Culture));
		}

		private ExDateTime time;

		private string ssmlString;
	}
}
