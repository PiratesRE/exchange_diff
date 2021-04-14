using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMissingLogFileException : EsentCorruptionException
	{
		public EsentMissingLogFileException() : base("Current log file missing", JET_err.MissingLogFile)
		{
		}

		private EsentMissingLogFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
