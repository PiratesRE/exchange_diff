using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyIndexesException : EsentUsageException
	{
		public EsentTooManyIndexesException() : base("Too many indexes", JET_err.TooManyIndexes)
		{
		}

		private EsentTooManyIndexesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
