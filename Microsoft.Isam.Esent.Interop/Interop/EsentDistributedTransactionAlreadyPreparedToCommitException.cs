using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDistributedTransactionAlreadyPreparedToCommitException : EsentObsoleteException
	{
		public EsentDistributedTransactionAlreadyPreparedToCommitException() : base("Attempted a write-operation after a distributed transaction has called PrepareToCommit", JET_err.DistributedTransactionAlreadyPreparedToCommit)
		{
		}

		private EsentDistributedTransactionAlreadyPreparedToCommitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
