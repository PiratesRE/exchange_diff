using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNullKeyDisallowedException : EsentUsageException
	{
		public EsentNullKeyDisallowedException() : base("Null keys are disallowed on index", JET_err.NullKeyDisallowed)
		{
		}

		private EsentNullKeyDisallowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
