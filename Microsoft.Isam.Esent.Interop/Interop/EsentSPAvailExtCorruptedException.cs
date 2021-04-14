using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSPAvailExtCorruptedException : EsentCorruptionException
	{
		public EsentSPAvailExtCorruptedException() : base("AvailExt space tree is corrupt", JET_err.SPAvailExtCorrupted)
		{
		}

		private EsentSPAvailExtCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
