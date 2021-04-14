using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Transport
{
	[Serializable]
	internal sealed class MemoryPressureException : SystemException
	{
		public MemoryPressureException() : base("The Process Manager indicated that process is under memory pressure.")
		{
		}

		public MemoryPressureException(uint percentLoad, ulong totalMemory) : base(string.Format(CultureInfo.InvariantCulture, "The Process Manager indicated that process is under memory pressure. The Percentage of physical memory in use is {0}%. The Total installed memory is bytes is {1}.", new object[]
		{
			percentLoad,
			totalMemory
		}))
		{
		}

		public MemoryPressureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private const string MemoryPressureMessage = "The Process Manager indicated that process is under memory pressure.";

		private const string MemoryPressureMessageWithStats = "The Process Manager indicated that process is under memory pressure. The Percentage of physical memory in use is {0}%. The Total installed memory is bytes is {1}.";
	}
}
