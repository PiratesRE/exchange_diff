using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMissingPreviousLogFileException : EsentCorruptionException
	{
		public EsentMissingPreviousLogFileException() : base("Missing the log file for check point", JET_err.MissingPreviousLogFile)
		{
		}

		private EsentMissingPreviousLogFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
