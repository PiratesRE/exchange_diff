using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotMaterializeForwardOnlySortException : EsentUsageException
	{
		public EsentCannotMaterializeForwardOnlySortException() : base("The temp table could not be created due to parameters that conflict with JET_bitTTForwardOnly", JET_err.CannotMaterializeForwardOnlySort)
		{
		}

		private EsentCannotMaterializeForwardOnlySortException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
