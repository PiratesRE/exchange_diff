using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRollbackErrorException : EsentFatalException
	{
		public EsentRollbackErrorException() : base("error during rollback", JET_err.RollbackError)
		{
		}

		private EsentRollbackErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
