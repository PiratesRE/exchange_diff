using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnNotUpdatableException : EsentUsageException
	{
		public EsentColumnNotUpdatableException() : base("Cannot set column value", JET_err.ColumnNotUpdatable)
		{
		}

		private EsentColumnNotUpdatableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
