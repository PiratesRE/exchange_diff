using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDiskIOException : EsentIOException
	{
		public EsentDiskIOException() : base("Disk IO error", JET_err.DiskIO)
		{
		}

		private EsentDiskIOException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
