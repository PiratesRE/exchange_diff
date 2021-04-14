using System;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public sealed class CorruptedPerformanceCountersException : Exception
	{
		public CorruptedPerformanceCountersException(Exception innerException) : base(string.Empty, innerException)
		{
		}
	}
}
