using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfSessionsException : EsentMemoryException
	{
		public EsentOutOfSessionsException() : base("Out of sessions", JET_err.OutOfSessions)
		{
		}

		private EsentOutOfSessionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
