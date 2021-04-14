using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSecondaryIndexCorruptedException : EsentCorruptionException
	{
		public EsentSecondaryIndexCorruptedException() : base("Secondary index is corrupt. The database must be defragmented or the affected index must be deleted. If the corrupt index is over Unicode text, a likely cause a sort-order change.", JET_err.SecondaryIndexCorrupted)
		{
		}

		private EsentSecondaryIndexCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
