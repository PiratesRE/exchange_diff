using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadPageLinkException : EsentCorruptionException
	{
		public EsentBadPageLinkException() : base("Database corrupted", JET_err.BadPageLink)
		{
		}

		private EsentBadPageLinkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
