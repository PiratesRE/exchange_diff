using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNoAttachmentsFailedIncrementalReseedException : EsentStateException
	{
		public EsentNoAttachmentsFailedIncrementalReseedException() : base("The incremental reseed being performed on the specified database cannot be completed because the min required log contains no attachment info.  A full reseed is required to recover this database.", JET_err.NoAttachmentsFailedIncrementalReseed)
		{
		}

		private EsentNoAttachmentsFailedIncrementalReseedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
