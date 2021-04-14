using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadEmptyPageException : EsentCorruptionException
	{
		public EsentBadEmptyPageException() : base("Database corrupted. Searching an unexpectedly empty page.", JET_err.BadEmptyPage)
		{
		}

		private EsentBadEmptyPageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
