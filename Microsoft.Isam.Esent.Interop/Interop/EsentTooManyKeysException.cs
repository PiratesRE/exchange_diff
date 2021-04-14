using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyKeysException : EsentUsageException
	{
		public EsentTooManyKeysException() : base("Too many columns in an index", JET_err.TooManyKeys)
		{
		}

		private EsentTooManyKeysException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
