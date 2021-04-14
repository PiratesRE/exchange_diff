using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordTooBigException : EsentStateException
	{
		public EsentRecordTooBigException() : base("Record larger than maximum size", JET_err.RecordTooBig)
		{
		}

		private EsentRecordTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
