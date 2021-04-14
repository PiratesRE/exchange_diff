using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotNestDDLException : EsentUsageException
	{
		public EsentCannotNestDDLException() : base("Nesting of hierarchical DDL is not currently supported.", JET_err.CannotNestDDL)
		{
		}

		private EsentCannotNestDDLException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
