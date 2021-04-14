using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLinkNotSupportedException : EsentObsoleteException
	{
		public EsentLinkNotSupportedException() : base("Link support unavailable", JET_err.LinkNotSupported)
		{
		}

		private EsentLinkNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
