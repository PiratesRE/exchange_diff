using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidGrbitException : EsentUsageException
	{
		public EsentInvalidGrbitException() : base("Invalid flags parameter", JET_err.InvalidGrbit)
		{
		}

		private EsentInvalidGrbitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
