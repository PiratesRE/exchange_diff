using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyOpenIndexesException : EsentMemoryException
	{
		public EsentTooManyOpenIndexesException() : base("Out of index description blocks", JET_err.TooManyOpenIndexes)
		{
		}

		private EsentTooManyOpenIndexesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
