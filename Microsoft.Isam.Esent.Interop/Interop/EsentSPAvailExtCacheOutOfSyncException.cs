using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSPAvailExtCacheOutOfSyncException : EsentObsoleteException
	{
		public EsentSPAvailExtCacheOutOfSyncException() : base("AvailExt cache doesn't match btree", JET_err.SPAvailExtCacheOutOfSync)
		{
		}

		private EsentSPAvailExtCacheOutOfSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
