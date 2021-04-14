using System;

namespace Microsoft.Filtering.Results
{
	public class FilteringElapsedTimes
	{
		public TimeSpan Total { get; set; }

		public TimeSpan Scanning { get; set; }

		public TimeSpan Parsing { get; set; }

		public TimeSpan TextExtraction { get; set; }
	}
}
