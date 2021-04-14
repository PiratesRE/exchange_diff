using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidSesidException : EsentUsageException
	{
		public EsentInvalidSesidException() : base("Invalid session handle", JET_err.InvalidSesid)
		{
		}

		private EsentInvalidSesidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
