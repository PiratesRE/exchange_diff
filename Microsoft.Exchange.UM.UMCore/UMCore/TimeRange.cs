using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TimeRange
	{
		internal TimeRange(ExDateTime st, ExDateTime et)
		{
			this.startTime = st;
			this.endTime = et;
		}

		internal ExDateTime StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		internal ExDateTime EndTime
		{
			get
			{
				return this.endTime;
			}
		}

		public override string ToString()
		{
			return this.ToString(Thread.CurrentThread.CurrentCulture);
		}

		internal string ToString(CultureInfo c)
		{
			return this.startTime.ToString("t", c) + "-" + this.endTime.ToString("t", c);
		}

		private ExDateTime startTime;

		private ExDateTime endTime;
	}
}
