using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDistributedTransactionNotYetPreparedToCommitException : EsentObsoleteException
	{
		public EsentDistributedTransactionNotYetPreparedToCommitException() : base("Attempted to commit a distributed transaction, but PrepareToCommit has not yet been called", JET_err.DistributedTransactionNotYetPreparedToCommit)
		{
		}

		private EsentDistributedTransactionNotYetPreparedToCommitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
