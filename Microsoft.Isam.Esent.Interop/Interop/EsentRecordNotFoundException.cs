using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRecordNotFoundException : EsentStateException
	{
		public EsentRecordNotFoundException() : base("The key was not found", JET_err.RecordNotFound)
		{
		}

		private EsentRecordNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
