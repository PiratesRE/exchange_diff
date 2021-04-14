using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMissingRestoreLogFilesException : EsentInconsistentException
	{
		public EsentMissingRestoreLogFilesException() : base("Some restore log files are missing", JET_err.MissingRestoreLogFiles)
		{
		}

		private EsentMissingRestoreLogFilesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
