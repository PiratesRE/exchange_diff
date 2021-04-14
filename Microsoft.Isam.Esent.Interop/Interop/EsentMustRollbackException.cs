using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMustRollbackException : EsentUsageException
	{
		public EsentMustRollbackException() : base("Transaction must rollback because failure of unversioned update", JET_err.MustRollback)
		{
		}

		private EsentMustRollbackException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
