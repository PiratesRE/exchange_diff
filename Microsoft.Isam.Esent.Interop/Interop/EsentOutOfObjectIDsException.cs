using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfObjectIDsException : EsentFragmentationException
	{
		public EsentOutOfObjectIDsException() : base("Out of btree ObjectIDs (perform offline defrag to reclaim freed/unused ObjectIds)", JET_err.OutOfObjectIDs)
		{
		}

		private EsentOutOfObjectIDsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
