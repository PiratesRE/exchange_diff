using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnNoChunkException : EsentUsageException
	{
		public EsentColumnNoChunkException() : base("No such chunk in long value", JET_err.ColumnNoChunk)
		{
		}

		private EsentColumnNoChunkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
