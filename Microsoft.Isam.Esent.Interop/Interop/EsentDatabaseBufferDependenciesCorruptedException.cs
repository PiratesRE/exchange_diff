using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseBufferDependenciesCorruptedException : EsentCorruptionException
	{
		public EsentDatabaseBufferDependenciesCorruptedException() : base("Buffer dependencies improperly set. Recovery failure", JET_err.DatabaseBufferDependenciesCorrupted)
		{
		}

		private EsentDatabaseBufferDependenciesCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
