using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogDiskFullException : EsentDiskException
	{
		public EsentLogDiskFullException() : base("Log disk full", JET_err.LogDiskFull)
		{
		}

		private EsentLogDiskFullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
