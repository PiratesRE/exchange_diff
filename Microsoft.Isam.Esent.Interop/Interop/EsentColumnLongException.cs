using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnLongException : EsentObsoleteException
	{
		public EsentColumnLongException() : base("Column value is long", JET_err.ColumnLong)
		{
		}

		private EsentColumnLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
