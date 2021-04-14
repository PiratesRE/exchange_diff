using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDerivedColumnCorruptionException : EsentCorruptionException
	{
		public EsentDerivedColumnCorruptionException() : base("Invalid column in derived table", JET_err.DerivedColumnCorruption)
		{
		}

		private EsentDerivedColumnCorruptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
