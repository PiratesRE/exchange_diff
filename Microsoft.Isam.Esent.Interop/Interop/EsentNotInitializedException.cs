using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNotInitializedException : EsentUsageException
	{
		public EsentNotInitializedException() : base("Database engine not initialized", JET_err.NotInitialized)
		{
		}

		private EsentNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
