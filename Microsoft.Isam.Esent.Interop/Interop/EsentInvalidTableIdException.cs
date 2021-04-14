using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidTableIdException : EsentUsageException
	{
		public EsentInvalidTableIdException() : base("Invalid table id", JET_err.InvalidTableId)
		{
		}

		private EsentInvalidTableIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
