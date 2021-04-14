using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSurrogateBackupInProgressException : EsentStateException
	{
		public EsentSurrogateBackupInProgressException() : base("A surrogate backup is in progress.", JET_err.SurrogateBackupInProgress)
		{
		}

		private EsentSurrogateBackupInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
