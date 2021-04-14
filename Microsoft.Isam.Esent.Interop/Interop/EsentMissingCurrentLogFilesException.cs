using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMissingCurrentLogFilesException : EsentInconsistentException
	{
		public EsentMissingCurrentLogFilesException() : base("Some current log files are missing for continuous restore", JET_err.MissingCurrentLogFiles)
		{
		}

		private EsentMissingCurrentLogFilesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
