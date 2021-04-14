using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSessionContextAlreadySetException : EsentUsageException
	{
		public EsentSessionContextAlreadySetException() : base("Specified session already has a session context set", JET_err.SessionContextAlreadySet)
		{
		}

		private EsentSessionContextAlreadySetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
