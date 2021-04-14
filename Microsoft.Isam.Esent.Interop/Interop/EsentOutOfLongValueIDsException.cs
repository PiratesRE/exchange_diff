using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfLongValueIDsException : EsentFragmentationException
	{
		public EsentOutOfLongValueIDsException() : base("Long-value ID counter has reached maximum value. (perform offline defrag to reclaim free/unused LongValueIDs)", JET_err.OutOfLongValueIDs)
		{
		}

		private EsentOutOfLongValueIDsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
