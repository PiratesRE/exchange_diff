using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPageNotInitializedException : EsentCorruptionException
	{
		public EsentPageNotInitializedException() : base("Blank database page", JET_err.PageNotInitialized)
		{
		}

		private EsentPageNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
