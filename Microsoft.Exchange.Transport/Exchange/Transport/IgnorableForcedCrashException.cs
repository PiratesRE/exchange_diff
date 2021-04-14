using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Transport
{
	[Serializable]
	internal sealed class IgnorableForcedCrashException : ApplicationException
	{
		public IgnorableForcedCrashException() : base("Crashing the process to generate a crash dump. Please ignore this crash.")
		{
		}

		public IgnorableForcedCrashException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private const string ForcedCrashExceptionMessage = "Crashing the process to generate a crash dump. Please ignore this crash.";
	}
}
