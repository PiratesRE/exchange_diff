using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogSectorSizeMismatchException : EsentFragmentationException
	{
		public EsentLogSectorSizeMismatchException() : base("the log file sector size does not match the current volume's sector size", JET_err.LogSectorSizeMismatch)
		{
		}

		private EsentLogSectorSizeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
