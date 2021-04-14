using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidIndexIdException : EsentUsageException
	{
		public EsentInvalidIndexIdException() : base("Illegal index id", JET_err.InvalidIndexId)
		{
		}

		private EsentInvalidIndexIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
