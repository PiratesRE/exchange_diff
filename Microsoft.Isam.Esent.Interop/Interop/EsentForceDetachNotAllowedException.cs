using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentForceDetachNotAllowedException : EsentUsageException
	{
		public EsentForceDetachNotAllowedException() : base("Force Detach allowed only after normal detach errored out", JET_err.ForceDetachNotAllowed)
		{
		}

		private EsentForceDetachNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
