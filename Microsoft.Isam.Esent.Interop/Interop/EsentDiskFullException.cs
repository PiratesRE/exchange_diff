using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDiskFullException : EsentDiskException
	{
		public EsentDiskFullException() : base("No space left on disk", JET_err.DiskFull)
		{
		}

		private EsentDiskFullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
