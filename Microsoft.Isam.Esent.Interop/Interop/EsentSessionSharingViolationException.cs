using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSessionSharingViolationException : EsentUsageException
	{
		public EsentSessionSharingViolationException() : base("Multiple threads are using the same session", JET_err.SessionSharingViolation)
		{
		}

		private EsentSessionSharingViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
