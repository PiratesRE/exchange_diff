using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIllegalOperationException : EsentUsageException
	{
		public EsentIllegalOperationException() : base("Oper. not supported on table", JET_err.IllegalOperation)
		{
		}

		private EsentIllegalOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
