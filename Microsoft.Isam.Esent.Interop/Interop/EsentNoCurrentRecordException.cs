using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNoCurrentRecordException : EsentStateException
	{
		public EsentNoCurrentRecordException() : base("Currency not on a record", JET_err.NoCurrentRecord)
		{
		}

		private EsentNoCurrentRecordException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
