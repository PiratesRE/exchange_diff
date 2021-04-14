using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnNotFoundException : EsentUsageException
	{
		public EsentColumnNotFoundException() : base("No such column", JET_err.ColumnNotFound)
		{
		}

		private EsentColumnNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
