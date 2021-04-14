using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileCompressedException : EsentUsageException
	{
		public EsentFileCompressedException() : base("read/write access is not supported on compressed files", JET_err.FileCompressed)
		{
		}

		private EsentFileCompressedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
