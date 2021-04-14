using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotAddFixedVarColumnToDerivedTableException : EsentObsoleteException
	{
		public EsentCannotAddFixedVarColumnToDerivedTableException() : base("Template table was created with NoFixedVarColumnsInDerivedTables", JET_err.CannotAddFixedVarColumnToDerivedTable)
		{
		}

		private EsentCannotAddFixedVarColumnToDerivedTableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
