using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogCorruptedException : EsentCorruptionException
	{
		public EsentLogCorruptedException() : base("Logs could not be interpreted", JET_err.LogCorrupted)
		{
		}

		private EsentLogCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
