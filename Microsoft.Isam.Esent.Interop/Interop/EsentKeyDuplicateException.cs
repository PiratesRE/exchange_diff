using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentKeyDuplicateException : EsentStateException
	{
		public EsentKeyDuplicateException() : base("Illegal duplicate key", JET_err.KeyDuplicate)
		{
		}

		private EsentKeyDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
