using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordNotDeletedException : EsentOperationException
	{
		public EsentRecordNotDeletedException() : base("Record has not been deleted", JET_err.RecordNotDeleted)
		{
		}

		private EsentRecordNotDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
