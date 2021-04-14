using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogTornWriteDuringHardRecoveryException : EsentCorruptionException
	{
		public EsentLogTornWriteDuringHardRecoveryException() : base("torn-write was detected during hard recovery (log was not part of a backup set)", JET_err.LogTornWriteDuringHardRecovery)
		{
		}

		private EsentLogTornWriteDuringHardRecoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
