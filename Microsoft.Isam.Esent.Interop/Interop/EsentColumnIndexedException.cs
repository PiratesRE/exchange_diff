using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnIndexedException : EsentObsoleteException
	{
		public EsentColumnIndexedException() : base("Column indexed, cannot delete", JET_err.ColumnIndexed)
		{
		}

		private EsentColumnIndexedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
