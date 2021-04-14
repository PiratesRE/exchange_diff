using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentKeyBoundaryException : EsentObsoleteException
	{
		public EsentKeyBoundaryException() : base("Reached Key Boundary", JET_err.KeyBoundary)
		{
		}

		private EsentKeyBoundaryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
