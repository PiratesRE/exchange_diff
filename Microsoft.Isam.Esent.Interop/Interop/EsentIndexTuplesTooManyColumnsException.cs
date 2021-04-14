using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexTuplesTooManyColumnsException : EsentObsoleteException
	{
		public EsentIndexTuplesTooManyColumnsException() : base("tuple index may only have eleven columns in the index", JET_err.IndexTuplesTooManyColumns)
		{
		}

		private EsentIndexTuplesTooManyColumnsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
