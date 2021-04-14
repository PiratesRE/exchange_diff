using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadParentPageLinkException : EsentCorruptionException
	{
		public EsentBadParentPageLinkException() : base("Database corrupted", JET_err.BadParentPageLink)
		{
		}

		private EsentBadParentPageLinkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
