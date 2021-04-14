using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogFileSizeMismatchException : EsentUsageException
	{
		public EsentLogFileSizeMismatchException() : base("actual log file size does not match JET_paramLogFileSize", JET_err.LogFileSizeMismatch)
		{
		}

		private EsentLogFileSizeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
