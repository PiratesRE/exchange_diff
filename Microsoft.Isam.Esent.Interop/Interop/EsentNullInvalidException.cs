using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNullInvalidException : EsentUsageException
	{
		public EsentNullInvalidException() : base("Null not valid", JET_err.NullInvalid)
		{
		}

		private EsentNullInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
