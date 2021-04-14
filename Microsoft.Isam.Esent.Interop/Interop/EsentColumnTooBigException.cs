using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnTooBigException : EsentUsageException
	{
		public EsentColumnTooBigException() : base("Field length is greater than maximum", JET_err.ColumnTooBig)
		{
		}

		private EsentColumnTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
