using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotNestDistributedTransactionsException : EsentObsoleteException
	{
		public EsentCannotNestDistributedTransactionsException() : base("Attempted to begin a distributed transaction when not at level 0", JET_err.CannotNestDistributedTransactions)
		{
		}

		private EsentCannotNestDistributedTransactionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
