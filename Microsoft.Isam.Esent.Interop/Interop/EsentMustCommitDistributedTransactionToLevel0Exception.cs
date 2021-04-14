using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMustCommitDistributedTransactionToLevel0Exception : EsentObsoleteException
	{
		public EsentMustCommitDistributedTransactionToLevel0Exception() : base("Attempted to PrepareToCommit a distributed transaction to non-zero level", JET_err.MustCommitDistributedTransactionToLevel0)
		{
		}

		private EsentMustCommitDistributedTransactionToLevel0Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
