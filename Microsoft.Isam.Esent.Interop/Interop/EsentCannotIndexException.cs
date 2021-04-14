using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotIndexException : EsentUsageException
	{
		public EsentCannotIndexException() : base("Cannot index escrow column", JET_err.CannotIndex)
		{
		}

		private EsentCannotIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
