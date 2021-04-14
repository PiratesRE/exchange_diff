using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManySortsException : EsentMemoryException
	{
		public EsentTooManySortsException() : base("Too many sort processes", JET_err.TooManySorts)
		{
		}

		private EsentTooManySortsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
