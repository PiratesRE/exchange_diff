using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFixedInheritedDDLException : EsentUsageException
	{
		public EsentFixedInheritedDDLException() : base("On a derived table, DDL operations are prohibited on inherited portion of DDL", JET_err.FixedInheritedDDL)
		{
		}

		private EsentFixedInheritedDDLException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
