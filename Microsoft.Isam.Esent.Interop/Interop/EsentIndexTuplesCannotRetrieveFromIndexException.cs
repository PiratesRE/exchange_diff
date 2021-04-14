using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesCannotRetrieveFromIndexException : EsentUsageException
	{
		public EsentIndexTuplesCannotRetrieveFromIndexException() : base("cannot call RetrieveColumn() with RetrieveFromIndex on a tuple index", JET_err.IndexTuplesCannotRetrieveFromIndex)
		{
		}

		private EsentIndexTuplesCannotRetrieveFromIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
