using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseLeakInSpaceException : EsentStateException
	{
		public EsentDatabaseLeakInSpaceException() : base("Some database pages have become unreachable even from the avail tree, only an offline defragmentation can return the lost space.", JET_err.DatabaseLeakInSpace)
		{
		}

		private EsentDatabaseLeakInSpaceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
