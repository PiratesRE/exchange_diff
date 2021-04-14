using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTempPathInUseException : EsentUsageException
	{
		public EsentTempPathInUseException() : base("Temp path already used by another database instance", JET_err.TempPathInUse)
		{
		}

		private EsentTempPathInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
