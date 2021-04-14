using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidOnSortException : EsentObsoleteException
	{
		public EsentInvalidOnSortException() : base("Invalid operation on Sort", JET_err.InvalidOnSort)
		{
		}

		private EsentInvalidOnSortException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
