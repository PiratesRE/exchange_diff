using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTransReadOnlyException : EsentUsageException
	{
		public EsentTransReadOnlyException() : base("Read-only transaction tried to modify the database", JET_err.TransReadOnly)
		{
		}

		private EsentTransReadOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
