using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSesidTableIdMismatchException : EsentUsageException
	{
		public EsentSesidTableIdMismatchException() : base("This session handle can't be used with this table id", JET_err.SesidTableIdMismatch)
		{
		}

		private EsentSesidTableIdMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
