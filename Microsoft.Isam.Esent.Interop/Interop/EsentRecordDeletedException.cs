using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordDeletedException : EsentStateException
	{
		public EsentRecordDeletedException() : base("Record has been deleted", JET_err.RecordDeleted)
		{
		}

		private EsentRecordDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
