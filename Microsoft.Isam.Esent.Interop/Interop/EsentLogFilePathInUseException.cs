using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogFilePathInUseException : EsentUsageException
	{
		public EsentLogFilePathInUseException() : base("Logfile path already used by another database instance", JET_err.LogFilePathInUse)
		{
		}

		private EsentLogFilePathInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
