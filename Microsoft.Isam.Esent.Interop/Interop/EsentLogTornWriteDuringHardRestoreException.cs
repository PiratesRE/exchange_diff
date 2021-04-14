using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogTornWriteDuringHardRestoreException : EsentCorruptionException
	{
		public EsentLogTornWriteDuringHardRestoreException() : base("torn-write was detected in a backup set during hard restore", JET_err.LogTornWriteDuringHardRestore)
		{
		}

		private EsentLogTornWriteDuringHardRestoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
