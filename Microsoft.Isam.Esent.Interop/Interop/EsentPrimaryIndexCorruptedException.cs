using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPrimaryIndexCorruptedException : EsentCorruptionException
	{
		public EsentPrimaryIndexCorruptedException() : base("Primary index is corrupt. The database must be defragmented or the table deleted.", JET_err.PrimaryIndexCorrupted)
		{
		}

		private EsentPrimaryIndexCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
