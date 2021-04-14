using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentUpdateNotPreparedException : EsentUsageException
	{
		public EsentUpdateNotPreparedException() : base("No call to JetPrepareUpdate", JET_err.UpdateNotPrepared)
		{
		}

		private EsentUpdateNotPreparedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
