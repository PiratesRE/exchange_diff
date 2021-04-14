using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordFormatConversionFailedException : EsentCorruptionException
	{
		public EsentRecordFormatConversionFailedException() : base("Internal error during dynamic record format conversion", JET_err.RecordFormatConversionFailed)
		{
		}

		private EsentRecordFormatConversionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
