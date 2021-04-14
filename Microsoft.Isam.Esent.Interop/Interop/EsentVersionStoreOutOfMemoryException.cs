using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentVersionStoreOutOfMemoryException : EsentQuotaException
	{
		public EsentVersionStoreOutOfMemoryException() : base("Version store out of memory (cleanup already attempted)", JET_err.VersionStoreOutOfMemory)
		{
		}

		private EsentVersionStoreOutOfMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
