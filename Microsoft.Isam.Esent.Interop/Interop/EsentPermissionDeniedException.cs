using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPermissionDeniedException : EsentUsageException
	{
		public EsentPermissionDeniedException() : base("Permission denied", JET_err.PermissionDenied)
		{
		}

		private EsentPermissionDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
