using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRfsFailureException : EsentObsoleteException
	{
		public EsentRfsFailureException() : base("Resource Failure Simulator failure", JET_err.RfsFailure)
		{
		}

		private EsentRfsFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
