using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPageBoundaryException : EsentObsoleteException
	{
		public EsentPageBoundaryException() : base("Reached Page Boundary", JET_err.PageBoundary)
		{
		}

		private EsentPageBoundaryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
