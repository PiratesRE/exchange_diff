using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidObjectException : EsentObsoleteException
	{
		public EsentInvalidObjectException() : base("Object is invalid for operation", JET_err.InvalidObject)
		{
		}

		private EsentInvalidObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
