using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDbTimeCorruptedException : EsentCorruptionException
	{
		public EsentDbTimeCorruptedException() : base("Dbtime on current page is greater than global database dbtime", JET_err.DbTimeCorrupted)
		{
		}

		private EsentDbTimeCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
