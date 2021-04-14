using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnInUseException : EsentUsageException
	{
		public EsentColumnInUseException() : base("Column used in an index", JET_err.ColumnInUse)
		{
		}

		private EsentColumnInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
