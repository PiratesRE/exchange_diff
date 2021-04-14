using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentReadPgnoVerifyFailureException : EsentCorruptionException
	{
		public EsentReadPgnoVerifyFailureException() : base("The database page read from disk had the wrong page number.", JET_err.ReadPgnoVerifyFailure)
		{
		}

		private EsentReadPgnoVerifyFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
