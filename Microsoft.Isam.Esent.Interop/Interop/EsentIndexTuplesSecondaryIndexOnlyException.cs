using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesSecondaryIndexOnlyException : EsentUsageException
	{
		public EsentIndexTuplesSecondaryIndexOnlyException() : base("tuple index can only be on a secondary index", JET_err.IndexTuplesSecondaryIndexOnly)
		{
		}

		private EsentIndexTuplesSecondaryIndexOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
