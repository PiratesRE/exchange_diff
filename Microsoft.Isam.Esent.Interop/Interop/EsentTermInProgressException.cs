using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTermInProgressException : EsentOperationException
	{
		public EsentTermInProgressException() : base("Termination in progress", JET_err.TermInProgress)
		{
		}

		private EsentTermInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
