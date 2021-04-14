using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogCorruptDuringHardRestoreException : EsentCorruptionException
	{
		public EsentLogCorruptDuringHardRestoreException() : base("corruption was detected in a backup set during hard restore", JET_err.LogCorruptDuringHardRestore)
		{
		}

		private EsentLogCorruptDuringHardRestoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
