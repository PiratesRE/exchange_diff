using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidCreateIndexException : EsentUsageException
	{
		public EsentInvalidCreateIndexException() : base("Invalid create index description", JET_err.InvalidCreateIndex)
		{
		}

		private EsentInvalidCreateIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
