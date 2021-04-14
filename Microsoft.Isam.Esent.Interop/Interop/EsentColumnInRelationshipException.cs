using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnInRelationshipException : EsentObsoleteException
	{
		public EsentColumnInRelationshipException() : base("Cannot delete, column participates in relationship", JET_err.ColumnInRelationship)
		{
		}

		private EsentColumnInRelationshipException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
