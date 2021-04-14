using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesKeyTooSmallException : EsentUsageException
	{
		public EsentIndexTuplesKeyTooSmallException() : base("specified key does not meet minimum tuple length", JET_err.IndexTuplesKeyTooSmall)
		{
		}

		private EsentIndexTuplesKeyTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
