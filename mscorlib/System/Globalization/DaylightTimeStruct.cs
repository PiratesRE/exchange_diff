using System;

namespace System.Globalization
{
	internal struct DaylightTimeStruct
	{
		public DaylightTimeStruct(DateTime start, DateTime end, TimeSpan delta)
		{
			this.Start = start;
			this.End = end;
			this.Delta = delta;
		}

		public DateTime Start { get; }

		public DateTime End { get; }

		public TimeSpan Delta { get; }
	}
}
