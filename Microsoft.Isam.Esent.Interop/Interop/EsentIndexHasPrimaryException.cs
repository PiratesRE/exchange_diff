using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexHasPrimaryException : EsentUsageException
	{
		public EsentIndexHasPrimaryException() : base("Primary index already defined", JET_err.IndexHasPrimary)
		{
		}

		private EsentIndexHasPrimaryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
