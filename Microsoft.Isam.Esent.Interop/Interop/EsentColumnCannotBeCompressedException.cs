using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnCannotBeCompressedException : EsentUsageException
	{
		public EsentColumnCannotBeCompressedException() : base("Only JET_coltypLongText and JET_coltypLongBinary columns can be compressed", JET_err.ColumnCannotBeCompressed)
		{
		}

		private EsentColumnCannotBeCompressedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
