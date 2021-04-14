using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentAlreadyPreparedException : EsentUsageException
	{
		public EsentAlreadyPreparedException() : base("Attempted to update record when record update was already in progress", JET_err.AlreadyPrepared)
		{
		}

		private EsentAlreadyPreparedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
