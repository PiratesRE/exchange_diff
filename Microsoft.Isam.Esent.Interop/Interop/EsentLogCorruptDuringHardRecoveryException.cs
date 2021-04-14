using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogCorruptDuringHardRecoveryException : EsentCorruptionException
	{
		public EsentLogCorruptDuringHardRecoveryException() : base("corruption was detected during hard recovery (log was not part of a backup set)", JET_err.LogCorruptDuringHardRecovery)
		{
		}

		private EsentLogCorruptDuringHardRecoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
