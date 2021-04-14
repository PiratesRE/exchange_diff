using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesTextBinaryColumnsOnlyException : EsentUsageException
	{
		public EsentIndexTuplesTextBinaryColumnsOnlyException() : base("tuple index must be on a text/binary column", JET_err.IndexTuplesTextBinaryColumnsOnly)
		{
		}

		private EsentIndexTuplesTextBinaryColumnsOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
