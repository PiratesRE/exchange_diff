using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSystemPathInUseException : EsentUsageException
	{
		public EsentSystemPathInUseException() : base("System path already used by another database instance", JET_err.SystemPathInUse)
		{
		}

		private EsentSystemPathInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
