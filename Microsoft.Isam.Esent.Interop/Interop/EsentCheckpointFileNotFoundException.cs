using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCheckpointFileNotFoundException : EsentInconsistentException
	{
		public EsentCheckpointFileNotFoundException() : base("Could not locate checkpoint file", JET_err.CheckpointFileNotFound)
		{
		}

		private EsentCheckpointFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
