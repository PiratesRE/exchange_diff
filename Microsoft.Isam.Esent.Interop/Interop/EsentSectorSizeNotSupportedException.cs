using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSectorSizeNotSupportedException : EsentFatalException
	{
		public EsentSectorSizeNotSupportedException() : base("The physical sector size reported by the disk subsystem, is unsupported by ESE for a specific file type.", JET_err.SectorSizeNotSupported)
		{
		}

		private EsentSectorSizeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
