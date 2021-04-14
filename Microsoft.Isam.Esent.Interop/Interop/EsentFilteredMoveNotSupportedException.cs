using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFilteredMoveNotSupportedException : EsentUsageException
	{
		public EsentFilteredMoveNotSupportedException() : base("Attempted to provide a filter to JetSetCursorFilter() in an unsupported scenario.", JET_err.FilteredMoveNotSupported)
		{
		}

		private EsentFilteredMoveNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
