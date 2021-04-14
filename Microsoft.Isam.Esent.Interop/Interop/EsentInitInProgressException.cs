using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInitInProgressException : EsentOperationException
	{
		public EsentInitInProgressException() : base("Database engine is being initialized", JET_err.InitInProgress)
		{
		}

		private EsentInitInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
