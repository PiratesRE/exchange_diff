using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFixedDDLException : EsentUsageException
	{
		public EsentFixedDDLException() : base("DDL operations prohibited on this table", JET_err.FixedDDL)
		{
		}

		private EsentFixedDDLException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
