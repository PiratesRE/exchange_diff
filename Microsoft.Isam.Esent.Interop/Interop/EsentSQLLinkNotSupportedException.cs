using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSQLLinkNotSupportedException : EsentObsoleteException
	{
		public EsentSQLLinkNotSupportedException() : base("SQL Link support unavailable", JET_err.SQLLinkNotSupported)
		{
		}

		private EsentSQLLinkNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
