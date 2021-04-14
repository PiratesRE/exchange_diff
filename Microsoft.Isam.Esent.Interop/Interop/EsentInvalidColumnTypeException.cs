using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidColumnTypeException : EsentUsageException
	{
		public EsentInvalidColumnTypeException() : base("Invalid column data type", JET_err.InvalidColumnType)
		{
		}

		private EsentInvalidColumnTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
