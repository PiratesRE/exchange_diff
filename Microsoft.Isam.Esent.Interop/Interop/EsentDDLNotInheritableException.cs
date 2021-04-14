using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDDLNotInheritableException : EsentUsageException
	{
		public EsentDDLNotInheritableException() : base("Tried to inherit DDL from a table not marked as a template table.", JET_err.DDLNotInheritable)
		{
		}

		private EsentDDLNotInheritableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
