using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNotInTransactionException : EsentUsageException
	{
		public EsentNotInTransactionException() : base("Operation must be within a transaction", JET_err.NotInTransaction)
		{
		}

		private EsentNotInTransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
