using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentKeyTruncatedException : EsentStateException
	{
		public EsentKeyTruncatedException() : base("key truncated on index that disallows key truncation", JET_err.KeyTruncated)
		{
		}

		private EsentKeyTruncatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
