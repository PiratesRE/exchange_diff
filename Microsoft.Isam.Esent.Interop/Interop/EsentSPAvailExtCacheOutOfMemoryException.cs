using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSPAvailExtCacheOutOfMemoryException : EsentObsoleteException
	{
		public EsentSPAvailExtCacheOutOfMemoryException() : base("Out of memory allocating an AvailExt cache node", JET_err.SPAvailExtCacheOutOfMemory)
		{
		}

		private EsentSPAvailExtCacheOutOfMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
