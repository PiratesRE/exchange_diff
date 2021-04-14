using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidBackupSequenceException : EsentUsageException
	{
		public EsentInvalidBackupSequenceException() : base("Backup call out of sequence", JET_err.InvalidBackupSequence)
		{
		}

		private EsentInvalidBackupSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
