using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLVCorruptedException : EsentCorruptionException
	{
		public EsentLVCorruptedException() : base("Corruption encountered in long-value tree", JET_err.LVCorrupted)
		{
		}

		private EsentLVCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
