using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidLoggedOperationException : EsentObsoleteException
	{
		public EsentInvalidLoggedOperationException() : base("Logged operation cannot be redone", JET_err.InvalidLoggedOperation)
		{
		}

		private EsentInvalidLoggedOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
