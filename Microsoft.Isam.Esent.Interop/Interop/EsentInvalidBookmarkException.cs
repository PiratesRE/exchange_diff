using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidBookmarkException : EsentUsageException
	{
		public EsentInvalidBookmarkException() : base("Invalid bookmark", JET_err.InvalidBookmark)
		{
		}

		private EsentInvalidBookmarkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
