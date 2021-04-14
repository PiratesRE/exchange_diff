using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesNonUniqueOnlyException : EsentUsageException
	{
		public EsentIndexTuplesNonUniqueOnlyException() : base("tuple index must be a non-unique index", JET_err.IndexTuplesNonUniqueOnly)
		{
		}

		private EsentIndexTuplesNonUniqueOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
