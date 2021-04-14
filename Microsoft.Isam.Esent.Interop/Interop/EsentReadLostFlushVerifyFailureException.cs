using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentReadLostFlushVerifyFailureException : EsentCorruptionException
	{
		public EsentReadLostFlushVerifyFailureException() : base("The database page read from disk had a previous write not represented on the page.", JET_err.ReadLostFlushVerifyFailure)
		{
		}

		private EsentReadLostFlushVerifyFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
