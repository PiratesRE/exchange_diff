using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCheckpointCorruptException : EsentCorruptionException
	{
		public EsentCheckpointCorruptException() : base("Checkpoint file not found or corrupt", JET_err.CheckpointCorrupt)
		{
		}

		private EsentCheckpointCorruptException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
