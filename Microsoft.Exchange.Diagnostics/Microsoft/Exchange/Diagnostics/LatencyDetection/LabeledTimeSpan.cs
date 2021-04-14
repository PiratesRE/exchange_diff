using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal class LabeledTimeSpan
	{
		internal LabeledTimeSpan(string label, TimeSpan timeSpan)
		{
			this.label = label;
			this.timeSpan = timeSpan;
		}

		internal string Label
		{
			get
			{
				return this.label;
			}
		}

		internal TimeSpan TimeSpan
		{
			get
			{
				return this.timeSpan;
			}
		}

		private readonly string label;

		private readonly TimeSpan timeSpan;
	}
}
