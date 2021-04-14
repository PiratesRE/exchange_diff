using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSessionContextNotSetByThisThreadException : EsentUsageException
	{
		public EsentSessionContextNotSetByThisThreadException() : base("Tried to reset session context, but current thread did not orignally set the session context", JET_err.SessionContextNotSetByThisThread)
		{
		}

		private EsentSessionContextNotSetByThisThreadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
