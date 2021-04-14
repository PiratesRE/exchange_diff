using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogFileCorruptException : EsentCorruptionException
	{
		public EsentLogFileCorruptException() : base("Log file is corrupt", JET_err.LogFileCorrupt)
		{
		}

		private EsentLogFileCorruptException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
