using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSPOwnExtCorruptedException : EsentCorruptionException
	{
		public EsentSPOwnExtCorruptedException() : base("OwnExt space tree is corrupt", JET_err.SPOwnExtCorrupted)
		{
		}

		private EsentSPOwnExtCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
