using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexBuildCorruptedException : EsentCorruptionException
	{
		public EsentIndexBuildCorruptedException() : base("Failed to build a secondary index that properly reflects primary index", JET_err.IndexBuildCorrupted)
		{
		}

		private EsentIndexBuildCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
