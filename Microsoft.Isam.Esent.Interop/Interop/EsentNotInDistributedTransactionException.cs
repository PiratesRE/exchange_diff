using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNotInDistributedTransactionException : EsentObsoleteException
	{
		public EsentNotInDistributedTransactionException() : base("Attempted to PrepareToCommit a non-distributed transaction", JET_err.NotInDistributedTransaction)
		{
		}

		private EsentNotInDistributedTransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
