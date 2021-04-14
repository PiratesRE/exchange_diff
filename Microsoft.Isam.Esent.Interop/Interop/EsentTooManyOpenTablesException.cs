using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyOpenTablesException : EsentQuotaException
	{
		public EsentTooManyOpenTablesException() : base("Cannot open any more tables (cleanup already attempted)", JET_err.TooManyOpenTables)
		{
		}

		private EsentTooManyOpenTablesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
