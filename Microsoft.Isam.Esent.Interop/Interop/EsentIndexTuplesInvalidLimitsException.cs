using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesInvalidLimitsException : EsentUsageException
	{
		public EsentIndexTuplesInvalidLimitsException() : base("invalid min/max tuple length or max characters to index specified", JET_err.IndexTuplesInvalidLimits)
		{
		}

		private EsentIndexTuplesInvalidLimitsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
