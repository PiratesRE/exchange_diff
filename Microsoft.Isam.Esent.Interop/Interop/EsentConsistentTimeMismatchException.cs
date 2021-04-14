using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentConsistentTimeMismatchException : EsentInconsistentException
	{
		public EsentConsistentTimeMismatchException() : base("Database last consistent time unmatched", JET_err.ConsistentTimeMismatch)
		{
		}

		private EsentConsistentTimeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
