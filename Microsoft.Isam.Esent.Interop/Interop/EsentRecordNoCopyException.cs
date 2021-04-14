using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordNoCopyException : EsentUsageException
	{
		public EsentRecordNoCopyException() : base("No working buffer", JET_err.RecordNoCopy)
		{
		}

		private EsentRecordNoCopyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
