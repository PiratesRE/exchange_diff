using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnRedundantException : EsentUsageException
	{
		public EsentColumnRedundantException() : base("Second autoincrement or version column", JET_err.ColumnRedundant)
		{
		}

		private EsentColumnRedundantException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
