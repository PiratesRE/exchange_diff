using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentReadVerifyFailureException : EsentCorruptionException
	{
		public EsentReadVerifyFailureException() : base("Checksum error on a database page", JET_err.ReadVerifyFailure)
		{
		}

		private EsentReadVerifyFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
