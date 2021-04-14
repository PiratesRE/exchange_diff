using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTransactionTooLongException : EsentQuotaException
	{
		public EsentTransactionTooLongException() : base("Too many outstanding generations between JetBeginTransaction and current generation.", JET_err.TransactionTooLong)
		{
		}

		private EsentTransactionTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
