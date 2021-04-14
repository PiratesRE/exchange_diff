using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRestoreInProgressException : EsentStateException
	{
		public EsentRestoreInProgressException() : base("Restore in progress", JET_err.RestoreInProgress)
		{
		}

		private EsentRestoreInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
