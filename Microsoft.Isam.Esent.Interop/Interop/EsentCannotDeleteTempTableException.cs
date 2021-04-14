using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotDeleteTempTableException : EsentUsageException
	{
		public EsentCannotDeleteTempTableException() : base("Use CloseTable instead of DeleteTable to delete temp table", JET_err.CannotDeleteTempTable)
		{
		}

		private EsentCannotDeleteTempTableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
