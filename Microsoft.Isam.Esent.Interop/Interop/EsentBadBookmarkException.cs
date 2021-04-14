using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadBookmarkException : EsentObsoleteException
	{
		public EsentBadBookmarkException() : base("Bookmark has no corresponding address in database", JET_err.BadBookmark)
		{
		}

		private EsentBadBookmarkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
