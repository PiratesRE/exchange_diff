using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordPrimaryChangedException : EsentUsageException
	{
		public EsentRecordPrimaryChangedException() : base("Primary key may not change", JET_err.RecordPrimaryChanged)
		{
		}

		private EsentRecordPrimaryChangedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
