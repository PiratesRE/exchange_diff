using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidOperationException : EsentUsageException
	{
		public EsentInvalidOperationException() : base("Invalid operation", JET_err.InvalidOperation)
		{
		}

		private EsentInvalidOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
