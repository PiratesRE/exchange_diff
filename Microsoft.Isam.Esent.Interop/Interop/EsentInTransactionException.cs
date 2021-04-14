using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInTransactionException : EsentUsageException
	{
		public EsentInTransactionException() : base("Operation not allowed within a transaction", JET_err.InTransaction)
		{
		}

		private EsentInTransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
