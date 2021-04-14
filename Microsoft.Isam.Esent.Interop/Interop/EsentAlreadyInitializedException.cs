using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentAlreadyInitializedException : EsentUsageException
	{
		public EsentAlreadyInitializedException() : base("Database engine already initialized", JET_err.AlreadyInitialized)
		{
		}

		private EsentAlreadyInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
