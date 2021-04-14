using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnDoesNotFitException : EsentUsageException
	{
		public EsentColumnDoesNotFitException() : base("Field will not fit in record", JET_err.ColumnDoesNotFit)
		{
		}

		private EsentColumnDoesNotFitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
