using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class OccurrenceTimeSpanTooBigException : RecurrenceException
	{
		public OccurrenceTimeSpanTooBigException(TimeSpan occurrenceTimeSpan, TimeSpan minimumTimeBetweenOccurrence, LocalizedString message) : base(message)
		{
			this.OccurrenceTimeSpan = occurrenceTimeSpan;
			this.MinimumTimeBetweenOccurrences = minimumTimeBetweenOccurrence;
		}

		protected OccurrenceTimeSpanTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.OccurrenceTimeSpan = (TimeSpan)info.GetValue("OccurrenceTimeSpan", typeof(TimeSpan));
			this.MinimumTimeBetweenOccurrences = (TimeSpan)info.GetValue("MinimumTimeBetweenOccurrences", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("OccurrenceTimeSpan", this.OccurrenceTimeSpan);
			info.AddValue("MinimumTimeBetweenOccurrences", this.MinimumTimeBetweenOccurrences);
		}

		private const string OccurrenceTimeSpanLabel = "OccurrenceTimeSpan";

		private const string MinimumTimeBetweenOccurrencesLabel = "MinimumTimeBetweenOccurrences";

		public readonly TimeSpan OccurrenceTimeSpan;

		public readonly TimeSpan MinimumTimeBetweenOccurrences;
	}
}
