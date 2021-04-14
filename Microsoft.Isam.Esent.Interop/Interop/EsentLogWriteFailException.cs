using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogWriteFailException : EsentIOException
	{
		public EsentLogWriteFailException() : base("Failure writing to log file", JET_err.LogWriteFail)
		{
		}

		private EsentLogWriteFailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
