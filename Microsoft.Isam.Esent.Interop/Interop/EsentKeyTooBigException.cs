using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentKeyTooBigException : EsentObsoleteException
	{
		public EsentKeyTooBigException() : base("Key is too large", JET_err.KeyTooBig)
		{
		}

		private EsentKeyTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
