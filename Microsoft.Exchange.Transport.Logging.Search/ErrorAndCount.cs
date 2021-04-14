using System;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class ErrorAndCount : IComparable<ErrorAndCount>
	{
		public int CompareTo(ErrorAndCount other)
		{
			return this.Count.CompareTo(other.Count);
		}

		public string Error;

		public int Count;

		internal enum Properties
		{
			Error,
			Count
		}
	}
}
