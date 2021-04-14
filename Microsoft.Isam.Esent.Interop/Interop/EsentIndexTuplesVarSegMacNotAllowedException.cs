using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesVarSegMacNotAllowedException : EsentUsageException
	{
		public EsentIndexTuplesVarSegMacNotAllowedException() : base("tuple index does not allow setting cbVarSegMac", JET_err.IndexTuplesVarSegMacNotAllowed)
		{
		}

		private EsentIndexTuplesVarSegMacNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
