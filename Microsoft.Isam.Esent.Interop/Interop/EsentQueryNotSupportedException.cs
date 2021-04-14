using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentQueryNotSupportedException : EsentUsageException
	{
		public EsentQueryNotSupportedException() : base("Query support unavailable", JET_err.QueryNotSupported)
		{
		}

		private EsentQueryNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
